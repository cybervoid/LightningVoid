using System;
using System.Collections.Generic;
using NBitcoin.RPC;
using Newtonsoft.Json;
using RestSharp;
using LightningLibrary.Objects;
namespace LightningLibrary.REST
{
    /// <summary>
    /// Documentation: https://blockexplorer.com/api-ref
    /// </summary>
    public class BlockExplorer
    {
        string url = "https://blockexplorer.com/";
        RestSharp.RestClient client;
        public BlockExplorer()
        {
            client = new RestSharp.RestClient(url);
        }

        public void GetUnspent(string address, bool noCache = false)
        {
            // /api/addr/[:addr]/utxo[?noCache=1]
            string resource = $"/api/addr/{address}/utxo";
            if (noCache == true)
                resource += "?noCache=1";
            var request = new RestRequest(resource, Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string
            Console.WriteLine(content);
            List<Objects.ExplorerUnspent> unspent = JsonConvert.DeserializeObject<List<ExplorerUnspent>>(content);

            string x = "";
        }
    }
}
