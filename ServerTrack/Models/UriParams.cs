using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ServerTrack.Models
{
    /// <summary>
    /// UriGetParams
    /// </summary>
    [DataContract(Namespace = "")]
    public class UriParams
    {
        /// <summary>
        /// Gets or sets the name of the SRV.
        /// </summary>
        /// <value>
        /// The name of the SRV.
        /// </value>
        [DataMember]
        [Required]
        [RegularExpression(@"(?i)(?=.{5,20}$)^(([a-z\d]|[a-z\d][a-z\d\-]*[a-z\d])\.)*([a-z\d]|[a-z\d][a-z\d\-]*[a-z\d])$", ErrorMessage = "Invalid Server Name.")]
        public string SrvName { get; set; }

        /// <summary>
        /// Gets or sets the time interval.
        /// </summary>
        /// <value>
        /// The time interval.
        /// </value>
        [DataMember]
        [Required]
        [RegularExpression("[1]|[2][4]", ErrorMessage = "Invalid Time Interval value. Value must be 1 or 24 hour(s)")]
        public int TimeInterval { get; set; }
    }
}