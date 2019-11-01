using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using UploaderWebAplication.Models;
using UploaderWebAplication.Properties;

namespace UploaderWebAplication.Controllers.api
{
    public class TransactionController : ApiController
    {
        [HttpPost, Route("api/gettran/bycurrency")]
        public List<Response> GetByCurrency([FromBody]int currencyId)
        {
            List<Response> resp = new List<Response>();
            using (SqlConnection conn = new SqlConnection(Settings.Default.Connection))
            using (SqlCommand comm = new SqlCommand("dbo.GetByCurrency", conn) { CommandType = System.Data.CommandType.StoredProcedure })
            {
                comm.Parameters.Add(
                    new SqlParameter 
                    { 
                        ParameterName = "@CurrencyId",
                        SqlDbType = System.Data.SqlDbType.Int,                       
                        Value = currencyId
                    });
                conn.Open();
                using (SqlDataReader reader = comm.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            resp.Add(new Response
                            {
                                Id = reader[0].ToString(),
                                Payment = $"{reader[1].ToString()} {reader[2].ToString()}",
                                Status = reader[3].ToString()
                            }
                           ) ;
                        }
                }
             }
            return resp;
        }
        
        [HttpPost, Route("api/gettran/bystatus")]
        public List<Response> GetByStatus([FromBody]int status)
        {
            List<Response> resp = new List<Response>();
            using (SqlConnection conn = new SqlConnection(Settings.Default.Connection))
            using (SqlCommand comm = new SqlCommand("dbo.GetByStatus", conn) { CommandType = System.Data.CommandType.StoredProcedure })
            {
                comm.Parameters.Add(
                     new SqlParameter
                     {
                         ParameterName = "@Status",
                         SqlDbType = System.Data.SqlDbType.Int,                       
                         Value = status
                     });
                conn.Open();
                using (SqlDataReader reader = comm.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            resp.Add(new Response
                            {
                                Id = reader[0].ToString(),
                                Payment = $"{reader[1].ToString()} {reader[2].ToString()}",
                                Status = reader[3].ToString()
                            }
                           );
                        }
                }
            }
            return resp;
        }
        [HttpPost, Route("api/gettran/bydate")]
        public List<Response> GetByDate([FromBody] ReqData data)
        {
            List<Response> resp = new List<Response>();
            using (SqlConnection conn = new SqlConnection(Settings.Default.Connection))
            using (SqlCommand comm = new SqlCommand("dbo.GetByDate", conn) { CommandType = System.Data.CommandType.StoredProcedure })
            {
                comm.Parameters.Add(
                     new SqlParameter
                     {
                         ParameterName = "@From",
                         SqlDbType = System.Data.SqlDbType.Date,                       
                         Value = data.From
                     });
                comm.Parameters.Add(
                     new SqlParameter
                     {
                         ParameterName = "@Till",
                         SqlDbType = System.Data.SqlDbType.Date,
                         Value = data.Till
                     });
                conn.Open();
                using (SqlDataReader reader = comm.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            resp.Add(new Response
                            {
                                Id = reader[0].ToString(),
                                Payment = $"{reader[1].ToString()} {reader[2].ToString()}",
                                Status = reader[3].ToString()
                            }
                           );
                        }
                }
            }
            return resp;
        }

        [HttpGet, Route("api/currency/get")]
        public List<Currency> GetCurrencies() 
        {
            List<Currency> resp = new List<Currency>();
            using (SqlConnection conn = new SqlConnection(Settings.Default.Connection))
            using (SqlCommand comm = new SqlCommand("dbo.GetCurrencies", conn) { CommandType = System.Data.CommandType.StoredProcedure })
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
            return resp;
        }
    }
}