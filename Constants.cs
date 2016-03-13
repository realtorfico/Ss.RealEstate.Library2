namespace Ss.RealEstate.Library2
{
    internal class Constants
    {
        internal const string Root = "http://www.zillow.com/webservice/";
        internal const string BaseUrl = "http://www.zillow.com/homes/for_sale/";
        //internal const string BaseUrl1 = "http://www.zillow.com/search/GetResults.htm?spt=homes&status=100000&lt=110101&ht=111001&pr=49918,600121&mp=181,2176&bd=1%2C&ba=0%2C&sf=,&lot=,&yr=1980,&pho=0&pets=0&parking=0&laundry=0&income-restricted=0&pnd=0&red=0&zso=0&days=any&ds=all&pmf=0&pf=0&zoom=11&rect={googleapicoordinates}&p=1&sort=globalrelevanceex&search=map&disp=1&rid=52650&rt=6&listright=true&isMapSearch=1&zoom=11";

        internal const string BaseUrl1 = "http://www.zillow.com/search/GetResults.htm?" +
                    "spt=homes&status=110001&lt=111001&ht=111001&pr=100000,1000000&mp=359,3593&bd=0%2C&ba=0%2C&sf=,&lot=,&yr=,&pho=0&pets=0&parking=0&" +
                    "laundry=0&income-restricted=0&pnd=0&red=0&zso=0&days=any&ds=all&pmf=1&pf=1&zoom=14&rect={googleapicoordinates}&p=1&" +
                    "sort=globalrelevanceex&search=maplist&disp=1&listright=true&photoCardsEnabled=true&isMapSearch=true&zoom=14";

        internal const string CriteriaUrlPortion = "{CityOrZip}-CA/fsba,fsbo,new_lt/house,condo,apartment_duplex,townhouse_type/{MinPrice}-{MaxPrice}_price/90_days/{MinYearBuilt}-{MaxYearBuilt}_built/";
        //internal const string ServiceId = "X1-ZWz19u37vgjjez_8gte4";
        internal const string DeepSearchResultsMethodName = "GetDeepSearchResults";
        internal const string UpdatedPropertyDetailsMethodName = "GetUpdatedPropertyDetails";
        internal const string DeepCompsMethodName = "GetDeepComps";
        internal const int AcceptableBasicScore = 2500;

        internal const string XPathForPropertyInfo          = "//dt[@class='property-info']//a"; 
        internal const string XPathForAddress               = "//dt[@class='property-address']//a";
        internal const string XPathForFullAddress           = "//header[@class='zsg-content-header addr']//h1"; 
        internal const string XPathForListedPrice           = "//div[@class='main-row  home-summary-row']/span";
        internal const string XPathForSaleType              = "//div[@class=' status-icon-row for-sale-row home-summary-row']";
        internal const string XPathForFacts                 = "//div[@class='fact-group-container zsg-content-component top-facts']//li";
        internal const string XPathForZestimateAndZRent     = "//div[@class='zsg-g zest-double']//div[@class='zest-value']";
        internal const string XPathForZestimateAndZRentAlt  = "//div[@class='zsg-g ']//div[@class='zest-value']";
        internal const string XPathForAdditionalFeatures    = "//div[@class='fact-group-container zsg-content-component z-moreless-content hide']//ul[@class='zsg-list_square zsg-lg-1-3 zsg-md-1-2 zsg-sm-1-1']//li";
        internal const string XPathForSchools               = "//section[@id='nearbySchools']//ul[@class='nearby-schools-list']//div[@class='nearby-schools-rating']//span";
        internal const string XPathForBedAndBathrooms       = "//header[@class='zsg-content-header addr']//span[@class='addr_bbs']";
        internal const string XPathForCity                  = "//section[@id='nearbySchools']//h2//span//span";
        internal const string XPathForPagination            = "//div[@id='search-pagination-wrapper']//li"; 

        internal const uint ElementarySchoolDefaultRating = 5;
        internal const uint ElementarySchoolDefaultWeight = 6;
        internal const uint MiddleSchoolDefaultRating = 5;
        internal const uint MiddleSchoolDefaultWeight = 4;
        internal const uint HighSchoolDefaultRating = 5;
        internal const uint HighSchoolDefaultWeight = 2;
        internal const uint BedroomsDefaultValue = 2;
        internal const uint BedroomsDefaultWeight = 3;
        internal const uint BathroomsDefaultValue = 2;
        internal const uint BathroomsDefaultWeight = 2; 
    }
}
