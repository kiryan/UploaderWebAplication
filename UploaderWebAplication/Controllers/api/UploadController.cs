using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml.Serialization;
using UploaderWebAplication.Models;
using UploaderWebAplication.Properties;

namespace UploaderWebAplication.Controllers.api
{
    public class UploadController : ApiController
    {
      
        string UploadPath = HttpContext.Current.Server.MapPath("~/upload");
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        public HttpResponseMessage Post()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            List<string> errorMesages = new List<string>();
             var httpRequest = HttpContext.Current.Request;
             if (httpRequest.Files.Count == 1)
             {
                 HttpPostedFile file = httpRequest.Files[0];
                 List<string> supportedTypes = GetSupportedTypes();
                 if (!supportedTypes.Any())
                    return new HttpRequestMessage().CreateResponse(HttpStatusCode.InternalServerError);
               
                 if (!supportedTypes.Where(c=> c == file.ContentType).Any()) 
                 {
                    return new HttpRequestMessage().CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "Unknown format");
                }

                if (file.ContentLength > 1000000)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, "Too large files. File must be less then 1 MB");
                   
                }
                FileInfo fl = new FileInfo(file.FileName);
                if (file.ContentType != "application/vnd.ms-excel" && file.ContentType != "text/xml")
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid format");
                  
                }
                if (fl.Extension != ".csv" && fl.Extension != ".xml")
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid format");
                   
                }
                var filePath = Path.Combine(UploadPath, file.FileName);
                 file.SaveAs(filePath);
                if (file.ContentType == "application/vnd.ms-excel")
                {
                    try
                    {
                        LoadCsv(file);
                    }
                    catch (Exception e)
                    {
                        return request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error when loading CSV");
                       
                    }

                }
                else
                {
                    try
                    {
                        LoadXml(file);
                    }
                    catch (Exception e)
                    {
                        return request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error when loading XML");
                       
                    }
                }               
             }
             else
             {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, "Too many files");
              
             }

             return request.CreateResponse(HttpStatusCode.OK);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }     
     
        private List<string> GetSupportedTypes()
        {
            List<string> contentTypes = new List<string>();
            try
            {
                RegistryKey classesRoot = Registry.ClassesRoot;
                string[] subKeys = classesRoot.GetSubKeyNames();
               
                foreach (string sk in subKeys)
                {
                    RegistryKey k = classesRoot.OpenSubKey(sk);
                    object contentType = k.GetValue("Content Type");
                    if (contentType != null)
                    {
                        string ct = contentType.ToString();
                        if (!contentTypes.Where(c => c == ct).Any())
                            contentTypes.Add(ct);
                    }
                }                
            }
            catch 
            {
                contentTypes = new List<string>();
            }
            return contentTypes;
        }
    
        private void LoadCsv(HttpPostedFile file) 
        {
            string[] lines = File.ReadAllLines(Path.Combine(UploadPath, file.FileName));
            if (lines.Count() == 0) 
                throw new Exception("Empty data"); 
            var table = new DataTable();
            table.Columns.Add("Tid", typeof(string));
            table.Columns.Add("Amount",typeof(decimal));
            table.Columns.Add("CurrencyCode", typeof(string));
            table.Columns.Add("TDate", typeof(DateTime));
            table.Columns.Add("StatusId",typeof(int));
            for (int i = 0; i < lines.Count(); i++)
            {
                string[] rows = lines[i].Split(';');
                if (string.IsNullOrWhiteSpace(rows[0])
                    || string.IsNullOrWhiteSpace(rows[1])
                    || string.IsNullOrWhiteSpace(rows[2])
                    || string.IsNullOrWhiteSpace(rows[3])
                    || string.IsNullOrWhiteSpace(rows[4]))
                {
                    throw new Exception("Incorrect data");                  
                }
                DateTime date = DateTime.ParseExact(rows[3], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                DataRow row = table.NewRow();
                row["Tid"] = rows[0];
                row["Amount"] = decimal.Parse(rows[1].Replace(",","").Replace(".",","));
                row["CurrencyCode"] = GetCurrencyId(rows[2].ToUpper()); 
                row["TDate"] = date;
                row["StatusId"] = GetStatusType(rows[4].ToUpper(), "csv");              
              
                table.Rows.Add(row);
            }
            using (SqlConnection conn = new SqlConnection(Settings.Default.Connection))
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn))
            {
                sqlBulkCopy.DestinationTableName = "Transactions";
                conn.Open();
                sqlBulkCopy.ColumnMappings.Add("Tid", "Tid");
                sqlBulkCopy.ColumnMappings.Add("Amount", "Amount");
                sqlBulkCopy.ColumnMappings.Add("CurrencyCode", "CurrencyCode");
                sqlBulkCopy.ColumnMappings.Add("TDate", "TDate");
                sqlBulkCopy.ColumnMappings.Add("StatusId", "StatusId");
                sqlBulkCopy.WriteToServer(table);
            }
        }

        private void LoadXml(HttpPostedFile file) 
        {
            string xml = File.ReadAllText(Path.Combine(UploadPath, file.FileName));
            TransactionData transactionData = FromXml<TransactionData>(xml);
            if(transactionData == null || !transactionData.Transactions.Any())
                throw new Exception("Empty data");
            var table = new DataTable();
            table.Columns.Add("Tid", typeof(string));
            table.Columns.Add("Amount", typeof(decimal));
            table.Columns.Add("CurrencyCode", typeof(string));
            table.Columns.Add("TDate", typeof(DateTime));
            table.Columns.Add("StatusId", typeof(int));
            foreach (Transaction tran in transactionData.Transactions)
            {
                if (string.IsNullOrWhiteSpace(tran.Date)
                   || string.IsNullOrWhiteSpace(tran.Id)                   
                   || string.IsNullOrWhiteSpace(tran.PaymentInfo.Amount)
                   || string.IsNullOrWhiteSpace(tran.PaymentInfo.CurrencyCode))
                {
                    throw new Exception("Incorrect data");
                }
                string dt = tran.Date.Replace("T", " ");
                DateTime date = DateTime.ParseExact(dt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                DataRow row = table.NewRow();
                row["Tid"] = tran.Id;
                row["Amount"] = decimal.Parse(tran.PaymentInfo.Amount.Replace(",", "").Replace(".", ","));
                row["CurrencyCode"] = GetCurrencyId(tran.PaymentInfo.CurrencyCode);
                row["TDate"] = date;
                row["StatusId"] = (int)tran.Status;

                table.Rows.Add(row);
            }
            using (SqlConnection conn = new SqlConnection(Settings.Default.Connection))
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn))
            {
                sqlBulkCopy.DestinationTableName = "Transactions";
                conn.Open();
                sqlBulkCopy.ColumnMappings.Add("Tid", "Tid");
                sqlBulkCopy.ColumnMappings.Add("Amount", "Amount");
                sqlBulkCopy.ColumnMappings.Add("CurrencyCode", "CurrencyCode");
                sqlBulkCopy.ColumnMappings.Add("TDate", "TDate");
                sqlBulkCopy.ColumnMappings.Add("StatusId", "StatusId");
                sqlBulkCopy.WriteToServer(table);
            }
        }

        private int GetStatusType(string status, string fileType)
        {
            using (SqlConnection conn = new SqlConnection(Settings.Default.Connection))
            using (SqlCommand comm = new SqlCommand("dbo.GetStatuses", conn) { CommandType = CommandType.StoredProcedure })
            {
                comm.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@StatusText",
                    SqlDbType = SqlDbType.VarChar,                    
                    Value = status
                });
                comm.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@FileType",
                    SqlDbType = SqlDbType.Char,
                    Size = 3,
                    Value = fileType
                });
                conn.Open();
                var reader = comm.ExecuteScalar();
                if (reader != null)
                {
                    return int.Parse(reader.ToString());
                }
            }
            return 0;
        }
        
        private int GetCurrencyId(string currency)
        {
            List<Currency> resp = new List<Currency>();
            using (SqlConnection conn = new SqlConnection(Settings.Default.Connection))
            using (SqlCommand comm = new SqlCommand("dbo.GetCurrencies", conn) { CommandType = CommandType.StoredProcedure })
            {
               
                conn.Open();
                using (SqlDataReader reader = comm.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            resp.Add(
                                new Currency
                                {
                                    Id = int.Parse(reader["Id"].ToString()),
                                    Code = reader["CurrencyCode"].ToString()
                                });
                        }
                }
            }
            return resp.FirstOrDefault(r=>r.Code.ToUpper() == currency.ToUpper() ).Id;
        }
        private static T FromXml<T>(string xml) where T : new()
        {
            if (string.IsNullOrEmpty(xml))
                return new T();


            var serializer = new XmlSerializer(typeof(T));

            using (var stringReader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(stringReader);
            }
        }
    }
}