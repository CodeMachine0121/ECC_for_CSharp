using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECC_Practice.Models;
using Newtonsoft.Json;

namespace ECC_Practice
{
    public class DataGenerator
    {
        public static DataObject getData()
        {
            return new DataObject()
            {
                VehId = "2",
                VehType = "ICE",
                VehClass = "Car",
                EngineConfandDisp = "4-FI 2.0L T/C",
                Transmission = "NO DATA",
                DriveWheels = "NO DATA",
                GeneralWeight = "3500"
            };
            
        }

        public static string DatatoString(DataObject data)
        {
            string strData = "";

            var json = JsonConvert.SerializeObject(data);
            strData = json.ToString();
            Console.WriteLine(strData);
            return strData;

        }


    }
}
