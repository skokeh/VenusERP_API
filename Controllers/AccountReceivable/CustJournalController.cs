
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using VenusERP_API.Utils;


namespace VenusERP_API.Controllers.AccountReceivable
{
    public class CustJournalController : BaseApiController
    {
        [HttpGet("GetList")]
        
        public ActionResult GetList(int pageSize, int pageNumber, string? orderBy = "", string? orderDirection = "", string? criteria = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
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

                var x = DataHelper.ExecScalar("fcs_CustJourGetList", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet( "GetByID")]
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

                DataHelper.ExecScalar("fcs_CustJourDetails", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
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

        [HttpGet( "GetNextCode")]
        public ActionResult GetNextCode(int TransTypeID,string JournalDate)
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
                l.Add(new SqlParameter("@TableName", "fcs_CustJour"));
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

        [HttpGet("getTransTypes")]
        public ActionResult getTransTypes()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));


                var x = DataHelper.ExecScalar("fcs_GetCustTransTypes", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("srchCusts")]
        public ActionResult srchCusts( string? ExciptIds = "", string? Filter = "")
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

                var x = DataHelper.ExecScalar("fcs_srchCusts", System.Data.CommandType.StoredProcedure, l.ToArray());
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
        public ActionResult srchCostCenters1(string? ExciptIds = "", string? Filter = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@LevelId", 1));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));

                var x = DataHelper.ExecScalar("fcs_srchCostCenters", System.Data.CommandType.StoredProcedure, l.ToArray());
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
        public ActionResult srchCostCenters2(string? ExciptIds = "", string? Filter = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@LevelId", 2));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));

                var x = DataHelper.ExecScalar("fcs_srchCostCenters", System.Data.CommandType.StoredProcedure, l.ToArray());
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
        public ActionResult srchCostCenters3(string? ExciptIds = "", string? Filter = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@LevelId", 3));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));

                var x = DataHelper.ExecScalar("fcs_srchCostCenters", System.Data.CommandType.StoredProcedure, l.ToArray());
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
        public ActionResult srchCostCenters4(string? ExciptIds = "", string? Filter = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@LevelId", 4));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));

                var x = DataHelper.ExecScalar("fcs_srchCostCenters", System.Data.CommandType.StoredProcedure, l.ToArray());
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
        [HttpPost( "Save")]
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
               
                long? formId = ExtractFormId();
                JObject oo = JObject.Parse(dataObj.dataObj);
               
                var headJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "attachments" && x.Name.ToLower() != "journalDetails").ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }
                var detailsJson = oo["journalDetails"];
                var attachments = oo["attachments"].ToArray();

                JObject newJson = new JObject();
                newJson.Add("Head", headJson);
                newJson.Add("journalDetails", detailsJson);

                


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

                DataHelper.ExecScalar("fcs_CustJourSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
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

                DataHelper.ExecScalar("fcs_CustJourDelete", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());

                return StatusCode(200, true);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }
    }
}
