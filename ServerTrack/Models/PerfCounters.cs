using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ServerTrack.Models
{
    /// <summary>
    /// PerfCounters class
    /// </summary>
    [DataContract(Namespace = "")]
    public class PerfCounters
    {
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        [DataMember]
        [DisplayFormat(DataFormatString = "{yyyy-MM-ddThh:mm:ss}")]
        [DataType(DataType.Date, ErrorMessage = "Invalid TimeStamp Format(null or 'yyyy-MM-ddThh:mm:ss'")]
        [RegularExpression
            (@"^([\+-]?\d{4}(?!\d{2}\b))((-?)((0[1-9]|1[0-2])(\3([12]\d|0[1-9]|3[01]))?|W([0-4]\d|5[0-2])(-?[1-7])?|(00[1-9]|0[1-9]\d|[12]\d{2}|3([0-5]\d|6[1-6])))([T\s]((([01]\d|2[0-3])((:?)[0-5]\d)?|24\:?00)([\.,]\d+(?!:))?)?(\17[0-5]\d([\.,]\d+)?)?([zZ]|([\+-])([01]\d|2[0-3]):?([0-5]\d)?)?)?)?$|b",
            ErrorMessage = "Invalid TimeStamp Format(null or 'yyyy-MM-ddThh:mm:ss'")]
        public string TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the cpu usage.
        /// </summary>
        /// <value>
        /// The cpu usage.
        /// </value>
        [DataMember]
        [Required]
        public double Cpu_Usage { get; set; }

        /// <summary>
        /// Gets or sets the ram usage.
        /// </summary>
        /// <value>
        /// The ram usage.
        /// </value>
        [DataMember]
        [Required]
        public double Ram_Usage { get; set; }
    }
}