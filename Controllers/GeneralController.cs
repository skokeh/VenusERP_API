using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Net.Mail;
using VenusERP_API.Models;
using VenusERP_API.Utils;

namespace VenusERP_API.Controllers
{

    public class GeneralController : BaseApiController
    {

        [HttpGet("Search")]
        
        public ActionResult Search(int SearchID, String? ExciptIds = "", string? Filter = "", bool? loadAll = false, string? AnotherCriteria = "", string? loadLimit = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@SearchID", SearchID));
                if (ExtractReportId() != null)
                    l.Add(new SqlParameter("@ReportId", ExtractReportId()));
                if (ExtractFormId() != null)
                    l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang",ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(loadLimit))
                    l.Add(new SqlParameter("@LoadLimit", loadLimit));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));
                l.Add(new SqlParameter("@LoadAll", loadAll));
                if (!string.IsNullOrEmpty(AnotherCriteria))
                    l.Add(new SqlParameter("@AnotherCriteria", AnotherCriteria));

                var x = DataHelper.ExecScalar("sys_SearchNew", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500,ex.Message);
            }


        }



        [HttpGet("GetAttachmentAsStream")]
        public IActionResult GetAttachmentAsStream(int Id)
        {
            try
            {
                //   int formId = int.Parse(ExtractFormId());
                DataSet mrtDataSet = new();
                mrtDataSet = DataHelper.ExcuteDataSet(@"SELECT Id, FormID, TransactionId, FileName, FileData, Size, type
                                                        FROM            sys_FormAttachments
                                                        WHERE     Id = " + Id);

                System.IO.MemoryStream strm;
                //if (MrtDataSet.Tables[0].Rows != null && MrtDataSet.Tables[0].Rows[0]["PrintReportFile"] != null)
                if (mrtDataSet.Tables[0].Rows.Count > 0)
                {
                    byte[] fileData = (byte[])mrtDataSet.Tables[0].Rows[0]["FileData"];
                    string fileName = mrtDataSet.Tables[0].Rows[0]["FileName"].ToString();
                    string contentType = mrtDataSet.Tables[0].Rows[0]["type"].ToString();
                    return File(fileData, contentType, fileName);
                }
                return NotFound();



            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost("DeleteAttachment")]
        public ActionResult DeleteAttachment(long Id)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@ID", Id));
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);
                DataHelper.ExecScalar("sys_DeleteAttachment", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                return StatusCode(200, true);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetFasicalYear")]
        public ActionResult GetFasicalYear()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                var result = DataHelper.ExecScalar(@"select Top 1 Year(FromDate) From sys_FiscalYearsPeriods order by FromDate Desc", System.Data.CommandType.Text);
                return StatusCode(200, result.ToString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }

        [NonAction]
        public byte[] ImageToByteArray(Bitmap img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        [HttpGet("GetAttachmentTypes")]
        public ActionResult GetAttachmentTypes()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                var x = DataHelper.ExecScalar("Fcs_GetAttachmentTypes", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500,ex.Message);
            }
        }

        [HttpGet("GetAttachmentList")]
        public ActionResult GetAttachmentList(int pageSize, int pageNumber, string? criteria = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@PageSize", pageSize));
                l.Add(new SqlParameter("@PageNumber", pageNumber));
                if (!string.IsNullOrEmpty(criteria))
                    l.Add(new SqlParameter("@Criteria", criteria));
                var x = DataHelper.ExecScalar("Fcs_GetAttachmentList", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        [HttpGet("GetOperationQrBase64")]
        public ActionResult GetOperationQrBase64(int Id)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@OperationID", Id));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                var qrData = DataHelper.ExecScalar("mms_GetOperationQR", System.Data.CommandType.StoredProcedure, l.ToArray());
                return StatusCode(200, qrData);

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //[HttpGet("GetOperationQrAsStream")]
        //public HttpResponseMessage GetOperationQrAsStream(int Id)
        //{
        //    try
        //    {

        //        List<SqlParameter> l = new List<SqlParameter>();
        //        l.Add(new SqlParameter("@User", GetCurrentUser()));
        //        l.Add(new SqlParameter("@OperationID", Id));
        //        l.Add(new SqlParameter("@Lang", ExtractLang()));

        //        var qrData = DataHelper.ExecScalar("mms_GetOperationQR", System.Data.CommandType.StoredProcedure, l.ToArray());
        //        QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
        //        QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(qrData.ToString(), QRCodeGenerator.ECCLevel.H);
        //        Bitmap qrCodeImage = 

        //        MemoryStream stream = new MemoryStream();
        //        qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        //        stream.Position = 0;

        //        return File(stream, "image/png", "qrcode.png");


        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        [HttpGet("GetUserMenu")]
        public ActionResult GetUserMenu()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                var x = DataHelper.ExecScalar("sys_GetMenu", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500,ex.Message);
            }


        }

        // Get Dashboard Data
        [HttpPost("GetDashboardReport")]
        public ActionResult GetDashboardReport(DataObj dataObj)
        {
            try
            {
                try
                {
                    DataHelper.Log(GetAllHeaders());
                    DataHelper.Log(dataObj.dataObj);

                }
                catch (Exception)
                {

                }
                // long? formId = Convert.ToInt64(ExtractFormId());

                JObject oo = JObject.Parse(dataObj.dataObj);
                //var parent = JsonConvert.DeserializeObject<JObject>(dataObj.dataObj);
                var headJson = new JObject();

                JObject newJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }
                newJson.Add("Head", headJson);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                // l.Add(new SqlParameter("@FormId", formId));
                l.Add(new SqlParameter("@Lang",ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("mms_DshB_StkonHandOT", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


                JObject o = JObject.Parse(result.ToString());
                #region //Save Attachments if record has been saved
                //try
                //{
                //    HelperUtilitis.saveAttachments(formId, Convert.ToInt64(o["ID"]), attachments);
                //}
                //catch (Exception ex)
                //{
                //    return StatusCode(500,ex.Message);
                //}
                #endregion


                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500,ex.Message);
            }


        }

        [HttpGet("GetUserProfile")]
        public ActionResult GetUserProfile()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang",ExtractLang()));

                var x = DataHelper.ExecScalar("sys_GetUserProfile", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500,ex.Message);
            }


        }

        [HttpGet("GetFormControls")]
        public ActionResult GetFormControls()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                //l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@RoutePath", ExtractRoutePath()));
                l.Add(new SqlParameter("@Lang",ExtractLang()));

                var x = DataHelper.ExecScalar("sys_GetFormControls", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500,ex.Message);
            }


        }

        [HttpGet("GetFormControlDetails")]
        public ActionResult GetFormControlDetails()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                //l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@RoutePath", ExtractRoutePath()));
                l.Add(new SqlParameter("@Lang",ExtractLang()));

                var x = DataHelper.ExecScalar("sys_GetFormControlDetails", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500,ex.Message);
            }


        }
        [HttpGet("GetDashboardSettings")]
        public ActionResult GetDashboardSettings()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang",ExtractLang()));

                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                DataHelper.ExecScalar("sys_GetDashboardSettings", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
                string result = listOutPutParams[0].Value.ToString();
                if (string.IsNullOrEmpty(result.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(result);
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500,ex.Message);
            }
        }

        [HttpPost("SaveDashboardPositions")]
        public ActionResult SaveDashboardPositions(DataObj dataObj)
        {
            try
            {
                try
                {
                    DataHelper.Log(GetAllHeaders());
                    DataHelper.Log(dataObj.dataObj);

                }
                catch (Exception)
                {

                }
                // long? formId = Convert.ToInt64(ExtractFormId());

                JObject oo = JObject.Parse(dataObj.dataObj);
                //var parent = JsonConvert.DeserializeObject<JObject>(dataObj.dataObj);
                var headJson = new JObject();

                JObject newJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }
                newJson.Add("Head", headJson);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                // l.Add(new SqlParameter("@FormId", formId));
                l.Add(new SqlParameter("@Lang",ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("sys_UserDashboardPositionSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


                JObject o = JObject.Parse(result.ToString());
                #region //Save Attachments if record has been saved
                //try
                //{
                //    HelperUtilitis.saveAttachments(formId, Convert.ToInt64(o["ID"]), attachments);
                //}
                //catch (Exception ex)
                //{
                //    return StatusCode(500,ex.Message);
                //}
                #endregion


                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500,ex.Message);
            }
        }
        [HttpGet("GetFromReportLinks")]
        public ActionResult GetFromReportLinks()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang",ExtractLang()));

                var x = DataHelper.ExecScalar("sys_GetFromReportLinks", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }


        }

        [HttpPost("UpdateAttachments")]
        public ActionResult UpdateAttachments(DataObj dataObj)
        {

            try
            {
                Newtonsoft.Json.Linq.JObject oo = JObject.Parse(dataObj.dataObj);
                var attachments = oo["attachments"].ToArray();
                Newtonsoft.Json.Linq.JObject file;
              
                    List<SqlParameter> listAttachments;
                    int operationHeadId = Convert.ToInt32(oo["ID"]);
                  

                    for (int i = 0; i < attachments.Length; i++)
                    {
                        file = (Newtonsoft.Json.Linq.JObject)attachments[i];
                        if (file["data"] != null)
                        {
                            listAttachments = new List<SqlParameter>();
                            listAttachments.Add(new SqlParameter("@User", ExtractUserId()));
                            listAttachments.Add(new SqlParameter("@FormId", ExtractFormId()));
                            listAttachments.Add(new SqlParameter("@TransactionId", operationHeadId));
                            listAttachments.Add(new SqlParameter("@FileName", file["path"].ToString()));
                            listAttachments.Add(new SqlParameter("@Size", file["size"].ToString()));
                            listAttachments.Add(new SqlParameter("@type", file["type"].ToString()));

                            byte[] imageData = Convert.FromBase64String(file["data"].ToString().Substring(("data:" + file["type"] + ";base64,").Length));
                            listAttachments.Add(new SqlParameter("@AttachedFile", SqlDbType.Binary, imageData.Length) { Value = imageData });
                            bool x = DataHelper.ExecNonQuery("sys_SaveFormAttachments", CommandType.StoredProcedure, listAttachments.ToArray());

                        }


                    
                    
                }
                return StatusCode(200, "Attachments Updated Successfully");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

            [HttpGet("GetContactsAndAddressInfo")]
        public ActionResult GetContactsAndAddressInfo(int? VendorId, int? CustomerId, int? LocationId)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (VendorId.HasValue)
                    l.Add(new SqlParameter("@VendorId", VendorId));
                if (CustomerId.HasValue)
                    l.Add(new SqlParameter("@CustomerId", CustomerId));
                if (LocationId.HasValue)
                    l.Add(new SqlParameter("@LocationId", LocationId));

                var x = DataHelper.ExecScalar("fcs_GetContactsAndAddressInfo", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(500, "");
                }
                JArray o = JArray.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
    }
}
