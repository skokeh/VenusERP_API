using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;


namespace API.Providers
{
    public class VATEngineProvider
    {
        private  IConfiguration config;
        private static string url = "";
        private static string baseUrl ="";
        public VATEngineProvider(IConfiguration config)
        {
             url = config.GetValue<string>("vatURL");
            baseUrl = config.GetValue<string>("VatBaseUrl");
        }


        public static bool VatCheck(string code, string vendorName, string pDescription, string price, string quantity, string taxPercentage, string taxAmount, string invoiceDate)
        {
            var _url = url;
            var _action = baseUrl;
            //var _url = "https://onesource-idt-det-uat-ws.hostedtax.thomsonreuters.com/sabrix/services/taxcalculationservice/2011-09-01/taxcalculationservice";
            //var _action = "https://onesource-idt-det-uat-ws.hostedtax.thomsonreuters.com/sabrix/services/taxcalculationservice/2011-09-01/taxcalculationservice?wsdl";
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(code, vendorName, pDescription, price, quantity, taxPercentage, invoiceDate);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                XmlDocument xml = new XmlDocument();

                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                    StringReader stringReader = new StringReader(soapResult);
                    xml.LoadXml(soapResult);
                    XmlNodeList getTax = xml.GetElementsByTagName("TOTAL_TAX_AMOUNT");
                    var taxValue = getTax[0].InnerText;
                    if ((Convert.ToDecimal(taxAmount) == Convert.ToDecimal(taxValue))) return true;

                }

            }

            return false;
        }

        private static XmlDocument CreateSoapEnvelope(string code, string vendorName, string pDescription, string price, string quantity, string taxPercentage, string invoiceDate)
        {
            XmlDocument soapEnvelop = new XmlDocument();
            soapEnvelop.LoadXml(
                $@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:wsa=""http://schemas.xmlsoap.org/ws/2004/08/addressing"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
    <soap:Header>
        <wsse:Security soap:mustUnderstand=""1"">
            <wsse:UsernameToken>
                <wsse:Username>^Abdulprod_ws_vs_prod</wsse:Username>
                <wsse:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText""> 4H5ef93K </wsse:Password>
            </wsse:UsernameToken>
        </wsse:Security>
    </soap:Header>
    <soap:Body>
        <taxCalculationRequest xmlns=""http://www.sabrix.com/services/taxcalculationservice/2011-09-01"">
            <INDATA version=""G"">
                <INVOICE>
		<CALLING_SYSTEM_NUMBER>DEV</CALLING_SYSTEM_NUMBER>
		<HOST_SYSTEM>HMS</HOST_SYSTEM>
		<COMPANY_NAME>ALJ Hospital and Rehabiliation_1</COMPANY_NAME>
		<EXTERNAL_COMPANY_ID>ALJ-VENUS-01</EXTERNAL_COMPANY_ID>
		<COMPANY_ROLE>B</COMPANY_ROLE>
		<VENDOR_NAME>{vendorName}</VENDOR_NAME>
		<VENDOR_NUMBER>{code}</VENDOR_NUMBER>
		<INVOICE_NUMBER>{invoiceDate}</INVOICE_NUMBER>
		<INVOICE_DATE>2022-04-04</INVOICE_DATE>
		<CURRENCY_CODE>SAR</CURRENCY_CODE>
		<IS_AUDITED>False</IS_AUDITED>
		<IS_CREDIT>False</IS_CREDIT>
		<CALCULATION_DIRECTION>F</CALCULATION_DIRECTION>
		<POINT_OF_TITLE_TRANSFER>I</POINT_OF_TITLE_TRANSFER>
			 <REGISTRATIONS>
                  <SELLER_ROLE>323456789012345</SELLER_ROLE>
                  </REGISTRATIONS>
    
		<LINE ID=""1"">
            <LINE_NUMBER>1</LINE_NUMBER>
            <DESCRIPTION>{pDescription}</DESCRIPTION>
            <GROSS_AMOUNT>{price}</GROSS_AMOUNT>
            <QUANTITIES>
                <QUANTITY>
                    <AMOUNT>{quantity}</AMOUNT>
                </QUANTITY>
            </QUANTITIES>
            <SHIP_FROM>
                <COUNTRY>SA</COUNTRY>
            </SHIP_FROM>
            <SHIP_TO>
                <COUNTRY>SA</COUNTRY>
            </SHIP_TO>
            <TRANSACTION_TYPE>DS</TRANSACTION_TYPE>
                   <USER_ELEMENT>
                       <NAME>ATTRIBUTE2</NAME>
                       <VALUE>{taxPercentage}</VALUE>
                   </USER_ELEMENT>
                 </LINE>


             </INVOICE>
                     </INDATA>
        </taxCalculationRequest>
    </soap:Body>
</soap:Envelope>"
                );
            return soapEnvelop;
        }


        public static bool InsertInvoiceToVatEngine(string code, string vendorName, JToken[] itemslist, string finalInvoiceDate, string invoiceNo)
        {
            var _url = url;
            var _action = baseUrl;
            //var _url = "https://onesource-idt-det-uat-ws.hostedtax.thomsonreuters.com/sabrix/services/taxcalculationservice/2011-09-01/taxcalculationservice";
            //var _action = "https://onesource-idt-det-uat-ws.hostedtax.thomsonreuters.com/sabrix/services/taxcalculationservice/2011-09-01/taxcalculationservice?wsdl";
            XmlDocument soapEnvelopeXml = CreateSoapEnvelopeForInvoice(code, vendorName, itemslist, finalInvoiceDate, invoiceNo);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                XmlDocument xml = new XmlDocument();

                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                    StringReader stringReader = new StringReader(soapResult);
                    xml.LoadXml(soapResult);
                    XmlNodeList getStatus = xml.GetElementsByTagName("IS_SUCCESS");
                    var status = getStatus[0].InnerText;
                    if ((status == "true")) return true;

                }

            }

            return false;
        }


        private static XmlDocument CreateSoapEnvelopeForInvoice(string code, string vendorName, JToken[] itemslist, string finalInvoiceDate, string invoiceNo)
        {
            string itemslists = "";
            for (int j = 0; j < itemslist.Length; j++)

            {

                itemslists += $@"<LINE ID=""{j}"">
                    <LINE_NUMBER>{j}</LINE_NUMBER>
                <DESCRIPTION>{itemslist[j]["description"]}</DESCRIPTION>
                <GROSS_AMOUNT>{itemslist[j]["price"]}</GROSS_AMOUNT>
            <QUANTITIES>
                    <QUANTITY>
                        <AMOUNT>{itemslist[j]["quantity"]}</AMOUNT>
                   </QUANTITY>
                </QUANTITIES>
               <SHIP_FROM>
                  <COUNTRY>SA</COUNTRY>
               </SHIP_FROM>
               <SHIP_TO>
                   <COUNTRY>SA</COUNTRY>
                </SHIP_TO>
            <TRANSACTION_TYPE>DS</TRANSACTION_TYPE>
                      <USER_ELEMENT>
            <NAME>ATTRIBUTE2</NAME>
                          <VALUE></VALUE>
                     </USER_ELEMENT>
                    </LINE>
                ";

            }
            XmlDocument soapEnvelop = new XmlDocument();
            soapEnvelop.LoadXml(
                $@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:wsa=""http://schemas.xmlsoap.org/ws/2004/08/addressing"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
    <soap:Header>
        <wsse:Security soap:mustUnderstand=""1"">
            <wsse:UsernameToken>
                <wsse:Username>^Abdulprod_ws_vs_prod</wsse:Username>
                <wsse:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText""> 4H5ef93K </wsse:Password>
            </wsse:UsernameToken>
        </wsse:Security>
    </soap:Header>
    <soap:Body>
        <taxCalculationRequest xmlns=""http://www.sabrix.com/services/taxcalculationservice/2011-09-01"">
            <INDATA version=""G"">
                <INVOICE>
		<CALLING_SYSTEM_NUMBER>DEV</CALLING_SYSTEM_NUMBER>
		<HOST_SYSTEM>HMS</HOST_SYSTEM>
		<COMPANY_NAME>ALJ Hospital and Rehabiliation_1</COMPANY_NAME>
		<EXTERNAL_COMPANY_ID>ALJ-VENUS-01</EXTERNAL_COMPANY_ID>
		<COMPANY_ROLE>B</COMPANY_ROLE>
		<VENDOR_NAME>{vendorName}</VENDOR_NAME>
		<VENDOR_NUMBER>{code}</VENDOR_NUMBER>
		<INVOICE_NUMBER>{invoiceNo}</INVOICE_NUMBER>
		<INVOICE_DATE>{finalInvoiceDate}</INVOICE_DATE>
		<CURRENCY_CODE>SAR</CURRENCY_CODE>
		<IS_AUDITED>True</IS_AUDITED>
		<IS_CREDIT>False</IS_CREDIT>
		<CALCULATION_DIRECTION>F</CALCULATION_DIRECTION>
		<POINT_OF_TITLE_TRANSFER>I</POINT_OF_TITLE_TRANSFER>
			 <REGISTRATIONS>
              <SELLER_ROLE>323456789012345</SELLER_ROLE>
              </REGISTRATIONS>
                    {itemslists}
             </INVOICE>
          </INDATA>
        </taxCalculationRequest>
    </soap:Body>
</soap:Envelope>"
                );
            return soapEnvelop;
        }


        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Headers.Add("Authorization", "Basic XkFiZHVsdWF0X3dzOjlramg3eUh0");
            //webRequest.Headers.Add("Content-Type", "application/xml");
            webRequest.Headers.Add("Cookie", "JSESSIONID=A1939749608836360C76EEABAFE72F28");
            webRequest.ContentType = "text/xml; encoding=utf-8";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

   
    }
}