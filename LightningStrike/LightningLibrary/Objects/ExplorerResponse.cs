using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LightningLibrary.Objects
{
    public class ExplorerResponse : BaseResponse
    {
        public string data { get; set; }

        public ExplorerResponse() : base()
        {

        }

        /// <summary>
        /// Converts the json string in 'data' to type T
        /// </summary>
        /// <returns>The deserialized data.</returns>
        /// <typeparam name="T">The desired object data type</typeparam>
        public T Convert<T>()
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            throw new ArgumentNullException("data property cannot be null");
        }

        public T Convert<T>(JsonConverter[] converter)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                return JsonConvert.DeserializeObject<T>(data, converter);
            }
            throw new ArgumentNullException("data property cannot be null");
        }
        public T Convert<T>(JsonSerializerSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                return JsonConvert.DeserializeObject<T>(data, settings);
            }
            throw new ArgumentNullException("data property cannot be null");
        }
    }
}
