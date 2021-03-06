﻿using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ss.RealEstate.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Ss.RealEstate.Library2
{
    public class CrawlerForProperty
    {
        //This is called if we want to get the addresses via the html way
        public static List<AddressInfo> GetAddresses(string cityOrZip, uint minPrice, uint maxPrice, uint minYearBuilt, uint maxYearBuilt, out bool isPaginated)
        {
            var addressList = new List<AddressInfo>();
            HtmlWeb web = new HtmlWeb();
            var criteriaUrlPortion = Constants.CriteriaUrlPortion
                                                .Replace("{CityOrZip}", cityOrZip)
                                                .Replace("{MinPrice}", minPrice.ToString())
                                                .Replace("{MaxPrice}", maxPrice.ToString())
                                                .Replace("{MinYearBuilt}", minYearBuilt.ToString())
                                                .Replace("{MaxYearBuilt}", maxYearBuilt.ToString());
            HtmlDocument doc = web.Load(Constants.BaseUrl + criteriaUrlPortion);

            //Get Addresses
            var nodeList = doc.DocumentNode.SelectNodes(Constants.XPathForAddress);
            if (nodeList == null || nodeList.Count <= 0) nodeList = doc.DocumentNode.SelectNodes(Constants.XPathForPropertyInfo);

            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (var node in nodeList)
                {
                    var address = new AddressInfo();
                    address.DetailsLink = node.Attributes["href"].Value;
                    address.FullAddress = node.InnerText.Replace("#", "Unit").Insert(node.InnerText.Trim().Length - 5, " ");
                    addressList.Add(address);
                }
            }

            var paginationList = doc.DocumentNode.SelectNodes(Constants.XPathForPagination); 
            isPaginated = (paginationList != null && paginationList.Count > 0) ? true : false;

            return addressList;
        }
        
        //This is called if we want to get addresses via the JSON way
        public static List<AddressInfo> GetAddressList(string cityOrZip, uint minPrice, uint maxPrice, uint minYearBuilt, uint maxYearBuilt, out bool isPaginated)
        {
            HtmlWeb web1 = new HtmlWeb();
            string northeastlng = string.Empty;
            string northeastlat = string.Empty;
            string southeastlng = string.Empty;
            string southeastlat = string.Empty;

            using (WebClient wc1 = new WebClient())
            {
                var json1 = wc1.DownloadString("http://maps.googleapis.com/maps/api/geocode/json?address=" + cityOrZip);
                JObject json11 = JObject.Parse(json1);
                dynamic dynJson1 = JsonConvert.DeserializeObject(json11.ToString());
                int inc1 = 0;
                foreach (var item in dynJson1["results"][0]["geometry"]["bounds"])
                {
                    if (inc1 == 0)
                    {
                        northeastlng = Convert.ToString(item.Value["lng"].Value);
                        northeastlat = Convert.ToString(item.Value["lat"].Value);
                        northeastlat = northeastlat.Substring(0, northeastlat.Length - 1);

                    }
                    if (inc1 == 1)
                    {
                        southeastlng = Convert.ToString(item.Value["lng"]);
                        southeastlat = Convert.ToString(item.Value["lat"]);
                        southeastlat = southeastlat.Substring(0, southeastlat.Length - 1);
                    }
                    inc1++;
                }
            }
            //string googleapicoordinates = northeastlng.Replace(".", "") + "," + northeastlat.Replace(".", "") + "," + southeastlng.Replace(".", "") + "," + southeastlat.Replace(".", "");
            string googleapicoordinates = southeastlng.Replace(".", "") + "," + southeastlat.Replace(".", "") + "," + northeastlng.Replace(".", "") + "," + northeastlat.Replace(".", "");

            var addressList = new List<AddressInfo>();
            HtmlWeb web = new HtmlWeb();
            using (WebClient wc = new WebClient())
            {
                //var criteriaUrlPortion = Constants.CriteriaUrlPortion
                //                                    .Replace("{CityOrZip}", cityOrZip)
                //                                    .Replace("{MinPrice}", minPrice)
                //                                    .Replace("{MaxPrice}", maxPrice)
                //                                    .Replace("{MinYearBuilt}", minYearBuilt)
                //                                    .Replace("{MaxYearBuilt}", maxYearBuilt);

                var json = wc.DownloadString(Constants.BaseUrl1.Replace("{googleapicoordinates}", googleapicoordinates));
                JObject json1 = JObject.Parse(json);
                dynamic dynJson = JsonConvert.DeserializeObject(json1.ToString());
                int inc = 0;
                foreach (var item in dynJson)
                {
                    if (inc == 1)
                    {
                        //if (item.Value == "" || item.Value == null)
                        //    return addressList;

                        for (int j = 0; j <= item.Value["properties"].Count - 1; j++)
                        {
                            var address = new AddressInfo();
                            address.DetailsLink = "";
                            address.FullAddress = item.Value["properties"][j][0];
                            addressList.Add(address);
                        }
                    }
                    inc++;
                }
            }

            //TODO 
            isPaginated = true;

            return addressList.Take(30).ToList();
        }

        internal static PropertyInfo GetPropertyInfo(AddressInfo address, bool usePropertyIdForUrl)
        {
            var url = GetUrl(string.Format("{0}", address.FullAddress), usePropertyIdForUrl);

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            //json call
            var addressNode = doc.DocumentNode.SelectNodes(Constants.XPathForFullAddress);
            if (addressNode != null && addressNode.Count > 0)
            {
                address.FullAddress = addressNode[0].InnerText.Trim();
            }

            //Get Zestimate and ZRent
            string zestimateStr = string.Empty, zRentStr = string.Empty;
            var nodeList = doc.DocumentNode.SelectNodes(Constants.XPathForZestimateAndZRent);
            if (nodeList == null || nodeList.Count <= 0)
            {
                nodeList = doc.DocumentNode.SelectNodes(Constants.XPathForZestimateAndZRentAlt);
            }
            if (nodeList != null && nodeList.Count > 0)
            {
                zestimateStr = nodeList[0].InnerText;
                zRentStr = nodeList[1].InnerText;
            }

            //Get SaleType 
            string saleType = string.Empty;
            var saleTypeList = doc.DocumentNode.SelectNodes(Constants.XPathForSaleType);
            if (saleTypeList != null && saleTypeList.Count > 0 && !string.IsNullOrEmpty(saleTypeList[0].InnerText))
                saleType = (saleTypeList[0].InnerText.Trim().ToLower() == "for sale by owner") ? "FSBO" : saleTypeList[0].InnerText;

            //Get Listed Price
            string listedPriceStr = string.Empty;
            var priceNodeList = doc.DocumentNode.SelectNodes(Constants.XPathForListedPrice);
            if (priceNodeList != null && priceNodeList.Count > 0) listedPriceStr = priceNodeList[0].InnerText;
            var listedPrice = Utility.GetUnsignedIntFromString(listedPriceStr);

            //Get HOA, HomeType, YearBuilt, MLS Number, PricePerSqft
            uint yearBuilt = 0, mlsNumber = 0, hoa = 0;
            var homeType = string.Empty;
            var factsList = doc.DocumentNode.SelectNodes(Constants.XPathForFacts);
            if (factsList != null && factsList.Count > 0)
            {
                foreach (var node in factsList)
                {
                    var nodeStr = node.InnerText.Trim().ToLower();
                    if (nodeStr.Contains("built")) yearBuilt = Utility.GetUnsignedIntFromString(nodeStr);
                    if (nodeStr.Contains("hoa")) hoa = Utility.GetUnsignedIntFromString(nodeStr);
                    if (nodeStr.Contains("single")) homeType = "SF";
                    if (nodeStr.Contains("condo")) homeType = "C";
                    if (nodeStr.Contains("townhouse")) homeType = "TH";
                    if (nodeStr.Contains("mls")) mlsNumber = Utility.GetUnsignedIntFromString(nodeStr);
                    //if (nodeStr.Contains("price/sqft")) pricePerSqft = Utility.GetUnsignedIntFromString(nodeStr); 
                }
            }

            //Get Stories, UnitCount, LastRemodelYear, FinishedSize
            uint stories = 0, unitCount = 0, finishedSize = 0, lastRemodelYear = 0, pricePerSqft = 0;
            var constructionList = doc.DocumentNode.SelectNodes(Constants.XPathForAdditionalFeatures);
            if (constructionList != null && constructionList.Count > 0)
            {
                foreach (var node in constructionList)
                {
                    var nodeStr = node.InnerText.Trim().ToLower();
                    if (nodeStr.Contains("stories")) stories = Utility.GetUnsignedIntFromString(nodeStr);
                    if (nodeStr.Contains("unit count")) unitCount = Utility.GetUnsignedIntFromString(nodeStr);
                    if (nodeStr.Contains("floor size")) finishedSize = Utility.GetUnsignedIntFromString(nodeStr);
                    if (nodeStr.Contains("last remodel year")) lastRemodelYear = Utility.GetUnsignedIntFromString(nodeStr);
                }

                if (listedPrice > 0 && finishedSize > 0) pricePerSqft = listedPrice / finishedSize;
            }

            //Get School information
            uint elementarySchoolRating = 0, middleSchoolRating = 0, highSchoolRating = 0;
            var schoolRatingList = doc.DocumentNode.SelectNodes(Constants.XPathForSchools);
            if (schoolRatingList != null && schoolRatingList.Count > 0)
            {
                elementarySchoolRating = Utility.GetUnsignedIntFromString(schoolRatingList[0].InnerText);
                middleSchoolRating = schoolRatingList.Count > 1 ? Utility.GetUnsignedIntFromString(schoolRatingList[1].InnerText) : 0;
                highSchoolRating = schoolRatingList.Count > 2 ? Utility.GetUnsignedIntFromString(schoolRatingList[2].InnerText) : 0;
            }

            //Get Bedrooms and Bathrooms
            uint bedrooms = 0;
            double bathrooms = 0.0;
            var addrList = doc.DocumentNode.SelectNodes(Constants.XPathForBedAndBathrooms);
            if (addrList != null && addrList.Count > 0)
            {
                bedrooms = Utility.GetUnsignedIntFromString(addrList[0].InnerText);
                bathrooms = Utility.GetDoubleFromString(addrList[1].InnerText);
            }

            //Get city
            var city = string.Empty;
            var cityList = doc.DocumentNode.SelectNodes(Constants.XPathForCity);
            if (cityList != null && cityList.Count > 0)
            {
                city = cityList[0].InnerText;
            }

            var prpInfo = new PropertyInfo
            {
                ListedPrice = listedPrice,
                HomeType = homeType,
                SaleType = saleType,
                YearBuilt = yearBuilt,
                Hoa = hoa,
                MlsNumber = mlsNumber,
                PricePerSqft = pricePerSqft,
                UnitCount = unitCount,
                Stories = stories,
                Bedrooms = bedrooms,
                Bathrooms = bathrooms,
                ElementarySchoolRating = elementarySchoolRating,
                MiddleSchoolRating = middleSchoolRating,
                HighSchoolRating = highSchoolRating,
                FinishedSize = finishedSize,
                LastRemodelYear = lastRemodelYear,
                ZRent = Utility.GetUnsignedIntFromString(zRentStr),
                ZAmount = Utility.GetUnsignedIntFromString(zestimateStr),
                HomeDetailsLink = address.DetailsLink,
                Address = new AddressInfo() { FullAddress = address.FullAddress, City = city, Zip = address.FullAddress.Trim().Substring(address.FullAddress.Trim().Length - 5) }
            };

            return prpInfo;
        }

        internal static string GetUrl(string portion, bool usePropertyIdForUrl)
        {
            return Constants.BaseUrl + (!usePropertyIdForUrl ? portion.Replace(" ", "+") + "_rb" : portion + "_zpid");
        }
    }
}
