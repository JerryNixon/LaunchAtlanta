using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaunchAtlanta.FaceSwap.Web.Models
{
    public class RequestBodyData
    {
        public string FaceFileName { get; set; }
        public string EnvFileName { get; set; }
        public string Key { get; set; }
    }
}