using LightningLibrary.RPC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace LightningLibrary.Utilities
{
    public static class Extensions
    {
        public static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Any()) ? attributes.First().Description : value.ToString();
        }

        //public static string ToJsonMethodName(this RpcMethod value)
        //{
        //    return GetEnumDescription(value);
        //}


        //public static string ToJsonMethodName(this BlockTag value)
        //{
        //    return GetEnumDescription(value);
        //}

        public static bool IsUri(this string source)
        {
            if (!string.IsNullOrEmpty(source) && Uri.IsWellFormedUriString(source, UriKind.RelativeOrAbsolute))
            {
                Uri tempValue;
                return (Uri.TryCreate(source, UriKind.RelativeOrAbsolute, out tempValue));
            }
            return (false);
        }

        //public static T FromJson<T>(this object obj)
        //{
        //    return (T)JsonConvert.DeserializeObject<T>(obj);
                    
        //}

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
