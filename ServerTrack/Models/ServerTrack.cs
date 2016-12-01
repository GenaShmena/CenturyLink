using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ServerTrack.Models
{
    /// <summary>
    /// ServerTrack class 
    /// </summary>
    [DataContract(Namespace = "")]
    public class ServerTrack
    {
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        [DataMember]
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the counters.
        /// </summary>
        /// <value>
        /// The counters.
        /// </value>
        [DataMember]
        public List<PerfCounters> Counters { get; set; }
    }
}