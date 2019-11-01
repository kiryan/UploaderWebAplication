using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace UploaderWebAplication.Models
{
    [DataContract]
    public class Response
    {
        [DataMember(Name = "id", Order = 1)]
        public string Id { get; set; }

        [DataMember(Name = "payment", Order = 2)]
        public string Payment { get; set; }

        [DataMember(Name = "status", Order = 3)]
        public string Status { get; set; }
    }
}