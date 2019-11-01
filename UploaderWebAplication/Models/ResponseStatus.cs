using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace UploaderWebAplication.Models
{
    public class ResponseStatus
    {
        /// <summary>
        /// Response Status Code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Error messages
        /// </summary>
        public List<string> ErrorMessages { get; set; }

        public ResponseStatus()
        {
            ErrorMessages = new List<string>();
        }
    }
}