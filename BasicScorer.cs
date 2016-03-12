using System;
using System.Collections.Generic;
using System.Linq;
using Ss.RealEstate.Model;

namespace Ss.RealEstate.Library2
{
    public class BasicScorer
    {
        private Dictionary<string, double> _zipCodeDict = new Dictionary<string, double>();

        #region Public Methods
        public List<PropertyInfo> GetBasicScore(List<AddressInfo> addressList)
        {
            return addressList.Select(GetBasicScore).ToList();
        }

        public PropertyInfo GetBasicScore(AddressInfo address)
        {
            var prpInfo = CrawlerForProperty.GetPropertyInfo(address);

            if (prpInfo.ZAmount <= 0 || prpInfo.ListedPrice <= 0 || prpInfo.ZRent <= 0 || prpInfo.ZRent > prpInfo.ZAmount || 
                    prpInfo.ZRent > prpInfo.ListedPrice || (prpInfo.HomeType.ToLower() == "c" && prpInfo.Hoa == 0) || 
                    prpInfo.SaleType.Trim().ToLower() == "auction" || prpInfo.SaleType.Trim().ToLower() == "coming soon") return prpInfo;

            //Now calculate the score because all needed information is available
            var first = (prpInfo.ListedPrice * 100) / Convert.ToUInt32(prpInfo.ZAmount);   //This could range between 50 thru 200, mostly hovering around 100
            var second = Math.Floor(prpInfo.ListedPrice / (Convert.ToUInt32(prpInfo.ZRent) - prpInfo.Hoa) * 1.5);   //Could range between 120 thru 510, mostly hovering around 300 
            var score = Convert.ToUInt32(900 - (first + Convert.ToUInt32(second)));

            //Year Built
            if (prpInfo.YearBuilt > 1900) score += (prpInfo.YearBuilt - 1980) * 3;

            //Schools
            score += (prpInfo.ElementarySchoolRating - Constants.ElementarySchoolDefaultRating) * Constants.ElementarySchoolDefaultWeight;    //ElementarySchoolRating
            score += (prpInfo.MiddleSchoolRating - Constants.MiddleSchoolDefaultRating) * Constants.MiddleSchoolDefaultWeight;    //MiddleSchoolRating
            score += (prpInfo.HighSchoolRating - Constants.HighSchoolDefaultRating) * Constants.HighSchoolDefaultWeight;    //HighSchoolRating

            //Bedrooms
            score += (prpInfo.Bedrooms - Constants.BedroomsDefaultValue) * Constants.BedroomsDefaultWeight;     

            //Bathrooms
            score += (uint) Math.Round((prpInfo.Bathrooms - Constants.BathroomsDefaultValue) * Constants.BathroomsDefaultWeight);

            //Finished Size
            score += prpInfo.FinishedSize == 1000 ? 0 : (uint) (((int)prpInfo.FinishedSize-1000) / 25);

            //Last Sold Date
            DateTime lastSoldDate;
            if (DateTime.TryParse(prpInfo.LastSoldDate, out lastSoldDate)) { score += (2010 - (uint)lastSoldDate.Year); }

            //Price Per Square Feet - Using 90% of City's PPS if it is Condo or TownHouse and 100% if it is SF
            if (prpInfo.Address != null && !string.IsNullOrEmpty(prpInfo.Address.Zip))
            {
                var zip = prpInfo.Address.Zip.Trim();
                double zipPps = 0.0;
                if (_zipCodeDict.ContainsKey(zip)) zipPps = _zipCodeDict[zip]; 
                else { zipPps = QuandlApi.GetSalePricePerSqft(zip); _zipCodeDict.Add(zip, zipPps); }
                score += (uint)((zipPps > 0) ? (((prpInfo.HomeType == "C" || prpInfo.HomeType == "TH") ? zipPps*.9 : zipPps) - prpInfo.PricePerSqft) : 0);
            }

            prpInfo.DesirabilityScore = score;

            //1 point every 1000 sqft - this is being dropped off because of complications (because some complexes give complete complex lot size for the condo)
            //uint lotSize; if (uint.TryParse(propInfo.LotSize, out lotSize)) total += lotSize/1000;                  

            return prpInfo;
        }

        public Dictionary<string, double> GetZipPps()
        {
            return _zipCodeDict;
        }
        #endregion
    }
}
