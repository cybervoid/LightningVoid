using System;
using System.Collections.Generic;
using NBitcoin;
using NBitcoin.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LightningLibrary.Utilities
{
    public static class Helpers
    {
        public static void ChangeColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }


        public static void ChangeColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }

        public static void ResetColor()
        {
            Console.ResetColor();
        }

        public static List<JsonConverter> GetConverters(Network network = null)
        {
            List<JsonConverter> converters = new List<JsonConverter>();

            converters.Add(new MoneyJsonConverter());
            converters.Add(new KeyJsonConverter());
            converters.Add(new CoinJsonConverter(network));
            converters.Add(new ScriptJsonConverter());
            converters.Add(new UInt160JsonConverter());
            converters.Add(new UInt256JsonConverter());
            converters.Add(new BitcoinSerializableJsonConverter());
            converters.Add(new NetworkJsonConverter());
            converters.Add(new KeyPathJsonConverter());
            converters.Add(new SignatureJsonConverter());
            converters.Add(new HexJsonConverter());
            converters.Add(new DateTimeToUnixTimeConverter());
            converters.Add(new TxDestinationJsonConverter());
            converters.Add(new LockTimeJsonConverter());
            converters.Add(new BitcoinStringJsonConverter()
            {
                Network = network
            });
            return converters;
        }

        public static JsonSerializerSettings GetSettings(Network network = null)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters = GetConverters(network);
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return settings;
        }
    }
}
