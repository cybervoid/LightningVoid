using System;
using System.Collections.Generic;

namespace LightningLibrary.Objects
{
    public class BaseRequest
    {
        
    }

    public class BaseResponse
    {
        public List<Exception> Errors { get; set; }

        public BaseResponse()
        {
            
        }

        public bool HasErrors()
        {
            if(Errors != null && Errors.Count >0 )
                return true;
            
            return false;
        }

        public void AddError(Exception exception)
        {
            if(Errors == null)
                Errors = new List<Exception>();

            Errors.Add(exception);
        }
    }
}