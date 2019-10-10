using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MMS.SupportedPlatforms.HitBtc
{
    public class Utilities
    {
        public static T ConverFromJason<T>(ApiResponse response) where T : class, new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch (Exception)
            {
                return new T();
            }
        }

        public static List<T> ConverFromJasons<T>(ApiResponse response) where T : class, new()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<T>>(response.Content);
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
