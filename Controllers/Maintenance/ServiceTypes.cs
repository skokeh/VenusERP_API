using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using VenusERP_API.Utils;

namespace VenusERP_API.Controllers.INV
{

    public class ServiceTypesController : BaseApiController
    {
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

                var x = DataHelper.ExecScalar("mnt_ServiceTypesList", System.Data.CommandType.StoredProcedure, l.ToArray());
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

                DataHelper.ExecScalar("MNT_ServiceTypeDetails", CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
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

        [HttpGet("GetNextCode")]
        public ActionResult GetNextCode()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                var result = DataHelper.ExecScalar(@"select CAST( isnull((max(cast(code as bigint))+1),1) AS VARCHAR(20))
                                                       from mnt_ServiceTypes WHERE ISNUMERIC(code) = 1"
                                    , CommandType.Text);
                //return Content(HttpStatusCode.OK, result.ToString());
                return StatusCode(200, result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }


        }

        [HttpGet("GetServiceTypes")]
        public ActionResult GetServiceTypes()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                var x = DataHelper.ExecScalar("Fcs_GetMaintenanceServiceTypes", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        //[HttpGet("CheckCodeExist")]
        //public ActionResult CheckCodeExist(string code)
        //{
        //    try
        //    {
        //        List<SqlParameter> l = new List<SqlParameter>();
        //        l.Add(new SqlParameter("@Code", code));
        //        var result = DataHelper.ExecScalar(@"select count(*) from mms_CustomerAddresses WHERE code = @Code", CommandType.Text, l.ToArray());
        //        return StatusCode(200, result.ToString());
        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(500, ex);
        //    }


        //}

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
                // long? formId = Convert.ToInt64(DataHelper.GetDataFromHeaders(Request, "FormId"));

                JObject oo = JObject.Parse(dataObj.dataObj);
                //var parent = JsonConvert.DeserializeObject<JObject>(dataObj.dataObj);
                var headJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "attachments").ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }
                JObject newJson = new JObject();
                newJson.Add("Head", headJson);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("MNT_ServiceTypesSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


                // JObject o = JObject.Parse(result);



                return StatusCode(200, result);
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
                    //DataHelper.Log(DataHelper.GetHeaders(Request));
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
                    pName = item.Name.ToString().ToLower();
                    headJson.Add(item);
                }
                var attachments = oo["attachments"].ToArray();

                JObject newJson = new JObject();
                newJson.Add("Head", headJson);


                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", formId));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@Id", headJson["ID"].ToString()));

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("MNT_ServiceTypesDelete", CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                return StatusCode(200, true);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }


        }
    }
}
