﻿using System;
namespace LightningLibrary.RPC
{
    public class RPCClient
    {

        private string _Authentication;
        private readonly Uri _Address;
        public RPCClient(string connectionString)
        {
            //this._Address = new Uri(address);
        }

        private Uri BuildUri(string connectionString)
        {
            throw new NotImplementedException("todo");   
        }
    }
}
