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

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

/// <summary>
/// Controller class
/// </summary>
namespace ServerTrack.Controllers
{
    //[RoutePrefix("{version}")]
    /// <summary>
    /// ServerTrackController class
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class ServerTrackController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ServerTrackController));

        /// <summary>
        /// Pings this instance.
        /// </summary>
        /// <returns>bool</returns>
        [HttpGet]
        [Route("Ping")]
        public bool Ping()
        {
            return true;
        }

        /// <summary>
        /// Gets the perf counters.
        /// </summary>
        /// <param name="uriParams">The URI get parameters.</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [ResponseType(typeof(ServerTrack.Models.ServerTrack))]
        [Route("PerfCounters")]
        public async Task<IHttpActionResult> GetPerfCounters([FromUri]UriParams uriParams)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            System.Web.Http.Results.ResponseMessageResult resultMessage = null;

            ServerTrack.DataManager.DataManager dm = new ServerTrack.DataManager.DataManager();
            ServerTrack.Models.ServerTrack st = await dm.MapDataTableToServerTrackModel(uriParams.SrvName, uriParams.TimeInterval);
            if (st == null)
            {

                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                resultMessage = new System.Web.Http.Results.ResponseMessageResult(responseMessage);
                return resultMessage;
            }
            return Ok(st);
        }

        /// <summary>
        /// Posts the perf counters.
        /// </summary>
        /// <param name="perfCounters">The perf counters.</param>
        /// <param name="srvName">Name of the SRV.</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("PerfCounters/{SrvName:regex((?i)(?=.{5,20}$)^(([a-z\\d]|[a-z\\d][a-z\\d\\-]*[a-z\\d])\\.)*([a-z\\d]|[a-z\\d][a-z\\d\\-]*[a-z\\d])$)}")]
        public async Task<IHttpActionResult> PostPerfCounters([FromUri]PerfCounters perfCounters, string srvName)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            System.Web.Http.Results.ResponseMessageResult resultMessage = null;

            ServerTrack.DataManager.DataManager dm = new ServerTrack.DataManager.DataManager();
            bool b = await dm.PostRecord(perfCounters, srvName);
            if (b)
            {
                responseMessage.StatusCode = HttpStatusCode.Created;
                //string uri = Url.Link("DefaultApi", new { srvName = srvName });
                //responseMessage.Headers.Location = new Uri(uri);
                resultMessage = new System.Web.Http.Results.ResponseMessageResult(responseMessage);
            }
            else
            {
                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                resultMessage = new System.Web.Http.Results.ResponseMessageResult(responseMessage);
            }
            return resultMessage;
        }
    }
}
