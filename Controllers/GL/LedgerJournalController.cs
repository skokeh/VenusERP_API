using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using VenusERP_API.Utils;


namespace VenusERP_API.Controllers.GL
{
    public class LedgerJournalController : BaseApiController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LedgerJournalController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet("GetList")]
        public ActionResult GetList(int pageSize, int pageNumber, string? orderBy = "", string? orderDirection = "", string? criteria = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@RoutePath", ExtractRoutePath()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@PageSize", pageSize));
                l.Add(new SqlParameter("@PageNumber", pageNumber));
                if (!string.IsNullOrEmpty(orderBy))
                    l.Add(new SqlParameter("@OrderBy", orderBy));
                if (!string.IsNullOrEmpty(orderDirection))
                    l.Add(new SqlParameter("@OrderDirection", orderDirection));
                if (!string.IsNullOrEmpty(criteria))
                    l.Add(new SqlParameter("@Criteria", criteria));

                var x = DataHelper.ExecScalar("fcs_LedgerJourGetList", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetByID")]
        public ActionResult GetByID(int ID)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ID", ID));
                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });

                DataHelper.ExecScalar("fcs_LedgerJourDetails", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
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

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetNextCode")]
        public ActionResult GetNextCode(int TransTypeID, string JournalDate)
        {
            try
            {
                //List<SqlParameter> l = new List<SqlParameter>();
                //var result = DataHelper.ExecScalar($@"select ISNULL(max(JournalNumber),0)+1 from fcs_LedgerJour LJ
                //                                        INNER JOIN fcs_TransTypes TT ON TT.ID=LJ.TransTypeID
                //                                        where TransTypeID={TransTypeID} 
                //                                        AND (TT.ISYear=0  OR YEAR(JournalDate) ={DateTime.Parse(JournalDate).Year})
                //                                        AND (TT.ISMonth=0 OR MONTH(JournalDate)={DateTime.Parse(JournalDate).Month})"
                //                    , System.Data.CommandType.Text);
                //return StatusCode(200, result.ToString());
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@TransDate", JournalDate));
                l.Add(new SqlParameter("@TransTypeID", TransTypeID));
                l.Add(new SqlParameter("@TableName", "fcs_LedgerJour"));
                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@OLastSeq", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output });

                DataHelper.ExecScalar("sys_GetFormLastSequence", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
                string lastSeq = listOutPutParams[0].Value.ToString();
                return StatusCode(200, lastSeq);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }

        [HttpPost("SaveReaucaranceJVLedgerJournal")]
        public ActionResult SaveRecurrenceJVLedgerJournal(DataObj dataObj)
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

                JObject oo = JObject.Parse(dataObj.dataObj);
                JObject newJson = new JObject();
                newJson.Add("Head", oo);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("fcs_RecurrenceJVLedgerJournalSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                var error = listOutParameters.Find(X => X.ParameterName == "@Error").Value;
                if (error != null && !string.IsNullOrEmpty(error.ToString()))
                {
                    DataHelper.Log(error.ToString());
                    return StatusCode(400, error);
                }

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


                JObject o = JObject.Parse(result);
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                DataHelper.Log(ex.Message);
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("getTransTypes")]
        public ActionResult getTransTypes()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));


                var x = DataHelper.ExecScalar("fcs_GetGlTransTypes", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("getFormSpecialLedgers")]
        public ActionResult getFormSpecialLedgers(String?ExciptIds = "", string?Filter = "", int?LedgerId = null, string?AnotherCriteria = "", int? VendorId=null, int? CustomerId=null)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));
                if (LedgerId != null)
                    l.Add(new SqlParameter("@LedgerId", LedgerId));
                if (!string.IsNullOrEmpty(AnotherCriteria))
                    l.Add(new SqlParameter("@AnotherCriteria", AnotherCriteria));
                if(VendorId != null)
                    l.Add(new SqlParameter("@VendorId", VendorId));
                if (CustomerId != null)
                    l.Add(new SqlParameter("@CustomerId", CustomerId));


                var x = DataHelper.ExecScalar("fcs_GetFormSpecialLedgers", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        
        
        }

        [HttpGet("srchLedgers")]
        public ActionResult srchLedgers(string? ExciptIds = "", string? Filter = "", int? LedgerId = null, string? AnotherCriteria = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));
                if (LedgerId != null)
                    l.Add(new SqlParameter("@LedgerId", LedgerId));
                if (!string.IsNullOrEmpty(AnotherCriteria))
                    l.Add(new SqlParameter("@AnotherCriteria", AnotherCriteria));

                var x = DataHelper.ExecScalar("fcs_srchLedgers", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("srchCostCenters1")]
        public ActionResult srchCostCenters1(int ledgerId, string? ExciptIds = "", string? Filter = "", int? VendorId = null, int? CustomerId = null)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@LedgerId", ledgerId));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));
                if (VendorId != null)
                    l.Add(new SqlParameter("@VendorId", VendorId));
                if (CustomerId != null)
                    l.Add(new SqlParameter("@CustomerId", CustomerId));

                var x = DataHelper.ExecScalar("fcs_srchCostCenters1", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("srchCostCenters2")]
        public ActionResult srchCostCenters2(int ledgerId, int costCenter1Id, string? ExciptIds = "", string? Filter = "", int? VendorId = null, int? CustomerId = null)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@LedgerId", ledgerId));
                l.Add(new SqlParameter("@CC1Id", costCenter1Id));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));
                if (VendorId != null)
                    l.Add(new SqlParameter("@VendorId", VendorId));
                if (CustomerId != null)
                    l.Add(new SqlParameter("@CustomerId", CustomerId));


                var x = DataHelper.ExecScalar("fcs_srchCostCenters2", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }
        [HttpGet("srchCostCenters3")]
        public ActionResult srchCostCenters3(int ledgerId, int costCenter1Id, int costCenter2Id, string? ExciptIds = "", string? Filter = "", int? VendorId = null, int? CustomerId = null)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@LedgerId", ledgerId));
                l.Add(new SqlParameter("@CC1Id", costCenter1Id));
                l.Add(new SqlParameter("@CC2Id", costCenter2Id));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));
                if (VendorId != null)
                    l.Add(new SqlParameter("@VendorId", VendorId));
                if (CustomerId != null)
                    l.Add(new SqlParameter("@CustomerId", CustomerId));
                var x = DataHelper.ExecScalar("fcs_srchCostCenters3", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("srchCostCenters4")]
        public ActionResult srchCostCenters4(int ledgerId, int costCenter1Id, int costCenter2Id, int costCenter3Id, string? ExciptIds = "", string? Filter = "", int? VendorId = null, int? CustomerId = null)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@LedgerId", ledgerId));
                l.Add(new SqlParameter("@CC1Id", costCenter1Id));
                l.Add(new SqlParameter("@CC2Id", costCenter2Id));
                l.Add(new SqlParameter("@CC3Id", costCenter3Id));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));
                if (VendorId != null)
                    l.Add(new SqlParameter("@VendorId", VendorId));
                if (CustomerId != null)
                    l.Add(new SqlParameter("@CustomerId", CustomerId));

                var x = DataHelper.ExecScalar("fcs_srchCostCenters4", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("srchBanks")]
        public ActionResult srchBanks(string? ExciptIds = "", string? Filter = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));

                var x = DataHelper.ExecScalar("fcs_srchBanks", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }
        [HttpPost("BulkStatusApproval")]
        public ActionResult BulkStatusApproval(DataObj dataObj)
        {
            try
            {
                try
                {
                    DataHelper.Log(GetAllHeaders());
                    DataHelper.Log(dataObj.dataObj);
                }
                catch(Exception ex)
                {

                }
                JObject oo = JObject.Parse(dataObj.dataObj);
                JObject newJson = new JObject();
                newJson.Add("Head", oo);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("fcs_BulkLedgerJournelApproval", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


                return StatusCode(200, result);


            }
            catch
            {
                return StatusCode(500, "");
            }
        }

        [HttpPost("Save")]
        public ActionResult Save(DataObj dataObj)
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
                //long? formId = Convert.ToInt64(DataHelper.GetDataFromHeaders(Request, "FormId"));
                long? formId = ExtractFormId();

                JObject oo = JObject.Parse(dataObj.dataObj);
                //var parent = JsonConvert.DeserializeObject<JObject>(dataObj.dataObj);
                var headJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "attachments"
                && x.Name.ToLower() != "journalDetails"
                && x.Name.ToLower() != "journalCheques").ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }
                var detailsJson = oo["journalDetails"];
                var chequesJson = oo["journalCheques"];
                var attachments = oo["attachments"].ToArray();
                var formNotes = oo["formNotes"].ToArray();

                JObject newJson = new JObject();
                newJson.Add("Head", headJson);

                newJson.Add("journalDetails", detailsJson);
                newJson.Add("journalCheques", chequesJson);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", formId));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);
                DataHelper.ExecScalar("fcs_LedgerJourSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


                JObject o = JObject.Parse(result);
                #region //Save Attachments if record has been saved
                try
                {
                    HelperUtilitis.saveAttachments(formId, Convert.ToInt64(o["ID"]), attachments);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
                #endregion

                try
                {
                    HelperUtilitis.saveNotes(ExtractFormId(), Convert.ToInt64(oo["ID"]), GetCurrentUser(), formNotes);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }


                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpPost("Delete")]
        public ActionResult Delete(DataObj dataObj)
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
                //long? formId = Convert.ToInt64(DataHelper.GetDataFromHeaders(Request, "FormId"));
                long? formId = ExtractFormId();

                JObject oo = JObject.Parse(dataObj.dataObj);
                var headJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "attachments").ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }
                var attachments = oo["attachments"].ToArray();

                JObject newJson = new JObject();
                newJson.Add("Head", headJson);


                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", formId));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@Id", ((JToken)headJson["ID"]).ToString()));

                List<SqlParameter> listOutParameters = new List<SqlParameter>();

                DataHelper.ExecScalar("fcs_LedgerJourDelete", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());

                return StatusCode(200, true);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        // Trans Type Status -Janzreh
        [HttpGet("GetJournalStatuses")]
        public ActionResult GetJournalStatuses(string? TransTypeID = "")
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(TransTypeID))
                    l.Add(new SqlParameter("@TransTypeID", TransTypeID));
                var x = DataHelper.ExecScalar("Fcs_GetJournalStatuses", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

       
        [HttpPost("ImportFromExcel")]
        public async Task<ActionResult> ImportFromExcel()
        {
            try
            {
                if (!Request.HasFormContentType)
                {
                    return BadRequest("Unsupported media type");
                }
                string webRootPath = _webHostEnvironment.WebRootPath;
                string contentRootPath = _webHostEnvironment.ContentRootPath;
                var formCollection = await Request.ReadFormAsync();
                var files = formCollection.Files;
               
                
                foreach (var file in files)
                {
                    var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var buffer = new byte[file.Length];
                    //Do whatever you want with filename and its binary data.
                    using (var stream = file.OpenReadStream())
                    {
                        await stream.ReadAsync(buffer, 0, buffer.Length);
                    }

                    // File path handling
                    string localFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Reports",GetCurrentUser());
                    Directory.CreateDirectory(localFilePath); // Creates the directory if it does not exist

                    filename = $"{DateTime.Now:ddMMMMyy_hhmmssfff}_{filename}";
                    var filePath = Path.Combine(localFilePath, filename);
                    await System.IO.File.WriteAllBytesAsync(filePath, buffer);

                    DataTable dtImportedData = HelperUtilitis.ImportFromXls(filePath);
                    if (dtImportedData.Rows.Count > 0)
                    {
                        string guid = Guid.NewGuid().ToString();
                        #region Insert to fcs_ledgerJourLineImport
                        string sql = "";
                        decimal credit = 0;
                        decimal debit = 0;

                        for (int i = 0; i < dtImportedData.Rows.Count; i++)
                        {
                            DataRow row = dtImportedData.Rows[i];
                            decimal.TryParse(row["Credit"].ToString().Trim(), out credit);
                            decimal.TryParse(row["Debit"].ToString().Trim(), out debit);
                            if (credit <= 0 && debit <= 0) continue;
                            if (credit > 0 && debit > 0) continue;


                            sql += $@"INSERT INTO fcs_ledgerJourLineImport
                         (ImportTrxGuid, Rank, LedgerCode, CostCenterCode1, CostCenterCode2, CostCenterCode3, CostCenterCode4, Customer, Vendor, CreditAmount, DebitAmount, EnglishDescription, ArabicDescription)
                VALUES   ('{guid}',{row["Rank"].ToString().Trim()},'{row["LedgerCode"].ToString().Trim()}','{row["CostCenterCode1"].ToString().Trim()}'
                        ,'{row["CostCenterCode2"].ToString().Trim()}','{row["CostCenterCode3"].ToString().Trim()}','{row["CostCenterCode4"].ToString().Trim()}',
                            '{row["Customer"].ToString().Trim()}','{row["Vendor"].ToString().Trim()}',{credit},{debit},'{row["EnglishDescription"].ToString().Trim()}'
                        ,'{row["ArabicDescription"].ToString().Trim()}');" + "\r\n";
                            if (i > 0 && i % 20 == 0)
                            {
                                DataHelper.ExecNonQuery(sql, CommandType.Text);//TODO >> HANDEL sql injection
                                sql = "";
                            }
                        }
                        if (!string.IsNullOrEmpty(sql))
                        {
                            DataHelper.ExecNonQuery(sql, CommandType.Text);//TODO >> HANDEL sql injection
                        }
                        #endregion

                        #region get Imported Data
                        List<SqlParameter> l = new List<SqlParameter>();
                        l.Add(new SqlParameter("@User", GetCurrentUser()));
                        l.Add(new SqlParameter("@FormId", ExtractFormId()));
                        l.Add(new SqlParameter("@Lang", ExtractLang()));
                        l.Add(new SqlParameter("@guid", guid));


                        var x = DataHelper.ExecScalar("fcs_LedgerJourImport", System.Data.CommandType.StoredProcedure, l.ToArray());
                        if (string.IsNullOrEmpty(x.ToString()))
                        {
                            return StatusCode(200, "");
                        }
                        JObject o = JObject.Parse(x.ToString());
                        return StatusCode(200, o);
                        #endregion
                    }

                }

                return StatusCode(500, "No data to import.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }


        [HttpPost("SaveHMS")]
        public async Task<ActionResult> SaveHMS()
        {
            try
            {
                JObject parsedData = null;

                if (Request.ContentType.Contains("multipart/form-data"))
                {
                    var form = await HttpContext.Request.ReadFormAsync();
                    var dataObjJson = form["dataObj"].ToString();
                    parsedData = JObject.Parse(dataObjJson);
                }
                else if (Request.ContentType.Contains("application/x-www-form-urlencoded"))
                {
                    var form = await HttpContext.Request.ReadFormAsync();
                    var dataObjJson = form["dataObj"].ToString();
                    parsedData = JObject.Parse(dataObjJson);
                }
                else if (Request.ContentType.Contains("application/json"))
                {
                    var body = await new StreamReader(Request.Body).ReadToEndAsync();
                    parsedData = JObject.Parse(body);
                }
                else
                {
                    return StatusCode(StatusCodes.Status415UnsupportedMediaType, "Unsupported content type");
                }

                var headJson = new JObject();
                string pName = "";
                foreach (var item in parsedData.Properties().Where(x => x.Name.ToLower() != "attachments"
                    && x.Name.ToLower() != "journalDetails"
                    && x.Name.ToLower() != "journalCheques").ToArray())
                {
                    pName = item.Name.ToString().ToLower();
                    headJson.Add(item);
                }
                var detailsJson = parsedData["journalDetails"];

                JObject newJson = new JObject();
                newJson.Add("Head", headJson);
                newJson.Add("journalDetails", detailsJson);

                List<SqlParameter> l = new List<SqlParameter>
        {
            new SqlParameter("@User", GetCurrentUser()),
            new SqlParameter("@Lang", ExtractLang()),
            new SqlParameter("@json", newJson.ToString()) { Size = -1 }
        };

                List<SqlParameter> listOutParameters = new List<SqlParameter>
        {
            new SqlParameter("@Result", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output },
            new SqlParameter("@Error", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output }
        };

                DataHelper.ExecScalar("fcs_LedgerJourSave_HMS", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());

                var error = listOutParameters.Find(X => X.ParameterName == "@Error").Value;
                if (error != null && !string.IsNullOrEmpty(error.ToString()))
                {
                    DataHelper.Log(error.ToString());
                    return StatusCode(400, error);
                }

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, "Empty result from stored procedure");
                }

                JObject resultObj = JObject.Parse(result);
                return Ok(resultObj);
            }
            catch (Exception ex)
            {
                DataHelper.Log(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("DeleteByTransTypeHMS")]
        public async Task<ActionResult> DeleteByTransTypeHMS()
        {
            try
            {


                JObject parsedData = null;

                if (Request.ContentType.Contains("multipart/form-data"))
                {
                    var form = await HttpContext.Request.ReadFormAsync();
                    var dataObjJson = form["dataObj"].ToString();
                    parsedData = JObject.Parse(dataObjJson);
                }
                else if (Request.ContentType.Contains("application/x-www-form-urlencoded"))
                {
                    var form = await HttpContext.Request.ReadFormAsync();
                    var dataObjJson = form["dataObj"].ToString();
                    parsedData = JObject.Parse(dataObjJson);
                }
                else if (Request.ContentType.Contains("application/json"))
                {
                    var body = await new StreamReader(Request.Body).ReadToEndAsync();
                    parsedData = JObject.Parse(body);
                }
                else
                {
                    return StatusCode(StatusCodes.Status415UnsupportedMediaType, "Unsupported content type");
                }

                List<SqlParameter> l = new List<SqlParameter>();
                string userCode = GetCurrentUser();
                l.Add(new SqlParameter("@User", userCode));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", parsedData.ToString()) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("fcs_LedgerJourDelete_Bulk", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                var error = listOutParameters.Find(X => X.ParameterName == "@Error").Value;
                if (error != null && !string.IsNullOrEmpty(error.ToString()))
                {
                    DataHelper.Log(error.ToString());
                    return StatusCode(400, error);
                }

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    DataHelper.Log(error.ToString());
                    return StatusCode(500, result);
                }


                JObject o = JObject.Parse(result);

                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                DataHelper.Log(ex.Message);
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetCurrencies")]
        public ActionResult GetCurrencies()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));


                var x = DataHelper.ExecScalar("fcs_GetCurrencies", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpPost("SaveExternalLedgerJournal")]

        public ActionResult SaveExternalLedgerJournal(DataObj dataObj)
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

                JObject oo = JObject.Parse(dataObj.dataObj);
                //var parent = JsonConvert.DeserializeObject<JObject>(dataObj.dataObj);
                var headJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "attachments"
                && x.Name.ToLower() != "journalDetails"
                && x.Name.ToLower() != "journalCheques").ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }
                var detailsJson = oo["journalDetails"];

                JObject newJson = new JObject();
                newJson.Add("Head", headJson);
                newJson.Add("journalDetails", detailsJson);




                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("fcs_ExternalLedgerJournalSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                var error = listOutParameters.Find(X => X.ParameterName == "@Error").Value;
                if (error != null && !string.IsNullOrEmpty(error.ToString()))
                {
                    DataHelper.Log(error.ToString());
                    return StatusCode(400, error);
                }

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


                JObject o = JObject.Parse(result);
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                DataHelper.Log(ex.Message);
                return StatusCode(500, ex.Message);
            }


        }

    }
}

    

