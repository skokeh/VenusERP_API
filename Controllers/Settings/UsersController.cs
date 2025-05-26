using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Security.Claims;
using VenusERP_API.Models;
using VenusERP_API.Utils;
using VenusERP_Application.Settings;

namespace VenusERP_API.Controllers.Settings
{

    public class UsersController : BaseApiController
    {
        private readonly UsersService _usersService;
        public UsersController(UsersService usersService)
        {
            _usersService = usersService;
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

                var x = DataHelper.ExecScalar("sys_UsersList", CommandType.StoredProcedure, l.ToArray());
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
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ID", ID));
                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                DataHelper.ExecScalar("sys_UserDetails", CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
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

                //return  return StatusCode(500, ex.Message);
                return StatusCode(500, ex.Message);

            }


        }
        [AllowAnonymous]
        [HttpPost("BulkEncryptPassword")]
        public async Task<ActionResult> BulkEncryptPassword()
        {
            try
            {
                var result = await _usersService.PasswordEncrypt();
                if (result != null)
                {
                    return StatusCode(200, result);
                }
                return StatusCode(500, result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
        [AllowAnonymous]

        [HttpGet("UsernameValidation")]
        public async Task<ActionResult> UsernameValidation(string username, string url)
        {
            var result = await _usersService.ValidatingUserName(username, url, ExtractLang());
            if(result.Code == HttpStatusCode.OK)
            {
                return StatusCode(200, result);
            }
            if(result.Code == HttpStatusCode.Accepted)
            {
                return StatusCode(202, result);
            }
            return StatusCode(500, result.Message);
        }

        [AllowAnonymous]
        [HttpGet("ValidatedUserToken")]
        public async Task<ActionResult> ValidatedUserToken(string token) { 
            var result = await _usersService.ValidatedToken(token);
            if(result.Code == HttpStatusCode.OK)
            {
                return StatusCode(200, result.Object);
            }
            return StatusCode((int)result.Code, result.Message);
        }

        [AllowAnonymous]
        [HttpPut("PasswordReset")]
        public async Task<ActionResult> PasswordReset(UserLogin userLogin)
        {
            var result = await _usersService.UserPasswordReset(userName:userLogin.UserName, newPassword:userLogin.Password);
            if(result.Code == HttpStatusCode.OK) { return StatusCode(200, result); }
            return StatusCode(500, result.Message);
        }
        [HttpGet("GetDashboardSettings")]
        public ActionResult GetDashboardSettings()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                DataHelper.ExecScalar("sys_GetUserDashboardPosition", CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
                string result = listOutPutParams.Find(X => X.ParameterName == "@Result").Value.ToString();
                if (string.IsNullOrEmpty(result.ToString()))
                {
                    return StatusCode(200, result.ToString());
                }
                JObject o = JObject.Parse(result);
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetUserDashboardSettings")]
        public ActionResult GetUserDashboardSettings()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                List<SqlParameter> listOutPutParams = new List<SqlParameter>
                {
                    new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output },
                    new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output }
                };
                DataHelper.ExecScalar("sys_GetUsersDshBord", CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
                string result = listOutPutParams.Find(X => X.ParameterName == "@Result").Value.ToString();
                if (string.IsNullOrEmpty(result.ToString()))
                {
                    return StatusCode(200, result.ToString());
                }
                JObject o = JObject.Parse(result);
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("GetUserDashboard")]

        public ActionResult GetUserDashboard(DataObj dataObj)
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
                    pName = item.Name.ToString().ToLower();
                    headJson.Add(item);
                }
                var getstoreprocedure = oo["datasource"].ToString();
                newJson.Add("Head", headJson);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                // l.Add(new SqlParameter("@FormId", formId));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar(getstoreprocedure, CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }

                JObject o = JObject.Parse(result.ToString());


                return StatusCode(200, o);
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
                                                       from sys_Users WHERE ISNUMERIC(code) = 1"
                                    , CommandType.Text);
                return StatusCode(200, result);
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
                var result = DataHelper.ExecScalar(@"select count(*) from sys_Users WHERE code = @Code"
                                    , CommandType.Text, l.ToArray());
                return StatusCode(200, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                long? formId = ExtractFormId();
                JObject oo = JObject.Parse(dataObj.dataObj);
                //var parent = JsonConvert.DeserializeObject<JObject>(dataObj.dataObj);
                var headJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "attachments"
                 && x.Name.ToLower() != "formsPermissions" && x.Name.ToLower() != "reportsPermissions"
                 && x.Name.ToLower() != "userLocations" && x.Name.ToLower() != "otStatusPermissions"
                 && x.Name.ToLower() != "ttStatusPermissions"
                 && x.Name.ToLower() != "menusPermissions"
                 && x.Name.ToLower() != "accountscodesPermissions"
                 && x.Name.ToLower() != "userDestinationLocation"
                 && x.Name.ToLower() != "userDashboardSetting"
                     && x.Name.ToLower() != "userImage"
                 ).ToArray())
                {
                    pName = item.Name.ToString().ToLower();
                    headJson.Add(item);
                }
                if (headJson.ContainsKey("password"))
                {
                    string plainPassword = headJson["password"].ToString();
                    long userId = headJson.ContainsKey("ID") ? Convert.ToInt64(headJson["ID"]) : 0;
                    bool changePass = headJson.ContainsKey("changePass") && headJson["changePass"].ToObject<bool>();


                    if (userId == 0 || changePass)
                    {
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);
                        headJson["password"] = hashedPassword;
                    }
                }
                var attachments = oo["attachments"].ToArray();
                var formsPermissions = oo["formsPermissions"];
                var reportsPermissions = oo["reportsPermissions"];
                var userLocations = oo["userLocations"];
                var otStatusPermissions = oo["otStatusPermissions"];
                var ttStatusPermissions = oo["ttStatusPermissions"];
                var menusPermissions = oo["menusPermissions"];
                var userDashboardSetting = oo["userDashboardSetting"];
                var userDestinationLocation = oo["userDestinationLocation"];
                var accountscodesPermissions = oo["accountscodesPermissions"];
                var userTransTypesPermission = oo["userTransTypesPermission"];
                var userCostCenterPermission = oo["userCostCenterPermission"];
                var userLedgerPermissions = oo["userLedgerPermissions"];
                var userLedgerClassPermissions = oo["userLedgerClassPermissions"];
                var userCostCenters = oo["userCostCenters"];
                var userVendorPermission = oo["userVendorPermission"];
                var userCustomerPermission = oo["userCustomerPermission"];
                var userItemsPermission = oo["userItemsPermission"];
                var userImage = oo["userimage"].ToString();
                JObject newJson = new JObject();
                newJson.Add("Head", headJson);
                newJson.Add("formsPermissions", formsPermissions);
                newJson.Add("reportsPermissions", reportsPermissions);
                newJson.Add("userLocations", userLocations);
                newJson.Add("otStatusPermissions", otStatusPermissions);
                newJson.Add("ttStatusPermissions", ttStatusPermissions);
                newJson.Add("menusPermissions", menusPermissions);
                newJson.Add("accountscodesPermissions", accountscodesPermissions);
                newJson.Add("userDashboardSetting", userDashboardSetting);
                newJson.Add("userDestinationLocation", userDestinationLocation);
                newJson.Add("userTransTypesPermission", userTransTypesPermission);
                newJson.Add("userCostCenterPermission", userCostCenterPermission);
                newJson.Add("userLedgerPermissions", userLedgerPermissions);
                newJson.Add("userLedgerClassPermissions", userLedgerClassPermissions);
                newJson.Add("userCostCenters", userCostCenters);
                newJson.Add("userVendorPermission", userVendorPermission);
                newJson.Add("userCustomerPermission", userCustomerPermission);
                newJson.Add("userItemsPermission", userItemsPermission);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", formId));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });
                l.Add(new SqlParameter("@DefaultImage", userImage));
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);
                DataHelper.ExecScalar("sys_UsersSave", CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
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

        [HttpPost("SaveShortcutByFormId")]

        public ActionResult SaveShortcutByFormId(DataObj dataObj)
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
                var formId = ExtractFormId();
                var reportId = ExtractReportId();
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                if (formId != null) l.Add(new SqlParameter("@FormId", formId));
                if (reportId != null) l.Add(new SqlParameter("@ReportId", reportId));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);
                DataHelper.ExecScalar("sys_UserShortcutsSaveUsingFormId", CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
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

        [HttpGet("ChangePassword")]

        public ActionResult ChangePassword(string? CurrentPassword, string? NewPassword)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@CurrentPassword", CurrentPassword));
                l.Add(new SqlParameter("@NewPassword", NewPassword));
                var lo = new List<SqlParameter>();
                lo.Add(new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });


                DataHelper.ExecScalar("sys_ChangePassword", System.Data.CommandType.StoredProcedure, ref lo, l.ToArray());
                string error = lo[0].Value.ToString();
                if (string.IsNullOrEmpty(error))
                {
                    return StatusCode(200, "");
                }
                else
                {
                    return StatusCode(500, "Error");
                }
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

                DataHelper.ExecScalar("sys_UsersDelete", CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
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

        [HttpGet("GetformsAndReports")]
        public ActionResult GetformsAndReports()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                var x = DataHelper.ExecScalar("sys_GetformsAndReports", CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetLookups")]
        public ActionResult GetLookups()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                var x = DataHelper.ExecScalar("sys_GetUsersLookups", CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(200, ex.Message);
            }


        }


        [HttpGet("GetTransTypeStatus")]
        public ActionResult GetTransTypeStatus()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                var x = DataHelper.ExecScalar("sys_GetUsersTransTypeStatus", CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetUserRemainders")]
        public ActionResult GetUserRemainders()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                List<SqlParameter> listOutPutParams = new List<SqlParameter>
                {
                    new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output },
                    new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output }
                };
                DataHelper.ExecScalar("sys_GetUserReminders", CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
                string result = listOutPutParams.Find(X => X.ParameterName == "@Result").Value.ToString();
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

        [HttpPost("SaveUserRemainders")]
        public ActionResult SaveUserRemainders(DataObj dataObj)
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
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "attachments"))
                {
                    pName = item.Name.ToString().ToLower();
                    headJson.Add(item);
                }
                var attachments = oo["attachments"].ToArray();
                newJson.Add("Head", headJson);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("sys_RemindersSave", CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);
                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }
                else
                {
                    JObject file;
                    if (attachments.Length > 0)
                    {
                        List<SqlParameter> listAttachments;
                        int RemainderID = Convert.ToInt32(result);

                        for (int k = 0; k < attachments.Length; k++)
                        {
                            file = (JObject)attachments[k];
                            if (file["data"] != null)//New Attachments Only
                            {
                                listAttachments = new List<SqlParameter>();
                                listAttachments.Add(new SqlParameter("@User", GetCurrentUser()));
                                listAttachments.Add(new SqlParameter("@RemainderID", RemainderID));
                                listAttachments.Add(new SqlParameter("@FileName", file["path"].ToString()));
                                listAttachments.Add(new SqlParameter("@Size", file["size"].ToString()));
                                listAttachments.Add(new SqlParameter("@type", file["type"].ToString()));

                                byte[] imageData = Convert.FromBase64String(file["data"].ToString().Substring(("data:" + file["type"] + ";base64,").Length));
                                listAttachments.Add(new SqlParameter("@AttachedFile", SqlDbType.Binary, imageData.Length) { Value = imageData });
                                bool x = DataHelper.ExecNonQuery("sys_ReminderAttachments", CommandType.StoredProcedure, listAttachments.ToArray());

                            }




                        }
                    }
                }
                #region //Save Attachments if record has been saved
                //try
                //{
                //    HelperUtilitis.saveAttachments(formId, Convert.ToInt64(o["ID"]), attachments);
                //}
                //catch (Exception ex)
                //{
                //    return  return StatusCode(500, ex.Message);
                //}
                #endregion


                return StatusCode(200, "Remainder Added Successfully");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("DeleteUserRemainders")]
        public ActionResult DeleteUserRemainders(DataObj dataObj)
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
                    pName = item.Name.ToString().ToLower();
                    headJson.Add(item);
                }
                newJson.Add("Head", headJson);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                // l.Add(new SqlParameter("@FormId", formId));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("sys_RemindersDelete", CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);


                #region //Save Attachments if record has been saved
                //try
                //{
                //    HelperUtilitis.saveAttachments(formId, Convert.ToInt64(o["ID"]), attachments);
                //}
                //catch (Exception ex)
                //{
                //    return  return StatusCode(500, ex.Message);
                //}
                #endregion


                return StatusCode(200, "Remainder Deleted Successfully");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetUserNotification/{date}")]
        public ActionResult GetUserNotification(string? date)
        {
            try
            {
                //JObject oo = JObject.Parse(dataObj.dataObj);
                //var date = oo["todayDate"];
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@NotificationDate", date));
                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });

                DataHelper.ExecScalar("sys_PushNotificationsTrans", CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
                string result = listOutPutParams.Find(X => X.ParameterName == "@Result").Value.ToString();
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

        [HttpGet("GetAllUserNotificationList")]
        public ActionResult GetAllUserNotificationList()
        {

            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                DataHelper.ExecScalar("sys_GetUserNotificationTransList", CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
                string result = listOutPutParams.Find(X => X.ParameterName == "@Result").Value.ToString();
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

        [HttpPost("MarkNotificationAsRead")]
        public ActionResult MarkNotificationAsRead(int id)
        {
            try
            {
                DataSet MrtDataSet = new DataSet();
                MrtDataSet = DataHelper.ExcuteDataSet(@"UPDATE Sys_NotificationsTrans set ReadedDate = GETDATE() where ID=" + id);
                MrtDataSet.AcceptChanges();
                return StatusCode(200, "Remainder Added Successfully");
            }
            catch
            {
                return StatusCode(500, "");
            }
        }

        [HttpPost("MarkAllNotificationAsRead")]
        public ActionResult MarkAllNotificationAsRead()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                //l.Add(new SqlParameter("@FormId", DataHelper.GetDataFromHeaders(Request, "FormId")));

                var x = DataHelper.ExecScalar("sys_UpdateUserNotificationsReadAll", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (x == null)
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
    }
}
