using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Web;
using VenusERP_API.Utils;
using SixLabors.ImageSharp;

namespace VenusERP_API.Controllers.Reports
{

    public class ReportsController : BaseApiController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PrintFactory _printFactory;
        public ReportsController(IWebHostEnvironment webHostEnvironment, PrintFactory printFactory)
        {
            _webHostEnvironment = webHostEnvironment;
            _printFactory = printFactory;
        }
        [HttpGet ("GetUserReports")]
        public ActionResult GetUserReports()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                var x = DataHelper.ExecScalar("sys_GetUserReportsNew", System.Data.CommandType.StoredProcedure, l.ToArray());
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }
        [HttpGet("GetReportControls")]
        public ActionResult GetReportControls(int ReportId)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ReportId", ReportId));

                var x = DataHelper.ExecScalar("sys_GetReportControlsNew", System.Data.CommandType.StoredProcedure, l.ToArray());
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpPost("GetSampleReportAsPDF")]
        public HttpResponseMessage GetSampleReportAsPDF(DataObj dataObj)
        {
            //string x = dataObj.dataObj;
            string guid = Guid.NewGuid().ToString();
            string localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, $"Reports/ + {guid}.pdf");
            //   StiReport report = new StiReport();
            DataSet data = new DataSet();
            //  report.Load(@"C:\WORK\NewERP\VenusAPI\API\Reports\SimpleList.mrt");
            //  data.ReadXml(@"C:\WORK\NewERP\VenusAPI\API\Reports\Demo.xml");
            //  report.RegData(data);
            //  report.Render();
            //  report.ExportDocument(StiExportFormat.Pdf, localFilePath);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = guid + ".pdf";
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

            return response;


        }

        [HttpPost("GetReportAsPDF")]
        public HttpResponseMessage GetReportAsPDF(DataObj dataObj)
        {
            try
            {
                DataHelper.Log(dataObj.dataObj);
                List<CriteriaField> listCriteria = new List<CriteriaField>();
                CriteriaField c; JProperty p;
                Newtonsoft.Json.Linq.JObject o = JObject.Parse(dataObj.dataObj);
                string reportID = ((JToken)o["ReportId"]).ToString();
                DateTime d;
                foreach (JToken item in o["Parameters"])
                {
                    p = ((JProperty)item);
                    c = new CriteriaField();
                    c.Name = p.Name;
                    //TODO MODEFIY REPORTS stored procedures TO BY 'yyyy/MM/dd'
                    if (DateTime.TryParse(p.Value.ToString(), out d))
                        c.Value = d.ToString("dd/MM/yyyy");
                    else if (p.Value.ToString().ToLower().Contains("searchfield"))//TODO 1-CHANGE IT TO CODE , 2-ADD TYPE TO PARAMETER
                        c.Value = ((JToken)p.Value)["searchField"].ToString();
                    else
                        c.Value = p.Value.ToString();
                    listCriteria.Add(c);
                }

                

                string lang = ExtractLang();
                string filePath = _printFactory.Print(reportID, listCriteria, GetCurrentUser(), lang.Trim().ToLower() == "ar", "pdf");
                //string filePath = printFactory.Print(reportID, listCriteria, DataHelper.GetUserName());

                string reportName = DateTime.Now.ToString("ddMMMMyy hhmmssfff");

                //var strm = printFactory.PrintStream(reportID, listCriteria, DataHelper.GetUserName());
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = reportName + ".pdf";
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                return response;
            }
            catch (Exception ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent(ex.Message);
                return response;

            }
        }



        [HttpPost("GetReportAsXlsx")]
        public HttpResponseMessage GetReportAsXlsx(DataObj dataObj)
        {
            try
            {
                DataHelper.Log(dataObj.dataObj);
                List<CriteriaField> listCriteria = new List<CriteriaField>();
                CriteriaField c; JProperty p;
                Newtonsoft.Json.Linq.JObject o = JObject.Parse(dataObj.dataObj);
                string reportID = ((JToken)o["ReportId"]).ToString();
                DateTime d;
                foreach (JToken item in o["Parameters"])
                {
                    p = ((JProperty)item);
                    c = new CriteriaField();
                    c.Name = p.Name;
                    //TODO MODEFIY REPORTS stored procedures TO BY 'yyyy/MM/dd'
                    if (DateTime.TryParse(p.Value.ToString(), out d))
                        c.Value = d.ToString("dd/MM/yyyy");
                    else if (p.Value.ToString().ToLower().Contains("searchfield"))//TODO 1-CHANGE IT TO CODE , 2-ADD TYPE TO PARAMETER
                        c.Value = ((JToken)p.Value)["searchField"].ToString();
                    else
                        c.Value = p.Value.ToString();
                    listCriteria.Add(c);
                }

                string lang = ExtractLang();
                string filePath = _printFactory.Print(reportID, listCriteria, GetCurrentUser(), lang.Trim().ToLower() == "ar", "xlsx");
                //string filePath = printFactory.Print(reportID, listCriteria, DataHelper.GetUserName());

                string reportName = DateTime.Now.ToString("ddMMMMyy hhmmssfff");

                //var strm = printFactory.PrintStream(reportID, listCriteria, DataHelper.GetUserName());
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = reportName + ".xlsx";
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.ms-excel");
                return response;
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
                
                return response;

            }
        }

        [HttpPost("GetReportData")]
        public ActionResult GetReportData(DataObj dataObj)
        {
            try
            {
                string userName = GetCurrentUser();
                DataHelper.Log(dataObj.dataObj);
                List<CriteriaField> listCriteria = new List<CriteriaField>();
                CriteriaField c;
                JProperty p;
                Newtonsoft.Json.Linq.JObject o = JObject.Parse(dataObj.dataObj);
                string reportID = ((JToken)o["ReportId"]).ToString();
                string formId = ExtractFormId() != null ? ExtractFormId().ToString() : null;
                DateTime d;
                string sqlAddStiReportsCriteriaData = "";
                if (o["Parameters"] != null && o["Parameters"].HasValues)
                {
                    foreach (JToken item in o["Parameters"])
                    {
                        p = ((JProperty)item);
                        c = new CriteriaField();
                        c.Name = p.Name;
                        sqlAddStiReportsCriteriaData += $@"EXEC [dbo].[sys_AddStiReportsCriteriaData] @ReportID = {reportID},@UserCode = N'{userName}' ,	@FieldName = N'{p.Name}'";
                        if (DateTime.TryParse(p.Value.ToString(), out d))
                        {
                            c.Value = d.ToString("yyyy/MM/dd");
                            sqlAddStiReportsCriteriaData += $",	@NewValue = N'{c.Value}' ;";
                        }
                        else if (p.Value.ToString().ToLower().Contains("searchfield"))
                        {
                            c.Value = ((JToken)p.Value)["searchField"].ToString();
                            sqlAddStiReportsCriteriaData += $",	@NewValue = N'{(JToken)p.Value}' ;";
                        }
                        else
                        {
                            if (p.Value.Type == JTokenType.Boolean)
                            {
                                c.Value = p.Value.ToString().ToLower();
                            }
                            else
                            {
                                c.Value = p.Value.ToString();
                            }
                            sqlAddStiReportsCriteriaData += $",	@NewValue = N'{c.Value}' ;";
                        }
                        listCriteria.Add(c);
                    }

                }

                else
                {
                    sqlAddStiReportsCriteriaData = $@"EXEC [dbo].[sys_AddStiReportsCriteriaData] 
                                                  @ReportID = {reportID}, 
                                                  @UserCode = N'{userName}', @FieldName = NULL, @NewValue = NULL";
                }

                // Execute the SQL query
                try
                {
                    DataHelper.ExecNonQuery(sqlAddStiReportsCriteriaData, CommandType.Text);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }

                string lang = ExtractLang();
                var reportData = _printFactory.GetDataAsJson(reportID, listCriteria, userName, lang.Trim().ToLower() == "ar", formId);
                return StatusCode(200, reportData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
