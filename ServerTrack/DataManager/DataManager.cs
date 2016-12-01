using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ServerTrack.DataAccess;
using ServerTrack.Models;

namespace ServerTrack.DataManager
{
    /// <summary>
    /// DataManager class
    /// </summary>
    public class DataManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DataManager));
        private DataAccess.DataAccess _dataAccess;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataManager"/> class.
        /// </summary>
        public DataManager()
        {
            _dataAccess = new DataAccess.DataAccess();
        }

        /// <summary>
        /// Maps the data table to server track model.
        /// </summary>
        /// <param name="srvName">Name of the SRV.</param>
        /// <param name="timeFrame">The time frame.</param>
        /// <returns>Models.ServerTrack</returns>
        public async Task<ServerTrack.Models.ServerTrack> MapDataTableToServerTrackModel(string srvName, int timeFrame)
        {
            ServerTrack.Models.ServerTrack serviceTrack  = null;
            ServerTrack.Models.PerfCounters perfCounters = null;
            List<PerfCounters> list                      = new List<PerfCounters>();
            DataTable tbl = null;

            tbl = await _dataAccess.GetRecords(srvName, timeFrame);

            if (tbl != null && tbl.Rows.Count > 0)
            {
                try
                {
                    serviceTrack = new Models.ServerTrack();
                    serviceTrack.Counters = new List<PerfCounters>();
                    serviceTrack.Server = srvName;
                    foreach (DataRow r in tbl.Rows)
                    {
                        perfCounters = new PerfCounters();
                        perfCounters.TimeStamp = ((DateTime)r.ItemArray.GetValue(0)).ToString("yyyy-MM-ddTHH:mm:ss");
                        perfCounters.Cpu_Usage = (double)r.ItemArray.GetValue(1);
                        perfCounters.Ram_Usage = (double)r.ItemArray.GetValue(2);
                        serviceTrack.Counters.Add(perfCounters);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("DataManager.MapDataTableToServerTrackModel", ex);
                    ErrorHolder.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                    ErrorHolder.Description = ex.Message;
                    serviceTrack = null;
                }
            }
            else
            {
                log.Error("DataManager.MapDataTableToServerTrackModel. Call 'await _dataAccess.GetRecords(srvName, timeFrame)' returned null or empty table");
                ErrorHolder.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                ErrorHolder.Description = "DataManager.MapDataTableToServerTrackModel.Call 'await _dataAccess.GetRecords(srvName, timeFrame)' returned null or empty table";
                serviceTrack = null;
            }

            return serviceTrack;
        }

        /// <summary>
        /// Posts the record.
        /// </summary>
        /// <param name="perfCounters">The perf counters.</param>
        /// <param name="srvName">Name of the SRV.</param>
        /// <returns>bool</returns>
        public async Task<bool> PostRecord(PerfCounters perfCounters, string srvName)
        {
            perfCounters.TimeStamp = string.IsNullOrEmpty(perfCounters.TimeStamp) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : perfCounters.TimeStamp;
            bool b = await _dataAccess.InsertRecord(srvName, perfCounters.TimeStamp, perfCounters.Cpu_Usage, perfCounters.Ram_Usage);
            return b;
        }
    }
}