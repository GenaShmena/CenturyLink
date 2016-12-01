using System;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace WebClientConsole
{
    internal class WMIClass
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal bool GetCounters(ref double cpu, ref double ram)
        {
            log4net.Util.LogLog.InternalDebugging = true;
            ManagementObjectSearcher searcher     = null;
            bool ret                              = true;
            long _totalRam                        = 0, _freePhysMem = 0;

            try
            {
                //_totalRam = _totalRam / 0;

                using (searcher = new ManagementObjectSearcher("select PercentProcessorTime from Win32_PerfFormattedData_PerfOS_Processor where Name='_Total'"))
                {
                    using (ManagementObjectCollection moc = searcher.Get())
                    {
                        cpu = double.Parse(moc.Cast<ManagementObject>().FirstOrDefault().GetPropertyValue("PercentProcessorTime").ToString());
                    }

                    searcher = new ManagementObjectSearcher("root\\CIMV2", "Select * from Win32_OPeratingSystem");
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        _totalRam = Convert.ToInt64(mo["TotalVisibleMemorySize"]);
                        _freePhysMem = Convert.ToInt64(mo["FreePhysicalMemory"]);
                    }

                    ram = (double)Decimal.Divide((decimal)_freePhysMem * 100, (decimal)_totalRam);
                    ram = 100.0 - Math.Round(ram, 1);
                }
            }
            catch (ManagementException me)
            {
                log.Error("ManagementException.Test", me);
                ret = false;
            }
            catch (Exception ex)
            {
                log.Error("error", ex);
                ret = false;
            }
            return ret;
        }
    }
}
