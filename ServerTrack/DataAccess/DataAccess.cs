using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace ServerTrack.DataAccess
{
    /// <summary>
    /// DataAccess class
    /// </summary>
    public class DataAccess
    {
        private string connectionStr;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccess"/> class.
        /// </summary>
        public DataAccess()
        {
            connectionStr = ConfigurationManager.ConnectionStrings["connectionStr"].ToString();
        }

        /// <summary>
        /// Insert record.
        /// </summary>
        /// <param name="srvName">Name of the Server</param>
        /// <param name="dateTime">DateTime</param>
        /// <param name="cpu">CPU value</param>
        /// <param name="ram">RAM value</param>
        /// <returns>bool</returns>
        public async Task<bool> InsertRecord(string srvName, string dateTime, double cpu, double ram)
        {
            SqlCommand cmd = null;
            bool ret       = false;

            try
            {
                dateTime = string.IsNullOrEmpty(dateTime) ? DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") : dateTime;
                string query = "INSERT INTO dbo.PerfCounters (ComputerName, DateCreated, Cpu_Usage, Ram_Usage) VALUES (@name, @date, @cpu, @ram)";

                using (SqlConnection conn = new SqlConnection(connectionStr))
                using (cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", srvName);
                    cmd.Parameters.AddWithValue("@date", dateTime);
                    cmd.Parameters.AddWithValue("@cpu",  cpu);
                    cmd.Parameters.AddWithValue("@ram",  ram);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    ret = true;

                }
            }
            catch (SqlException sex)
            {
                log.Error(string.Format("DataAccess.InsertRecord. SqlText: {0}.\r\nValues are: srvName = {1}, date = {2}, cpu = {3}, rem = {4}", cmd.CommandText, srvName, dateTime, cpu, ram));
                log.Error(sex);
                ErrorHolder.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                ErrorHolder.Description = sex.Message;
                ret = false;
            }
            catch (Exception ex)
            {
                log.Error(string.Format("DataAccess.InsertRecord. SqlText: {0}.\r\nValues are: srvName = {1}, date = {2}, cpu = {3}, rem = {4}", cmd.CommandText, srvName, dateTime, cpu, ram));
                log.Error(ex);
                ErrorHolder.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                ErrorHolder.Description = ex.Message;
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// Retrieve records.
        /// </summary>
        /// <param name="srvName">Name of the Server</param>
        /// <param name="timeFrame">TimeFrame. Value must be 1 or 24</param>
        /// <returns></returns>
        public async Task<DataTable> GetRecords(string srvName, int timeFrame)
        {
            DateTime  now = DateTime.Now;
            DataTable dt  = null;
            DataRow   dr  = null;

            string from = (timeFrame == 1) ? now.AddMinutes(-60).ToString("yyyy-MM-dd HH:mm:ss") : (timeFrame == 24 ? now.AddHours(-24).ToString("yyyy-MM-dd HH:mm:ss") : null);
            string to = now.ToString("yyyy-MM-dd HH:mm:ss");

            string query1 = string.Format(
                "SELECT DateCreated AS TimeStamp, Cpu_Usage, Ram_Usage FROM dbo.PerfCounters WHERE ComputerName = '{0}' AND DateCreated BETWEEN '{1}' AND '{2}' ORDER BY DateCreated ASC",
                srvName, from, to);

            string query24 = string.Format(@"SELECT dateadd(hour, datediff(hour, 0, DateCreated), 0) AS TimeStamp, AVG(Cpu_Usage) AS Cpu_Usage, AVG(Ram_Usage) AS Ram_Usage
FROM dbo.PerfCounters WHERE ComputerName = '{0}'
GROUP BY dateadd(hour, datediff(hour, 0, DateCreated), 0)
ORDER BY dateadd(hour, datediff(hour, 0, DateCreated), 0)", srvName);

            string query = (timeFrame == 1) ? query1 : query24;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionStr))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    if (reader.HasRows)
                    {
                        dt = new DataTable();
                        dt = new DataTable();

                        dt.Columns.Add("TimeStamp", typeof(DateTime));
                        dt.Columns.Add("Cpu_Usage", typeof(double));
                        dt.Columns.Add("Ram_Usage", typeof(double));
                        while (await reader.ReadAsync())
                        {
                            dr = dt.NewRow();
                            dr["TimeStamp"] = reader["TimeStamp"];
                            dr["Cpu_Usage"] = reader["Cpu_Usage"];
                            dr["Ram_Usage"] = reader["Ram_Usage"];
                            dt.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (SqlException sex)
            {
                log.Error(string.Format("DataAccess.GetRecords. SqlText: '{0}.", query));
                log.Error(sex);
                dt = null;
            }
            catch (Exception ex)
            {
                log.Error(string.Format("DataAccess.GetRecords. SqlText: '{0}.", query));
                log.Error(ex);
                dt = null;
            }

            return dt;
        }
    }
}