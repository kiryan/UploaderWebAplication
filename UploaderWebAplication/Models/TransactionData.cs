using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace UploaderWebAplication.Models
{
    [XmlRoot("Transactions")]
    public class TransactionData
    {
        [XmlElement("Transaction")]
        public List<Transaction> Transactions { get; set; }
    }
}