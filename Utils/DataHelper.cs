using Microsoft.Extensions.Hosting.Internal;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace VenusERP_API.Utils
{
    public static  class DataHelper
    {

        public static object ExecScalar(string SqlStatement, CommandType ComType, SqlParameter[] Params = null)
        {
            object Result = null;

            using (SqlConnection SqlConName = new SqlConnection(ReturnSqlConnectonString()))
            {
                if (string.IsNullOrEmpty(SqlConName.ConnectionString)) return null;
                using (SqlCommand SqlCmd = new SqlCommand())
                {
                    SqlCmd.CommandType = System.Data.CommandType.Text;
                    SqlCmd.CommandText = SqlStatement;
                    SqlCmd.CommandType = ComType;
                    SqlCmd.CommandTimeout = 0;

                    //add params if not null
                    if (Params != null && Params.Length > 0)
                    {
                        SqlCmd.Parameters.AddRange(Params);
                    }
                    SqlCmd.Connection = SqlConName;
                    try
                    {
                        if (SqlConName.State == System.Data.ConnectionState.Open) { SqlConName.Close(); }
                        SqlConName.Open();
                        Result = SqlCmd.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {

                        var sb = new StringBuilder();
                        sb.AppendLine("Date:           " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        sb.AppendLine("Error Message:  " + ex.Message);
                        sb.AppendLine("----------------------------------------------------------------");

                        //var fileName = Path.Combine(HostingEnvironment.MapPath("~/Errors"), "log.txt");
                        //File.AppendAllText(fileName, sb.ToString());
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("Date:           " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        sb.AppendLine("Error Message:  " + ex.Message);
                        sb.AppendLine("----------------------------------------------------------------");

                        //var fileName = Path.Combine(HostingEnvironment.MapPath("~/Errors"), "log.txt");
                        //File.AppendAllText(fileName, sb.ToString());
                        //File.AppendAllText(fileName, SqlStatement);
                        //foreach (var item in Params)
                        //{
                        //    File.AppendAllText(fileName, $"ParameterName {item.ParameterName}, Value: {item.SqlValue}");
                        //}
                        throw ex;
                    }
                    finally
                    {
                        SqlConName.Close();
                    }
                }
            }


            return Result;


        }

        public static DataSet ExcuteDataSet(string q)
        {
            //try
            //{
            DataSet ObData = new DataSet();
            // "Set DATEFORMAT DMY;" + 
            var mSqlDataAdapter = new System.Data.SqlClient.SqlDataAdapter(q, ReturnSqlConnectonString());
            mSqlDataAdapter.SelectCommand.CommandTimeout = 0;
            mSqlDataAdapter.Fill(ObData);
            return ObData;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }
        public static DataTable ExcuteDataTable(string SqlStatement, CommandType ComType, SqlParameter[] Params = null)
        {
            DataTable Result = new DataTable();

            using (SqlConnection SqlConName = new SqlConnection(ReturnSqlConnectonString()))
            {
                if (string.IsNullOrEmpty(SqlConName.ConnectionString)) return null;
                using (SqlCommand SqlCmd = new SqlCommand())
                {
                    SqlCmd.CommandType = System.Data.CommandType.Text;
                    SqlCmd.CommandText = SqlStatement;
                    SqlCmd.CommandType = ComType;
                    SqlCmd.CommandTimeout = 0;

                    //add params if not null
                    if (Params != null && Params.Length > 0)
                    {
                        SqlCmd.Parameters.AddRange(Params);
                    }
                    SqlCmd.Connection = SqlConName;
                    try
                    {

                        DataTable ObData = new DataTable();
                        // "Set DATEFORMAT DMY;" + 
                        var mSqlDataAdapter = new System.Data.SqlClient.SqlDataAdapter(SqlCmd);
                        mSqlDataAdapter.SelectCommand.CommandTimeout = 0;
                        mSqlDataAdapter.Fill(Result);
                    }
                    catch (SqlException ex)
                    {

                        var sb = new StringBuilder();
                        sb.AppendLine("Date:           " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        sb.AppendLine("Error Message:  " + ex.Message);
                        sb.AppendLine("----------------------------------------------------------------");

                        var fileName = "";/* Path.Combine(HostingEnvironment.MapPath("~/Errors"), "log.txt");*/
                        File.AppendAllText(fileName, sb.ToString());
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("Date:           " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        sb.AppendLine("Error Message:  " + ex.Message);
                        sb.AppendLine("----------------------------------------------------------------");

                        var fileName = "";/*Path.Combine(HostingEnvironment.MapPath("~/Errors"), "log.txt");*/
                        File.AppendAllText(fileName, sb.ToString());
                        File.AppendAllText(fileName, SqlStatement);
                        foreach (var item in Params)
                        {
                            File.AppendAllText(fileName, $"ParameterName {item.ParameterName}, Value: {item.SqlValue}");
                        }
                        throw ex;
                    }
                    finally
                    {
                        SqlConName.Close();
                    }
                }
            }


            return Result;





        }
        public static object ExecScalar(string SqlStatement, CommandType ComType, ref List<SqlParameter> OutputParameters, SqlParameter[] Params = null)
        {
            object Result = null;

            using (SqlConnection SqlConName = new SqlConnection(ReturnSqlConnectonString()))
            {
                if (string.IsNullOrEmpty(SqlConName.ConnectionString)) return null;
                using (SqlCommand SqlCmd = new SqlCommand())
                {
                    SqlCmd.CommandType = System.Data.CommandType.Text;
                    SqlCmd.CommandText = SqlStatement;
                    SqlCmd.CommandType = ComType;
                    SqlCmd.CommandTimeout = 0;
                    //add params if not null
                    if (Params != null && Params.Length > 0)
                    {
                        SqlCmd.Parameters.AddRange(Params);
                    }
                    //add Output Parameters if not null
                    if (OutputParameters != null && OutputParameters.Count > 0)
                    {
                        SqlCmd.Parameters.AddRange(OutputParameters.ToArray());
                    }
                    SqlCmd.Connection = SqlConName;
                    try
                    {
                        if (SqlConName.State == System.Data.ConnectionState.Open) { SqlConName.Close(); }
                        SqlConName.Open();
                        Result = SqlCmd.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {

                        var sb = new StringBuilder();
                        sb.AppendLine("Date:           " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        sb.AppendLine("Error Message:  " + ex.Message);
                        sb.AppendLine("----------------------------------------------------------------");

                        //var fileName = Path.Combine(HostingEnvironment.MapPath("~/Errors"), "log.txt");
                        //File.AppendAllText(fileName, sb.ToString());
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("Date:           " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        sb.AppendLine("Error Message:  " + ex.Message);
                        sb.AppendLine("----------------------------------------------------------------");

                        //var fileName = Path.Combine(HostingEnvironment.MapPath("~/Errors"), "log.txt");
                       // File.AppendAllText(fileName, sb.ToString());
                        throw ex;
                    }
                    finally
                    {
                        SqlConName.Close();
                    }
                }
            }


            return Result;

        }
        public static bool ExecNonQuery(string SqlStatement, CommandType ComType, SqlParameter[] Params = null)
        {
            bool success = false;

            using (SqlConnection SqlConName = new SqlConnection(ReturnSqlConnectonString()))
            {
                if (string.IsNullOrEmpty(SqlConName.ConnectionString))
                    return success;
                using (SqlCommand SqlCmd = new SqlCommand())
                {
                    SqlCmd.CommandType = System.Data.CommandType.Text;
                    SqlCmd.CommandText = SqlStatement;
                    SqlCmd.CommandType = ComType;

                    //add params if not null
                    if (Params != null && Params.Length > 0)
                    {
                        SqlCmd.Parameters.AddRange(Params);
                    }
                    SqlCmd.Connection = SqlConName;
                    try
                    {
                        if (SqlConName.State == System.Data.ConnectionState.Open) { SqlConName.Close(); }
                        SqlConName.Open();
                        if (ComType == CommandType.StoredProcedure)
                        {
                            if (SqlCmd.ExecuteNonQuery() == 1)
                                success = false;
                            else
                                success = true;

                        }
                        else
                        {
                            if (SqlCmd.ExecuteNonQuery() == 0)
                                success = false;
                            else
                                success = true;
                        }

                    }
                    catch (SqlException ex)
                    {

                        var sb = new StringBuilder();
                        sb.AppendLine("Date:           " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        sb.AppendLine("Error Message:  " + ex.Message);
                        sb.AppendLine("----------------------------------------------------------------");

                       // var fileName = Path.Combine(HostingEnvironment.MapPath("~/Errors"), "log.txt");
                       // File.AppendAllText(fileName, sb.ToString());
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("Date:           " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        sb.AppendLine("Error Message:  " + ex.Message);
                        sb.AppendLine("----------------------------------------------------------------");

                        //var fileName = Path.Combine(HostingEnvironment.MapPath("~/Errors"), "log.txt");
                        //File.AppendAllText(fileName, sb.ToString());
                        throw ex;
                    }
                    finally
                    {
                        SqlConName.Close();
                    }
                }
            }


            return success;





        }
        public static string ReturnSqlConnectonString()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            IConfiguration Configuration = builder.Build();
            return Configuration["ConnectionStrings:DefaultConnection"];
            //return @"Data Source=ABUBAKR-PC\SQL2017EXPRESS;Initial Catalog=Dan_VenusNet_API;Persist Security Info=False;Connection Timeout=120;user id=sa;password=123;";
        }


        public static string GetCompanyId()
        {
            string companyId = "15";
            //try
            //{
            //    var claims = ((System.Security.Claims.ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims;
            //    foreach (var item in claims)
            //    {
            //        if (item.Type.ToLower().Equals("companyId"))
            //        {
            //            companyId = item.Value;
            //            break;
            //        }
            //    }
            //}
            //catch (Exception)
            //{

            //}
            return companyId;
        }

        public static string GetAccountId()
        {
            string accountId = "1";
            //try
            //{
            //    var claims = ((System.Security.Claims.ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims;
            //    foreach (var item in claims)
            //    {
            //        if (item.Type.ToLower().Equals("accountId"))
            //        {
            //            companyId = item.Value;
            //            break;
            //        }
            //    }
            //}
            //catch (Exception)
            //{

            //}
            return accountId;
        }

        public static string GetBranchId()
        {
            string branchId = "1";
            //try
            //{
            //    var claims = ((System.Security.Claims.ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims;
            //    foreach (var item in claims)
            //    {
            //        if (item.Type.ToLower().Equals("branchId"))
            //        {
            //            companyId = item.Value;
            //            break;
            //        }
            //    }
            //}
            //catch (Exception)
            //{

            //}
            return branchId;
        }
        public static bool IsUserAuthenticated(string User, string Pass)
        {
            List<SqlParameter> l = new List<SqlParameter>();
            l.Add(new SqlParameter("@User", User));
            l.Add(new SqlParameter("@Pass", Pass));

            var x = DataHelper.ExecScalar("sp_IsUserAuthenticated", System.Data.CommandType.StoredProcedure, l.ToArray());

            return Convert.ToBoolean(x);

        }
        public static string GetDataFromHeaders(System.Net.Http.HttpRequestMessage req, string key)
        {
            //var headers = Request.Headers;
            var headers = req.Headers;
            if (headers.Contains(key))
                return headers.GetValues(key).First();
            return "";
        }
        public static string GetHeaders(System.Net.Http.HttpRequestMessage req)
        {
            //var headers = Request.Headers;
            StringBuilder s = new StringBuilder();
            var headers = req.Headers;
            foreach (var item in headers)
            {
                s.AppendLine(item.Key + " : " + ((string[])item.Value)[0]);

            }
            return s.ToString();
        }
        public static void Log(string message)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Date:           " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
               // sb.AppendLine($"User Name:{GetUserName()} Message:{message}  ");
                sb.AppendLine("----------------------------------------------------------------");

                //var fileDirectory = Path.Combine(HostingEnvironment.MapPath("~/Errors/" + DateTime.Now.Date.ToString("ddMMMMyy")));

                //bool exists = System.IO.Directory.Exists(fileDirectory);

                //if (!exists)
                //    System.IO.Directory.CreateDirectory(fileDirectory);

                ////var fileName = Path.Combine(fileDirectory, GetUserName() + "_log.txt");
                //File.AppendAllText(fileName, sb.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class DataObj
    {
        public string dataObj { get; set; }
    }
    public class GetListHeader
    {
        public string criteria { get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
        public string orderBy { get; set; }
        public string orderDirection { get; set; }

    }
}

