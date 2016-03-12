using Newtonsoft.Json;
using QuandlCS.Connection;
using QuandlCS.Interfaces;
using QuandlCS.Requests;
using QuandlCS.Types;
using System;
using System.Collections.Generic;

namespace Ss.RealEstate.Library2
{
    public class QuandlApi
    {
        //private static IEnumerable<string[]> _lines = File.ReadAllLines((Utility.CurrentAssemblyDirectory() + "/city_codes.csv")).Select(a => a.Split('\n'));

        //public static double GetSalePricePerSqft(string city, string state)
        //{
        //    if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(state)) return 0.0; 

        //    var cityCode = string.Empty;
        //    foreach (var line in _lines)
        //    {
        //        var elements = line[0].Split(',');
        //        if (elements[0].Trim().ToLower() == city.ToLower() && elements[1].Trim().ToLower() == state.ToLower()) cityCode = elements[3].Split('|')[1];
        //    }
        //    if (string.IsNullOrEmpty(cityCode)) return 0.0;
        //    var dataCode = new Datacode("ZILL", string.Format("{0}{1}_{2}", "C", cityCode, "MSPSF"));

        //    var request = new QuandlDownloadRequest()
        //    {
        //        APIKey = "7gtXr7SxAGBBgT_XhoCA",
        //        Datacode = dataCode,
        //        Format = FileFormats.JSON,
        //        Frequency = Frequencies.Monthly,
        //        Truncation = 1,
        //        Sort = SortOrders.Descending,
        //        Transformation = Transformations.None,
        //        StartDate = new DateTime(2015, 01, 01),
        //        EndDate = new DateTime(2016, 03, 01)
        //    };

        //    IQuandlConnection connection = new QuandlConnection();
        //    var data = connection.Request(request);

        //    dynamic obj = JsonConvert.DeserializeObject(data);

        //    return Convert.ToDouble(obj.data[0][1]);
        //}

        public static double GetSalePricePerSqft(string zipCode)
        {
            if (string.IsNullOrEmpty(zipCode)) return 0.0;

            var dataCode = new Datacode("ZILL", string.Format("{0}{1}_{2}", "Z", zipCode, "MSPSF"));

            var request = new QuandlDownloadRequest()
            {
                APIKey = "7gtXr7SxAGBBgT_XhoCA",
                Datacode = dataCode,
                Format = FileFormats.JSON,
                Frequency = Frequencies.Monthly,
                Truncation = 1,
                Sort = SortOrders.Descending,
                Transformation = Transformations.None,
                StartDate = new DateTime(2015, 01, 01),
                EndDate = new DateTime(2016, 03, 01)
            };

            IQuandlConnection connection = new QuandlConnection();
            var data = connection.Request(request);

            dynamic obj = JsonConvert.DeserializeObject(data);
            var value = Convert.ToDouble(obj.data[0][1]);

            return value; 
        }
    }
}
