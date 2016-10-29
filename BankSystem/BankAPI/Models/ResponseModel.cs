using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BankAPI.Helper;

namespace BankAPI.Models
{
    public class ResponseModel
    {
        public ResponseCode Status { get; set; }
        public string Description { get; set; }

        public ResponseModel()
        {
            
        }

        public ResponseModel(ResponseCode code)
        {
            Status = code;
            Description = Utilities.ResponseDictionary[code];
        }

    }
}