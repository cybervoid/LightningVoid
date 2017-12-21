using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
namespace LightningLibrary.RPC
{
    public class GRPCClient : ClientBase
    {
        /// <summary>
        /// https://grpc.io/docs/tutorials/basic/csharp.html#client
        /// </summary>
        public void Run()
        {
            //var options = ChannelOptions.
            Channel channel = new Channel("192.168.1.18:9735", ChannelCredentials.Insecure);
            
            //var client = new RouteGuideClient(new RouteGuide.RouteGuideClient(channel));

            channel.ShutdownAsync().Wait();
        }
    }

    //public class RouteGuideClient
    //{
    //    readonly RouteGuide.RouteGuideClient client;

    //    public RouteGuideClient(RouteGuide.RouteGuideClient client)
    //    {
    //        this.client = client;
    //    }
    //}
}
