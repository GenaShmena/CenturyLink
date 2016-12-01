using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ServerTrack.Models;
using ServerTrack.DataAccess;

namespace ServerTrack
{
    public static class ErrorHolder
    {
        public static HttpStatusCode StatusCode { get; set; }
        public static string Description { get; set; }
    }
}