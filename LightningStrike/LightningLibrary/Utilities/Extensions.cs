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

        public static bool IsUri(this string source)
        {
            if (!string.IsNullOrEmpty(source) && Uri.IsWellFormedUriString(source, UriKind.RelativeOrAbsolute))
            {
                Uri tempValue;
                return (Uri.TryCreate(source, UriKind.RelativeOrAbsolute, out tempValue));
            }
            return (false);
        }


        public static string AsJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
