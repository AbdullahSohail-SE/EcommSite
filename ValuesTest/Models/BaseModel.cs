using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Web.Models
{
    public class BaseModel
    {
        public string ServerUrl { get; private set; }

        public BaseModel()
        {
            ServerUrl = ConfigurationManager.AppSettings["Url"].ToString();
        }
    }
}