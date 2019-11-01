using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace UploaderWebAplication.Models
{
    public class Transaction
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("TransactionDate")]
        public string Date { get; set; }

        [XmlElement("PaymentDetails")]
        public Payment PaymentInfo { get; set; }

        [XmlElement("Status")]
        public Status Status { get; set; }
    }
}