using System.Net;
using VenusERP_API.Utils;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json;
using RestSharp;

using static QRCoder.PayloadGenerator.ShadowSocksConfig;


namespace VenusERP_API.Controllers.Operations
{
    public class RSD
    {
        private IConfiguration _configuration;
        public RSD(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        private string GetOperationProducts(int operationHeadId, out string AuthHeader, out string ToGln)
        {
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(new SqlParameter("@OperationHeadId", operationHeadId));
            List<SqlParameter> listOutPutParams = new List<SqlParameter>();
            listOutPutParams.Add(new SqlParameter() { ParameterName = "@productsData", Size = -1, SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output });
            listOutPutParams.Add(new SqlParameter() { ParameterName = "@RsdUser", Size = -1, SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output });
            listOutPutParams.Add(new SqlParameter() { ParameterName = "@RsdPassword", Size = -1, SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output });
            listOutPutParams.Add(new SqlParameter() { ParameterName = "@ToGln", Size = -1, SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output });

            DataHelper.ExecScalar("rsd_getOperationProducts", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
            string RsdUser = listOutPutParams[1].Value.ToString();
            string RsdPassword = listOutPutParams[2].Value.ToString();
            //string RsdUser = "68200316000010000";
            //sring RsdPassword = "Ahmc@2323666";
            ToGln = listOutPutParams[3].Value.ToString();


            var encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            byte[] headerBytes = encoding.GetBytes(RsdUser + ":" + RsdPassword);
            AuthHeader = "Basic " + Convert.ToBase64String(headerBytes);

            return listOutPutParams[0].Value.ToString();
        }
        private string GetOperationProducts(int operationHeadId, out string AuthHeader)
        {
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(new SqlParameter("@OperationHeadId", operationHeadId));
            List<SqlParameter> listOutPutParams = new List<SqlParameter>();
            listOutPutParams.Add(new SqlParameter() { ParameterName = "@productsData", Size = -1, SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output });
            listOutPutParams.Add(new SqlParameter() { ParameterName = "@RsdUser", Size = -1, SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output });
            listOutPutParams.Add(new SqlParameter() { ParameterName = "@RsdPassword", Size = -1, SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output });
            listOutPutParams.Add(new SqlParameter() { ParameterName = "@ToGln", Size = -1, SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output });

            DataHelper.ExecScalar("rsd_getOperationProducts", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
            string RsdUser = listOutPutParams[1].Value.ToString();
            string RsdPassword = listOutPutParams[2].Value.ToString();


            var encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            byte[] headerBytes = encoding.GetBytes(RsdUser + ":" + RsdPassword);
            AuthHeader = "Basic " + Convert.ToBase64String(headerBytes);

            return listOutPutParams[0].Value.ToString();
        }


        string getRSD_Data(int operationHeadId, out string RequestBaseURL)
        {
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(new SqlParameter("@OperationHeadId", operationHeadId));
            List<SqlParameter> listOutPutParams = new List<SqlParameter>();
            listOutPutParams.Add(new SqlParameter() { ParameterName = "@RequestBaseURL", Size = -1, SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output });
            listOutPutParams.Add(new SqlParameter() { ParameterName = "@bodyData", Size = -1, SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output });

            DataHelper.ExecScalar("rsd_GetRequestBody", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
            RequestBaseURL = listOutPutParams[0].Value.ToString();
            string body = listOutPutParams[1].Value.ToString();
            return body;
        }

        void LogRequest(string userCode, string RequestBaseURL, string RequestBody, string ResponseStatus, string ErrorMessage
            , string ResponseContent)
        {
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(new SqlParameter("@RequestBaseURL", RequestBaseURL.Replace("'", "''")) { Size = -1 });
            l.Add(new SqlParameter("@RequestBody", RequestBody.Replace("'", "''")) { Size = -1 });
            l.Add(new SqlParameter("@ResponseStatus", ResponseStatus.ToString().Replace("'", "''")) { Size = -1 });
            l.Add(new SqlParameter("@ErrorMessage", ErrorMessage) { Size = -1 });
            l.Add(new SqlParameter("@ResponseContent", ResponseContent.Replace("'", "''")) { Size = -1 });
            l.Add(new SqlParameter("@RegUserCode", userCode.ToString()) { Size = -1 });
            List<SqlParameter> listOutPutParams = new List<SqlParameter>();

            DataHelper.ExecScalar("rsd_RSDLogSave", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
        }

        void LogLeenRequest(string userCode, string RequestBaseURL, string RequestBody, string ResponseStatus, string ErrorMessage
               , string ResponseContent, string PRODUCTLIST, string NOTIFICATIONID, string OperationHeadID)
        {
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(new SqlParameter("@RequestBaseURL", RequestBaseURL.Replace("'", "''")) { Size = -1 });
            l.Add(new SqlParameter("@RequestBody", RequestBody.Replace("'", "''")) { Size = -1 });
            l.Add(new SqlParameter("@ResponseStatus", ResponseStatus.ToString().Replace("'", "''")) { Size = -1 });
            l.Add(new SqlParameter("@ErrorMessage", ErrorMessage) { Size = -1 });
            l.Add(new SqlParameter("@ResponseContent", ResponseContent.Replace("'", "''")) { Size = -1 });
            l.Add(new SqlParameter("@RegUserCode", userCode.ToString()));
            l.Add(new SqlParameter("@Response_PRODUCTLIST", PRODUCTLIST.Replace("'", "''")) { Size = -1 });
            l.Add(new SqlParameter("@Response_NOTIFICATIONID", NOTIFICATIONID));
            l.Add(new SqlParameter("@OperationHeadID", OperationHeadID));

            List<SqlParameter> listOutPutParams = new List<SqlParameter>();

            DataHelper.ExecScalar("rsd_RSDLogSave", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
        }

        public void post(int operationHeadId, string userCode)
        {

            string RequestBaseURL = "";
            string body = getRSD_Data(operationHeadId, out RequestBaseURL);

            if (!string.IsNullOrEmpty(RequestBaseURL) && !string.IsNullOrEmpty(body))
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                //var client = new RestClient("https://api.juleb.com/rasd/inventory_accept");
                var client = new RestClient(RequestBaseURL);
                // client.Timeout = -1;
                var request = new RestRequest(RequestBaseURL,RestSharp.Method.Post);
                request.AddHeader("Content-Type", "application/json");
                /*
                var body = @"{" + "\n" +
                //@"    ""branch_user"": ""68200220000010000""," + "\n" + // pharmacy
                @"    ""branch_user"": ""68200220000020000""," + "\n" + // hospital
                @"    ""branch_pass"": ""f8afa222faea22bd760ffc224cdbba45""," + "\n" +
                @"    ""product_list"": [{""gti_number"":""07612797425442"",""serial_number"":""960656684899"",""batch_number"":""22D01GA"",""expiry_date"":""2024-03-31""},{""gti_number"":""07612797425442"",""serial_number"":""970246248902"",""batch_number"":""22D01GA"",""expiry_date"":""2024-03-31""},{""gti_number"":""07612797425442"",""serial_number"":""917173952525"",""batch_number"":""22D01GA"",""expiry_date"":""2024-03-31""}]
                }";*/

                request.AddParameter("application/json", body, ParameterType.RequestBody);
                var response = client.Execute(request);

                LogRequest(userCode, RequestBaseURL,
                        body,
                        response.ResponseStatus.ToString(),
                        response.ErrorMessage,
                        response.Content
                        );
            }

        }

        public string getDispatchInfo_juleb(string notificationId, int locationId, string userCode)
        {

            DataTable d = DataHelper.ExcuteDataTable("SELECT RsdUser,RsdPassword FROM sys_Locations WHERE ID=" + locationId, System.Data.CommandType.Text, null);
            if (d.Rows.Count == 0)
            {
                return null;
            }
            string branch_user = d.Rows[0]["RsdUser"].ToString();
            string branch_pass = d.Rows[0]["RsdPassword"].ToString();
            string RequestBaseURL = "https://api.juleb.com/rasd/dispatch_info";
            string body = @"{ ""branch_user"": """ + branch_user + @""",
""branch_pass"": """ + branch_pass + @""",
""notification"": """ + notificationId + @"""
 }";

            if (!string.IsNullOrEmpty(RequestBaseURL) && !string.IsNullOrEmpty(body))
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                //var client = new RestClient("https://api.juleb.com/rasd/inventory_accept");
                var client = new RestClient(RequestBaseURL);
                // client.Timeout = -1;
                var request = new RestRequest(RequestBaseURL, RestSharp.Method.Post);
                request.AddHeader("Content-Type", "application/json");

                request.AddParameter("application/json", body, ParameterType.RequestBody);
                var response = client.Execute(request);

                LogRequest(userCode, RequestBaseURL,
                        body,
                        response.ResponseStatus.ToString(),
                        response.ErrorMessage,
                        response.Content
                        );
                if (response.ResponseStatus == ResponseStatus.Completed && string.IsNullOrEmpty(response.ErrorMessage))
                {
                    return response.Content;
                }
            }
            return null;

        }



        public void InvAccept(int operationHeadID, string userCode)
        {
            string LeenUrl = _configuration.GetSection("Leen")["LeenUrl"];
            string RequestURL = LeenUrl + "/AcceptService/AcceptService";
            string authHeader = "";
            string productAsXml = GetOperationProducts(operationHeadID, out authHeader);


            var options = new RestClientOptions("http://tandtwstest.sfda.gov.sa:8080/ws/AcceptService/AcceptService")
            {
                ThrowOnAnyError = true,
                MaxTimeout = -1
            };

            var client = new RestClient(RequestURL);
          
            var request = new RestRequest(RequestURL, RestSharp.Method.Post);
            request.AddHeader("Authorization", authHeader);
            request.AddHeader("Content-Type", "text/xml;charset=UTF-8");
            var body = @"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:acc='http://dtts.sfda.gov.sa/AcceptService'>
   <soapenv:Header/>
   <soapenv:Body>
      <acc:AcceptServiceRequest>
        <PRODUCTLIST>" +
        productAsXml
          + @"</PRODUCTLIST>
    </acc:AcceptServiceRequest>
   </soapenv:Body>
</soapenv:Envelope>";
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            var response = client.Execute(request);
            string strNotificationID = "";
            string strProductList = "";
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                XDocument doc = XDocument.Parse(response.Content);
                XNamespace ns = "http://dtts.sfda.gov.sa/AcceptService";
                IEnumerable<XElement> responses = doc.Descendants(ns + "AcceptServiceResponse");

                foreach (XElement responseXml in responses)
                {
                    strNotificationID = (string)responseXml.Element("NOTIFICATIONID");
                    strProductList = responseXml.Element("PRODUCTLIST").ToString();
                    //DataHelper.Log(strNotificationID);
                    //DataHelper.Log(strProductList);

                }
            }
            else
            {

            }
           


            LogLeenRequest(userCode, RequestURL,
           body,
           response.ResponseStatus.ToString(),
           response.ErrorMessage,
           response.Content,
           strProductList,
           strNotificationID,
           operationHeadID.ToString()
           );


        }

        public void PharmacySale(int operationHeadID, string userCode)
        {
            string Authorization = "";
            string productAsXml = GetOperationProducts(operationHeadID, out Authorization);

            var options = new RestClientOptions("http://tandtwstest.sfda.gov.sa:8080/ws/PharmacySaleService/PharmacySaleService")
            {
                ThrowOnAnyError = true,
                MaxTimeout = -1
            };

            var client = new RestClient(options);

            var request = new RestRequest("", RestSharp.Method.Post);
            request.AddHeader("Content-Type", "text/xml;charset=UTF-8");
            //request.AddHeader("Authorization", "Basic NjgyMzA3NTEwMDAwMTAwMDA6RG8xMjM0NTZA");
            request.AddHeader("Authorization", Authorization);

            var body = $@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:phar='http://dtts.sfda.gov.sa/PharmacySaleService'>
   <soapenv:Header/>
   <soapenv:Body>
      <phar:PharmacySaleServiceRequest>
         <TOGLN>0000000000000</TOGLN>
         <!--Optional:-->
         <DOCTORID></DOCTORID>
         <!--Optional:-->
         <PATIENTNATIONALID></PATIENTNATIONALID>
         <PRESCRIPTIONID></PRESCRIPTIONID>
         <PRESCRIPTIONDATE>{DateTime.Now.ToString("yyyy-MM-dd")}</PRESCRIPTIONDATE>
        <PRODUCTLIST>" +
        productAsXml
            //   <!--1 or more repetitions:-->
            //   <PRODUCT>
            //    <GTIN>06102030405091</GTIN>
            //      <SN>QQ6Q33883</SN>
            //      <!--Optional:-->
            //      <BN>AAA222</BN>
            //      <!--Optional:-->
            //      <XD>2025-01-01</XD>
            //   </PRODUCT>
        + @"</PRODUCTLIST>
    </phar:PharmacySaleServiceRequest>
   </soapenv:Body>
</soapenv:Envelope>";
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
            DataHelper.Log(response.Content);
            //return response.Content;


            //string responseContent = InvAccept(productAsXml);

            XDocument doc = XDocument.Parse(response.Content);
            XNamespace ns = "http://dtts.sfda.gov.sa/PharmacySaleService";
            IEnumerable<XElement> responses = doc.Descendants(ns + "PharmacySaleServiceResponse");
            string strNotificationID = "";
            string strProductList = "";
            foreach (XElement responseXml in responses)
            {
                strNotificationID = (string)responseXml.Element("NOTIFICATIONID");
                strProductList = responseXml.Element("PRODUCTLIST").ToString();
                DataHelper.Log(strNotificationID);
                DataHelper.Log(strProductList);

            }


            LogLeenRequest(userCode, "http://tandtwstest.sfda.gov.sa:8080/ws/AcceptService/AcceptService",
           body,
           response.ResponseStatus.ToString(),
           response.ErrorMessage,
           response.Content,
           strProductList,
           strNotificationID,
           operationHeadID.ToString()
           );


        }

        public void PharmacySaleCancel(int operationHeadID, string userCode)
        {
            string Authorization = "";
            string productAsXml = GetOperationProducts(operationHeadID, out Authorization);

            var options = new RestClientOptions("http://tandtwstest.sfda.gov.sa:8080/ws/PharmacySaleCancelService/PharmacySaleCancelService")
            {
                ThrowOnAnyError = true,
                MaxTimeout = -1
            };

            var client = new RestClient(options);

            var request = new RestRequest("", RestSharp.Method.Post);
            request.AddHeader("Content-Type", "text/xml;charset=UTF-8");
            //request.AddHeader("Authorization", "Basic NjgyMzA3NTEwMDAwMTAwMDA6RG8xMjM0NTZA");
            request.AddHeader("Authorization", Authorization);

            var body = $@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:phar='http://dtts.sfda.gov.sa/PharmacySaleCancelService'>
   <soapenv:Header/>
   <soapenv:Body>
      <phar:PharmacySaleCancelServiceRequest>
         <TOGLN>0000000000000</TOGLN>
         <PRESCRIPTIONID>?</PRESCRIPTIONID>
        <PRODUCTLIST>" +
        productAsXml
            //   <!--1 or more repetitions:-->
            //   <PRODUCT>
            //    <GTIN>06102030405091</GTIN>
            //      <SN>QQ6Q33883</SN>
            //      <!--Optional:-->
            //      <BN>AAA222</BN>
            //      <!--Optional:-->
            //      <XD>2025-01-01</XD>
            //   </PRODUCT>
        + @"</PRODUCTLIST>
    </phar:PharmacySaleCancelServiceRequest>
   </soapenv:Body>
</soapenv:Envelope>";
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
            DataHelper.Log(response.Content);
            //return response.Content;


            //string responseContent = InvAccept(productAsXml);

            XDocument doc = XDocument.Parse(response.Content);
            XNamespace ns = "http://dtts.sfda.gov.sa/PharmacySaleCancelService";
            IEnumerable<XElement> responses = doc.Descendants(ns + "PharmacySaleCancelServiceResponse");
            string strNotificationID = "";
            string strProductList = "";
            foreach (XElement responseXml in responses)
            {
                strNotificationID = (string)responseXml.Element("NOTIFICATIONID");
                strProductList = responseXml.Element("PRODUCTLIST").ToString();
                DataHelper.Log(strNotificationID);
                DataHelper.Log(strProductList);

            }


            LogLeenRequest(userCode, "http://tandtwstest.sfda.gov.sa:8080/ws/AcceptService/AcceptService",
           body,
           response.ResponseStatus.ToString(),
           response.ErrorMessage,
           response.Content,
           strProductList,
           strNotificationID,
           operationHeadID.ToString()
           );


        }

        public void Deactivat(int operationHeadID, string userCode, string DeactivationReasonCode)
        {
            string url = "http://tandtwstest.sfda.gov.sa:8080/ws/DeactivationService/DeactivationService";
            string Authorization = "";
            string productAsXml = GetOperationProducts(operationHeadID, out Authorization);


            var options = new RestClientOptions(url)
            {
                ThrowOnAnyError = true,
                MaxTimeout = -1
            };

            var client = new RestClient(options);

            var request = new RestRequest(url, RestSharp.Method.Post);
           
            request.AddHeader("Content-Type", "text/xml;charset=UTF-8");
            //request.AddHeader("Authorization", "Basic NjgyMzA3NTEwMDAwMTAwMDA6RG8xMjM0NTZA");
            request.AddHeader("Authorization", Authorization);

            var body = $@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:deac='http://dtts.sfda.gov.sa/DeactivationService'>
   <soapenv:Header/>
   <soapenv:Body>
      <deac:DeactivationServiceRequest>
      <!-- -->
      <DR>{DeactivationReasonCode}</DR>
      <EXPLANATION>
            {DeactivationReasonCode}</EXPLANATION>
        <PRODUCTLIST>" +
        productAsXml

        + @" </PRODUCTLIST>
</deac:DeactivationServiceRequest>
      
   </soapenv:Body>
</soapenv:Envelope>";
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
            DataHelper.Log(response.Content);
            //return response.Content;


            //string responseContent = InvAccept(productAsXml);

            XDocument doc = XDocument.Parse(response.Content);
            XNamespace ns = "http://dtts.sfda.gov.sa/DeactivationService";
            IEnumerable<XElement> responses = doc.Descendants(ns + "DeactivationServiceResponse");
            string strNotificationID = "";
            string strProductList = "";
            foreach (XElement responseXml in responses)
            {
                strNotificationID = (string)responseXml.Element("NOTIFICATIONID");
                strProductList = responseXml.Element("PRODUCTLIST").ToString();
                DataHelper.Log(strNotificationID);
                DataHelper.Log(strProductList);

            }


            LogLeenRequest(userCode, "http://tandtwstest.sfda.gov.sa:8080/ws/AcceptService/AcceptService",
           body,
           response.ResponseStatus.ToString(),
           response.ErrorMessage,
           response.Content,
           strProductList,
           strNotificationID,
           operationHeadID.ToString()
           );


        }

        public void DeactivatCancel(int operationHeadID, string userCode)
        {
            string url = "http://tandtwstest.sfda.gov.sa:8080/ws/DeactivationCancelService/DeactivationCancelService";
            string Authorization = "";
            string productAsXml = GetOperationProducts(operationHeadID, out Authorization);



            var options = new RestClientOptions(url)
            {
                ThrowOnAnyError = true,
                MaxTimeout = -1
            };

            var client = new RestClient(options);

            var request = new RestRequest(url, RestSharp.Method.Post);
            request.AddHeader("Content-Type", "text/xml;charset=UTF-8");
            //request.AddHeader("Authorization", "Basic NjgyMzA3NTEwMDAwMTAwMDA6RG8xMjM0NTZA");
            request.AddHeader("Authorization", Authorization);

            var body = $@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:deac='http://dtts.sfda.gov.sa/DeactivationCancelService'>
   <soapenv:Header/>
   <soapenv:Body>
      <deac:DeactivationCancelServiceRequest>
        <PRODUCTLIST>" +
        productAsXml

        + @"</PRODUCTLIST>
</deac:DeactivationCancelServiceRequest>
   </soapenv:Body>
</soapenv:Envelope>";
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
            DataHelper.Log(response.Content);
            //return response.Content;


            //string responseContent = InvAccept(productAsXml);

            XDocument doc = XDocument.Parse(response.Content);
            XNamespace ns = "http://dtts.sfda.gov.sa/DeactivationCancelService";
            IEnumerable<XElement> responses = doc.Descendants(ns + "DeactivationCancelServiceResponse");
            string strNotificationID = "";
            string strProductList = "";
            foreach (XElement responseXml in responses)
            {
                strNotificationID = (string)responseXml.Element("NOTIFICATIONID");
                strProductList = responseXml.Element("PRODUCTLIST").ToString();
                DataHelper.Log(strNotificationID);
                DataHelper.Log(strProductList);

            }


            LogLeenRequest(userCode, "http://tandtwstest.sfda.gov.sa:8080/ws/AcceptService/AcceptService",
           body,
           response.ResponseStatus.ToString(),
           response.ErrorMessage,
           response.Content,
           strProductList,
           strNotificationID,
           operationHeadID.ToString()
           );


        }
        public void Transfer(int operationHeadID, string userCode)
        {
            string Authorization = "";
            string ToGln = "";
            string authHeader = "";
            string productAsXml = GetOperationProducts(operationHeadID, out authHeader, out ToGln);



            string url = "http://tandtwstest.sfda.gov.sa:8080/ws/TransferService/TransferService";
            var options = new RestClientOptions(url)
            {
                ThrowOnAnyError = true,
                MaxTimeout = -1
            };

            var client = new RestClient(options);

            var request = new RestRequest(url, RestSharp.Method.Post);
            request.AddHeader("Content-Type", "text/xml;charset=UTF-8");
            //request.AddHeader("Authorization", "Basic NjgyMzA3NTEwMDAwMTAwMDA6RG8xMjM0NTZA");
            request.AddHeader("Authorization", Authorization);

            var body = $@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tran='http://dtts.sfda.gov.sa/TransferService'>
   <soapenv:Header/>
   <soapenv:Body>
      <tran:TransferServiceRequest>
         <TOGLN>{ToGln}</TOGLN>
        <PRODUCTLIST>" +
        productAsXml

        + @"</PRODUCTLIST>
</tran:TransferServiceRequest>
   </soapenv:Body>
</soapenv:Envelope>";
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
            DataHelper.Log(response.Content);
            //return response.Content;


            //string responseContent = InvAccept(productAsXml);

            XDocument doc = XDocument.Parse(response.Content);
            XNamespace ns = "http://dtts.sfda.gov.sa/TransferService";
            IEnumerable<XElement> responses = doc.Descendants(ns + "TransferServiceResponse");
            string strNotificationID = "";
            string strProductList = "";
            foreach (XElement responseXml in responses)
            {
                strNotificationID = (string)responseXml.Element("NOTIFICATIONID");
                strProductList = responseXml.Element("PRODUCTLIST").ToString();
                DataHelper.Log(strNotificationID);
                DataHelper.Log(strProductList);

            }


            LogLeenRequest(userCode, "http://tandtwstest.sfda.gov.sa:8080/ws/AcceptService/AcceptService",
           body,
           response.ResponseStatus.ToString(),
           response.ErrorMessage,
           response.Content,
           strProductList,
           strNotificationID,
           operationHeadID.ToString()
           );


        }

        public void TransferCancel(int operationHeadID, string userCode)
        {
            var url = "http://tandtwstest.sfda.gov.sa:8080/ws/TransferCancelService/TransferCancelService";
            string Authorization = "";
            string ToGln = "";
            string productAsXml = GetOperationProducts(operationHeadID, out Authorization, out ToGln);
          
            var options = new RestClientOptions(url)
            {
                ThrowOnAnyError = true,
                MaxTimeout = -1
            };

            var client = new RestClient(options);

            var request = new RestRequest(url, RestSharp.Method.Post);
            request.AddHeader("Content-Type", "text/xml;charset=UTF-8");
            //request.AddHeader("Authorization", "Basic NjgyMzA3NTEwMDAwMTAwMDA6RG8xMjM0NTZA");
            request.AddHeader("Authorization", Authorization);

            var body = $@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tran='http://dtts.sfda.gov.sa/TransferCancelService'>
   <soapenv:Header/>
   <soapenv:Body>
      <tran:TransferCancelServiceRequest>
         <PRODUCTLIST>" +
        productAsXml

        + @"</PRODUCTLIST>
      </tran:TransferCancelServiceRequest>
   </soapenv:Body>
</soapenv:Envelope>

";
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
            DataHelper.Log(response.Content);
            //return response.Content;


            //string responseContent = InvAccept(productAsXml);

            XDocument doc = XDocument.Parse(response.Content);
            XNamespace ns = "http://dtts.sfda.gov.sa/TransferCancelService";
            IEnumerable<XElement> responses = doc.Descendants(ns + "TransferCancelServiceResponse");
            string strNotificationID = "";
            string strProductList = "";
            foreach (XElement responseXml in responses)
            {
                strNotificationID = (string)responseXml.Element("NOTIFICATIONID");
                strProductList = responseXml.Element("PRODUCTLIST").ToString();
                DataHelper.Log(strNotificationID);
                DataHelper.Log(strProductList);

            }


            LogLeenRequest(userCode, "http://tandtwstest.sfda.gov.sa:8080/ws/AcceptService/AcceptService",
           body,
           response.ResponseStatus.ToString(),
           response.ErrorMessage,
           response.Content,
           strProductList,
           strNotificationID,
           operationHeadID.ToString()
           );


        }

     

        public void Return(int operationHeadID, string userCode)
        {
            string url = "http://tandtwstest.sfda.gov.sa:8080/ws/ReturnService/ReturnService";
            string Authorization = "";
            string ToGln = "";
            string productAsXml = GetOperationProducts(operationHeadID, out Authorization, out ToGln);



          
            var options = new RestClientOptions(url)
            {
                ThrowOnAnyError = true,
                MaxTimeout = -1
            };

            var client = new RestClient(options);

            var request = new RestRequest(url, RestSharp.Method.Post);
            request.AddHeader("Content-Type", "text/xml;charset=UTF-8");
            //request.AddHeader("Authorization", "Basic NjgyMzA3NTEwMDAwMTAwMDA6RG8xMjM0NTZA");
            request.AddHeader("Authorization", Authorization);

            var body = $@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:ret='http://dtts.sfda.gov.sa/ReturnService'>
   <soapenv:Header/>
   <soapenv:Body>
      <ret:ReturnServiceRequest>
         <TOGLN>6823075100003</TOGLN>
        <PRODUCTLIST>" +
        productAsXml

        + @"</PRODUCTLIST>
</ret:ReturnServiceRequest>
   </soapenv:Body>
</soapenv:Envelope>";
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
            DataHelper.Log(response.Content);
            //return response.Content;


            //string responseContent = InvAccept(productAsXml);

            XDocument doc = XDocument.Parse(response.Content);
            XNamespace ns = "http://dtts.sfda.gov.sa/ReturnService";
            IEnumerable<XElement> responses = doc.Descendants(ns + "ReturnServiceResponse");
            string strNotificationID = "";
            string strProductList = "";
            foreach (XElement responseXml in responses)
            {
                strNotificationID = (string)responseXml.Element("NOTIFICATIONID");
                strProductList = responseXml.Element("PRODUCTLIST").ToString();
                DataHelper.Log(strNotificationID);
                DataHelper.Log(strProductList);

            }


            LogLeenRequest(userCode, "http://tandtwstest.sfda.gov.sa:8080/ws/AcceptService/AcceptService",
           body,
           response.ResponseStatus.ToString(),
           response.ErrorMessage,
           response.Content,
           strProductList,
           strNotificationID,
           operationHeadID.ToString()
           );


        }

        public void Consume(int operationHeadID, string userCode)
        {
            string Authorization = "";
            string ToGln = "";
            string productAsXml = GetOperationProducts(operationHeadID, out Authorization, out ToGln);



            var url = "http://tandtwstest.sfda.gov.sa:8080/ws/ConsumeService/ConsumeService";
            var options = new RestClientOptions(url)
            {
                ThrowOnAnyError = true,
                MaxTimeout = -1
            };

            var client = new RestClient(options);

            var request = new RestRequest(url, RestSharp.Method.Post);
            request.AddHeader("Content-Type", "text/xml;charset=UTF-8");
            //request.AddHeader("Authorization", "Basic NjgyMzA3NTEwMDAwMTAwMDA6RG8xMjM0NTZA");
            request.AddHeader("Authorization", Authorization);

            var body = $@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:con='http://dtts.sfda.gov.sa/ConsumeService'>
   <soapenv:Header/>
   <soapenv:Body>
      <con:ConsumeServiceRequest>
         <TOGLN>0000000000000</TOGLN>
         <!--Optional:-->
         <DOCTORID></DOCTORID>
         <!--Optional:-->
         <PATIENTNATIONALID></PATIENTNATIONALID>
         <PRESCRIPTIONID></PRESCRIPTIONID>
         <PRESCRIPTIONDATE>{DateTime.Now.ToString("yyyy-MM-dd")}</PRESCRIPTIONDATE>
        <PRODUCTLIST>" +
        productAsXml
            //   <!--1 or more repetitions:-->
            //   <PRODUCT>
            //    <GTIN>06102030405091</GTIN>
            //      <SN>QQ6Q33883</SN>
            //      <!--Optional:-->
            //      <BN>AAA222</BN>
            //      <!--Optional:-->
            //      <XD>2025-01-01</XD>
            //   </PRODUCT>
        + @"</PRODUCTLIST>
     </con:ConsumeServiceRequest>
   </soapenv:Body>
</soapenv:Envelope>";
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
            DataHelper.Log(response.Content);
            //return response.Content;


            //string responseContent = InvAccept(productAsXml);

            XDocument doc = XDocument.Parse(response.Content);
            XNamespace ns = "http://dtts.sfda.gov.sa/ConsumeService";
            IEnumerable<XElement> responses = doc.Descendants(ns + "ConsumeServiceResponse");
            string strNotificationID = "";
            string strProductList = "";
            foreach (XElement responseXml in responses)
            {
                strNotificationID = (string)responseXml.Element("NOTIFICATIONID");
                strProductList = responseXml.Element("PRODUCTLIST").ToString();
                DataHelper.Log(strNotificationID);
                DataHelper.Log(strProductList);

            }


            LogLeenRequest(userCode, "http://tandtwstest.sfda.gov.sa:8080/ws/AcceptService/AcceptService",
           body,
           response.ResponseStatus.ToString(),
           response.ErrorMessage,
           response.Content,
           strProductList,
           strNotificationID,
           operationHeadID.ToString()
           );


        }

        public void ConsumeCancel(int operationHeadID, string userCode)
        {
            string Authorization = "";
            string ToGln = "";
            string productAsXml = GetOperationProducts(operationHeadID, out Authorization, out ToGln);



            var url ="http://tandtwstest.sfda.gov.sa:8080/ws/ConsumeCancelService/ConsumeCancelService";
            var options = new RestClientOptions(url)
            {
                ThrowOnAnyError = true,
                MaxTimeout = -1
            };

            var client = new RestClient(options);

            var request = new RestRequest(url, RestSharp.Method.Post); ;
            request.AddHeader("Content-Type", "text/xml;charset=UTF-8");
            //request.AddHeader("Authorization", "Basic NjgyMzA3NTEwMDAwMTAwMDA6RG8xMjM0NTZA");
            request.AddHeader("Authorization", Authorization);

            var body = $@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:con='http://dtts.sfda.gov.sa/ConsumeCancelService'>
   <soapenv:Header/>
   <soapenv:Body>
      <con:ConsumeCancelServiceRequest>
         <PRODUCTLIST>" +
        productAsXml
            //   <!--1 or more repetitions:-->
            //   <PRODUCT>
            //    <GTIN>06102030405091</GTIN>
            //      <SN>QQ6Q33883</SN>
            //      <!--Optional:-->
            //      <BN>AAA222</BN>
            //      <!--Optional:-->
            //      <XD>2025-01-01</XD>
            //   </PRODUCT>
        + @"</PRODUCTLIST>
     </con:ConsumeCancelServiceRequest>
   </soapenv:Body>
</soapenv:Envelope>";
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
            DataHelper.Log(response.Content);
            //return response.Content;


            //string responseContent = InvAccept(productAsXml);

            XDocument doc = XDocument.Parse(response.Content);
            XNamespace ns = "http://dtts.sfda.gov.sa/ConsumeCancelService";
            IEnumerable<XElement> responses = doc.Descendants(ns + "ConsumeCancelServiceResponse");
            string strNotificationID = "";
            string strProductList = "";
            foreach (XElement responseXml in responses)
            {
                strNotificationID = (string)responseXml.Element("NOTIFICATIONID");
                strProductList = responseXml.Element("PRODUCTLIST").ToString();
                DataHelper.Log(strNotificationID);
                DataHelper.Log(strProductList);

            }


            LogLeenRequest(userCode, "http://tandtwstest.sfda.gov.sa:8080/ws/ConsumeCancelService/ConsumeCancelService",
           body,
           response.ResponseStatus.ToString(),
           response.ErrorMessage,
           response.Content,
           strProductList,
           strNotificationID,
           operationHeadID.ToString()
           );


        }

        private string GetLeenLocationAuth(int locationId, out string AuthHeader)
        {
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(new SqlParameter("@locationId", locationId));
            List<SqlParameter> listOutPutParams = new List<SqlParameter>();
            listOutPutParams.Add(new SqlParameter() { ParameterName = "@RsdUser", Size = -1, SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output });
            listOutPutParams.Add(new SqlParameter() { ParameterName = "@RsdPassword", Size = -1, SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output });

            DataHelper.ExecScalar("rsd_GetLeenLocationAuth", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
            string RsdUser = listOutPutParams[0].Value.ToString();
            string RsdPassword = listOutPutParams[1].Value.ToString();
            //string RsdUser = "68200316000010000";
            //string RsdPassword = "Ahmc@2323666";


            var encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            byte[] headerBytes = encoding.GetBytes(RsdUser + ":" + RsdPassword);
            AuthHeader = "Basic " + Convert.ToBase64String(headerBytes);

            return listOutPutParams[0].Value.ToString();
        }

        //TODO CONVERT TO public STRING
        //AuthCode
        public string PTSQuery(int locationId, string userCode, DateTime fromDate, DateTime toDate, string tOGLN, string fromGLN = "")
        {
            string authHeader;
            //string productAsXml = GetOperationProducts(operationHeadID, out authHeader);
            string productAsXml = GetLeenLocationAuth(locationId, out authHeader);
            string LeenUrl = _configuration.GetSection("Leen")["LeenUrl"];

            var RequestURL = LeenUrl + "/PackageQueryService/PackageQueryService";
            var options = new RestClientOptions(RequestURL)
            {
                ThrowOnAnyError = true,
                MaxTimeout = -1
            };

            var client = new RestClient(options);

            var request = new RestRequest(RequestURL, RestSharp.Method.Post);
            request.AddHeader("Content-Type", "text/xml;charset=UTF-8");
            //request.AddHeader("Authorization", "Basic NjgyMzA3NTEwMDAwMTAwMDA6RG8xMjM0NTZA");
            request.AddHeader("Authorization", authHeader);
            //request.AddHeader("Authorization", Authorization);

            var body = $@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:pac='http://dtts.sfda.gov.sa/PackageQueryService'>
   <soapenv:Header/>
   <soapenv:Body>
      <pac:PackageQueryServiceRequest>
       <!--Optional: FROM Or TO-->
         <FROMGLN>
            {fromGLN}</FROMGLN>
         <TOGLN>{tOGLN}</TOGLN>
         <GETALL>true</GETALL>
         <STARTDATE>{fromDate.ToString("yyyy-MM-dd")}</STARTDATE>
         <ENDDATE>{toDate.ToString("yyyy-MM-dd")}</ENDDATE>
      </pac:PackageQueryServiceRequest>
   </soapenv:Body>
</soapenv:Envelope>";
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
            DataHelper.Log(response.Content);
            //return response.Content;


            //string responseContent = InvAccept(productAsXml);

            XDocument doc = XDocument.Parse(response.Content);
            XNamespace ns = "http://dtts.sfda.gov.sa/PackageQueryService";
            IEnumerable<XElement> responses = doc.Descendants(ns + "PackageQueryServiceResponse");
            string strNotificationID = "";
            string strProductList = "";

            //foreach (XElement responseXml in responses)
            //{
            //    strProductList = responseXml.Element("TRANSFERDETAIL").ToString();
            //    DataHelper.Log(strNotificationID);
            //    DataHelper.Log(strProductList);

            //}

            string xml = response.Content;

            XmlDocument doc22 = new XmlDocument();
            doc22.LoadXml(xml);
            doc22.GetElementsByTagName("TRANSFERDETAIL");
            string json = "";
            if (doc22.GetElementsByTagName("TRANSFERDETAILLIST").Count > 0 && doc22.GetElementsByTagName("TRANSFERDETAILLIST")[0].HasChildNodes)
            {
                json = JsonConvert.SerializeXmlNode(doc22.GetElementsByTagName("TRANSFERDETAILLIST")[0]);
            }

           // LogLeenRequest(userCode,RequestURL,
           //body,
           //response.ResponseStatus.ToString(),
           //response.ErrorMessage,
           //response.Content,
           //strProductList,
           //strNotificationID,
           //operationHeadID.ToString()
           //);
            return json;

        }

        //TODO  GetAttachmentAsStream")]
        //AuthCode
//        public System.Net.Http.HttpResponseMessage PTSDownLoad(long transferId, int locationId)
//        {
//            string authHeader;
//            string productAsXml = GetLeenLocationAuth(locationId, out authHeader);
//            string LeenUrl = _configuration.GetSection("Leen")["LeenUrl"];
//            //TODO GET AUTH FROM CURRENT LOCATION
//           // throw new Exception("ERROR : GET AUTH FROM CURRENT LOCATION");



//            var request = new RestRequest("", RestSharp.Method.Post);
//            request.AddHeader("Content-Type", "text/xml;charset=UTF-8");
//            //request.AddHeader("Authorization", "Basic NjgyMzA3NTEwMDAwMTAwMDA6RG8xMjM0NTZA");
//            request.AddHeader("Authorization", "Basic NjgyMzA3NTEwMDAwMTAwMDA6RG8xMjM0NTZA");
//            //request.AddHeader("Authorization", Authorization);

//            var body = $@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:pac='http://dtts.sfda.gov.sa/PackageDownloadService'>
//   <soapenv:Header/>
//   <soapenv:Body>
//      <pac:PackageDownloadServiceRequest>
//         <TRANSFERID>{transferId}</TRANSFERID>
//      </pac:PackageDownloadServiceRequest>
//   </soapenv:Body>
//</soapenv:Envelope>";
//            request.AddParameter("text/xml", body, ParameterType.RequestBody);

//            var client = new RestClient(LeenUrl + "/PackageDownloadService/PackageDownloadService");

 

//            var response = client.Execute(request);
//            if (response.StatusCode == System.Net.HttpStatusCode.OK)
//            {

//            }
//            //return response.Content;
//            DataHelper.Log("AAAAA" + response.Content);


//            string fileAsString64 = "";
//            try
//            {
//                //XmlDocument xDoc = new XmlDocument();
//                //xDoc.LoadXml(response.Content);
//                //XmlNodeList name = xDoc.GetElementsByTagName("MD5CHECKSUM");

//                XDocument doc = XDocument.Parse(response.Content);
//                XNamespace ns = "http://dtts.sfda.gov.sa/PackageDownloadService";
//                IEnumerable<XElement> responses = doc.Descendants(ns + "PackageDownloadServiceResponse");
//                foreach (XElement responseXml in responses)
//                {
//                    fileAsString64 = (string)responseXml.Element("FILE");
//                    DataHelper.Log("QQQQ" + fileAsString64);

//                }
//            }
//            catch (Exception ex)
//            {
//                DataHelper.Log("ex" + ex.Message);

//            }



//            //System.IO.MemoryStream strm;
//            //strm = new System.IO.MemoryStream(response.RawBytes);
//            //string fileName = "testPtsDownload";
//            //string type = "application/x-zip-compressed";

//            System.IO.MemoryStream strm;
//            //if (MrtDataSet.Tables[0].Rows != null && MrtDataSet.Tables[0].Rows[0]["PrintReportFile"] != null)
//            //{
//            byte[] rByte = Convert.FromBase64String(fileAsString64);
//            strm = new System.IO.MemoryStream(rByte);
//            string fileName = "testPtsDownload";
//            string type = "application/x-zip-compressed";



//            System.Net.Http.HttpResponseMessage res = new System.Net.Http.HttpResponseMessage(HttpStatusCode.OK);
//            res.Content = new System.Net.Http.StreamContent(strm);
//            res.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
//            res.Content.Headers.ContentDisposition.FileName = fileName;
//            res.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(type);

//            return res;

//        }

        public System.Net.Http.HttpResponseMessage PTSDownLoad(long transferId, int locationId)
        {
            //TODO GET AUTH FROM CURRENT LOCATION
            //throw new Exception("ERROR : GET AUTH FROM CURRENT LOCATION");
            string authHeader = "";
            string LeenUrl = _configuration.GetSection("Leen")["LeenUrl"];
            string productAsXml = GetLeenLocationAuth(locationId, out authHeader);
            string RequestURL = LeenUrl + "/PackageQueryService/PackageQueryService";
            var request = new RestRequest(RequestURL, RestSharp.Method.Post);
            request.AddHeader("Content-Type", "text/xml;charset=UTF-8");
            request.AddHeader("Authorization", authHeader);


            //request.AddHeader("Authorization", "Basic " + encodedheader);

            var body = $@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:pac='http://dtts.sfda.gov.sa/PackageDownloadService'>
   <soapenv:Header/>
   <soapenv:Body>
      <pac:PackageDownloadServiceRequest>
         <TRANSFERID>{transferId}</TRANSFERID>
      </pac:PackageDownloadServiceRequest>
   </soapenv:Body>
</soapenv:Envelope>";
            request.AddParameter("text/xml", body, ParameterType.RequestBody);

            //var client = new RestClient("http://tandtwstest.sfda.gov.sa:8080/ws/PackageDownloadService/PackageDownloadService");
            var client = new RestClient(LeenUrl + "/PackageDownloadService/PackageDownloadService");

            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
            //return response.Content;
            DataHelper.Log("AAAAA" + response.Content);


            string fileAsString64 = "";
            try
            {
                //XmlDocument xDoc = new XmlDocument();
                //xDoc.LoadXml(response.Content);
                //XmlNodeList name = xDoc.GetElementsByTagName("MD5CHECKSUM");

                XDocument doc = XDocument.Parse(response.Content);
                XNamespace ns = "http://dtts.sfda.gov.sa/PackageDownloadService";
                IEnumerable<XElement> responses = doc.Descendants(ns + "PackageDownloadServiceResponse");
                foreach (XElement responseXml in responses)
                {
                    fileAsString64 = (string)responseXml.Element("FILE");
                    DataHelper.Log("QQQQ" + fileAsString64);

                }
            }
            catch (Exception ex)
            {
                DataHelper.Log("ex" + ex.Message);

            }



            //System.IO.MemoryStream strm;
            //strm = new System.IO.MemoryStream(response.RawBytes);
            //string fileName = "testPtsDownload";
            //string type = "application/x-zip-compressed";

            System.IO.MemoryStream strm;
            //if (MrtDataSet.Tables[0].Rows != null && MrtDataSet.Tables[0].Rows[0]["PrintReportFile"] != null)
            //{
            byte[] rByte = Convert.FromBase64String(fileAsString64);
            strm = new System.IO.MemoryStream(rByte);
            string fileName = "testPtsDownload";
            string type = "application/x-zip-compressed";




            System.Net.Http.HttpResponseMessage res = new System.Net.Http.HttpResponseMessage(HttpStatusCode.OK);
            res.Content = new System.Net.Http.StreamContent(strm);
            res.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            res.Content.Headers.ContentDisposition.FileName = fileName;
            res.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(type);

            return res;

        }


    }
}