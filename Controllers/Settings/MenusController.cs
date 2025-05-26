using Microsoft.AspNetCore.Authorization;
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


namespace VenusERP_API.Controllers.Settings
{
    public class MenusController : BaseApiController
    {

        [HttpGet("GetList")]
        public ActionResult GetList()
        {
         
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                var x = DataHelper.ExecScalar("sys_GetMenuSettings", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, x);
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

                DataHelper.ExecScalar("sys_FormDetails", CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
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

                //return Content(HttpStatusCode.InternalServerError, ex.Message);
                return StatusCode(500, ex.Message);

            }

        }

        [HttpPost("Update")]

        public ActionResult Update(DataObj dataObj)
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
                var headJson = new JObject();

                JObject newJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().ToArray())
                {
                    pName = item.Name.ToString().ToLower();
                    headJson.Add(item);
                }
                newJson.Add("Head", headJson);
                List<SqlParameter> l = new List<SqlParameter>
                {
                    new SqlParameter("@User", GetCurrentUser()),
                    new SqlParameter("@Lang", ExtractLang()),
                    new SqlParameter("@json", newJson.ToString()) { Size = -1 }
                };
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar); DataHelper.ExecScalar("sys_UpdateMenuSettings", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
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
                //    return Content(HttpStatusCode.InternalServerError, ex.Message);
                //}
                #endregion

                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpPost("MenuShortcutsSave")]
        public ActionResult MenuShortcutsSave(DataObj dataObj)
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
                // long? formId = Convert.ToInt64(DataHelper.GetDataFromHeaders(Request, "FormId"));

                //JObject oo = JObject.Parse(dataObj.dataObj);
                //var parent = JsonConvert.DeserializeObject<JObject>(dataObj.dataObj);
                //var headJson = new JObject();

                //JObject newJson = new JObject();
                //string pName = "";
                //foreach (var item in oo.Properties().ToArray())
                //{
                //    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                //    headJson.Add(item);
                //}
                //newJson.Add("Head", headJson);
                //List<SqlParameter> l = new List<SqlParameter>
                //{
                //    new SqlParameter("@User", DataHelper.GetUserName()),
                //    // l.Add(new SqlParameter("@FormId", formId));
                //    //l.Add(new SqlParameter("@Lang", DataHelper.GetDataFromHeaders(Request, "Lang")));
                //    new SqlParameter("@json", newJson.ToString()) { Size = -1 }
                //};
                //List<SqlParameter> listOutParameters = new List<SqlParameter>();
                //SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                //listOutParameters.Add(resPar);
                //SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                //listOutParameters.Add(errPar);

                //long? formId = Convert.ToInt64(DataHelper.GetDataFromHeaders(Request, "FormId"));
                JObject oo = JObject.Parse(dataObj.dataObj);
                //var parent = JsonConvert.DeserializeObject<JObject>(dataObj.dataObj);
                var headJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "attachments").ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }


                //JObject newJson = new JObject();
                //newJson.Add("Head", headJson);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                //l.Add(new SqlParameter("@FormId", formId));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", headJson.ToString()) { Size = -1 });
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("sys_UserShortcutsSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


                // JObject o = JObject.Parse(result.ToString());



                return StatusCode(200, "");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("GetNextCode")]
        public ActionResult GetNextCode()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                var result = DataHelper.ExecScalar(@"select CAST( isnull((max(cast(code as bigint))+1),1) AS VARCHAR(20))
                                                       from fcs_Ledger WHERE ISNUMERIC(code) = 1"
                                    , System.Data.CommandType.Text);
                return StatusCode(200, result.ToString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("CheckCodeExist")]
        public ActionResult CheckCodeExist(string code)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@Code", code));
                var result = DataHelper.ExecScalar(@"select count(*) from fcs_Ledger WHERE code = @Code"
                                    , System.Data.CommandType.Text, l.ToArray());
                return StatusCode(200, result.ToString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }
    }
}
