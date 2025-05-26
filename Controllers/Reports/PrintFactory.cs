using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using System.Collections;
using System.Data;
using VenusERP_API.Utils;

namespace VenusERP_API.Controllers.Reports
{
    public class PrintFactory
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        public PrintFactory(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }
        private DataSet ReportDataSet = new DataSet(), MrtDataSet = new DataSet();
        #region mobile
        public JObject GetDataAsJson(string ReportId, List<CriteriaField> listCriteria, string userName, bool isArabic, string FormId)
        {
            //TODO CHECK REPORTS PERMESSIONS
            string reportPath = "";
            string fixedDirection = "";
            string subQuery = "";
            if (!string.IsNullOrEmpty(FormId))
            {
                subQuery = $@" UNION
                    SELECT R.ID FROM sys_FormsReportsLinks FRL
                    INNER JOIN sys_FormsPermissions FP ON FP.FormID=FRL.SourceFormID AND FP.AllowPrint = 1
                    INNER JOIN sys_Users U  ON FP.UserID=U.ID 
                    INNER JOIN sys_StiReports R  ON R.ID=FRL.TargetReportID
                    WHERE U.Code='{userName}' AND FRL.TargetReportID={ReportId} AND FormID={FormId}";
            }

            MrtDataSet = DataHelper.ExcuteDataSet($@"SELECT code,DataSource,ISNULL(P.name,'') ParameterUser
                    ,ReportPath
                    ,FixedDirection
                    ,(SELECT DISTINCT p.PARAMETER_NAME
					 FROM INFORMATION_SCHEMA.PARAMETERS p
					 WHERE p.PARAMETER_NAME = '@Lang' and p.SPECIFIC_NAME=R.DataSource) as ParameterLang
                    FROM sys_StiReports R
                    LEFT JOIN sys.Objects O ON O.name = R.DataSource
                    LEFT JOIN sys.parameters P  ON O.object_id = P.object_id  AND P.name IN ( '@User','@CurrentUserCode')
                    WHERE R.ID = {ReportId}

                    SELECT * FROM 
                    (SELECT RP.ID FROM sys_StiReportsPermissions  RP
                    	INNER JOIN sys_Users U  ON RP.UserID=U.ID  WHERE U.Code='{userName}' 
                            AND ReportID={ReportId} "
                   + subQuery + @"
                    )D
                     ");


            //SELECT * FROM sys_StiReportsPermissions RP
            //INNER JOIN sys_Users U  ON RP.UserID=U.ID 
            //WHERE U.Code='{userName }' AND RP.ReportID={ReportId} ");
            reportPath = MrtDataSet.Tables[0].Rows[0]["ReportPath"].ToString();
            fixedDirection = MrtDataSet.Tables[0].Rows[0]["FixedDirection"].ToString();
            int hasPermession = MrtDataSet.Tables[1].Rows.Count;
            if (hasPermession <= 0)
            {
                throw new Exception(isArabic ? "ليس لديك صلاحية فتح هذا التقرير" : "You are not authorized to view this report.");
            }
            string sql = @"set dateformat ymd ; Exec " + MrtDataSet.Tables[0].Rows[0]["DataSource"] + " ";
            foreach (DataRow row in MrtDataSet.Tables[0].Rows)
            {
                if (row["ParameterUser"].ToString() == "@User")
                    sql += " @User='" + userName + "',";

                if (row["ParameterUser"].ToString() == "@CurrentUserCode")
                    sql += " @CurrentUserCode='" + userName + "',";

                if (row["ParameterLang"] != DBNull.Value)
                    sql += " @Lang='" + ((isArabic == true) ? "ar" : "en") + "',";
            }


            foreach (var item in listCriteria)
            {
                sql += " @" + item.Name + "='" + item.Value + "',";
            }
            sql = sql.TrimEnd(',');
            ReportDataSet = DataHelper.ExcuteDataSet(sql);
            if (ReportDataSet == null)
            {
                return null;
            }
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(ReportDataSet);
            //هنا اعمل جسون جديد حط فيه المسار والبيانات
            JObject newJson = new JObject();
            newJson.Add("path", reportPath);
            newJson.Add("fixedDirection", fixedDirection);
            newJson.Add("dataSet", JSONString);
            return newJson;
        }

        public string Print(string ReportId, List<CriteriaField> listCriteria, string userName, bool isArabic, string fileType)
        {

            // MrtDataSet = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(HelperCls.ConnString.ToString(), CommandType.Text, "SELECT TOP 1 * FROM mms_OperationTypesPrintsOut where IsDefault = 1 and OperationTypeID = " & Request.QueryString("OperationTypeID"))
            // ReportDataSet = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(HelperCls.ConnString.ToString(), "mms_Prn_Operations", OperationHeadID, 0)

            MrtDataSet = DataHelper.ExcuteDataSet(@"SELECT code,DataSource AS PrintReportFile,DataSource,ISNULL(P.name,'') ParameterUser
                    ,ReportPath
                    FROM sys_StiReports R
                    LEFT JOIN sys.Objects O ON O.name = R.DataSource
                    LEFT JOIN sys.parameters P  ON O.object_id = P.object_id  AND P.name IN ( '@User','@CurrentUserCode')
                    WHERE R.ID = " + ReportId);
            string sql = @"set dateformat dmy ; Exec " + MrtDataSet.Tables[0].Rows[0]["DataSource"] + " ";
            foreach (DataRow row in MrtDataSet.Tables[0].Rows)
            {
                if (row["ParameterUser"].ToString() == "@User")
                    sql += " @User='" + userName + "',";

                if (row["ParameterUser"].ToString() == "@CurrentUserCode")
                    sql += " @CurrentUserCode='" + userName + "',";
            }


            foreach (var item in listCriteria)
            {
                sql += " @" + item.Name + "='" + item.Value + "',";
            }
            sql = sql.TrimEnd(',');
            ReportDataSet = DataHelper.ExcuteDataSet(sql);
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(ReportDataSet);
            // return null;
            return GetPath(listCriteria, MrtDataSet.Tables[0].Rows[0]["code"].ToString(), userName, isArabic, fileType);
        }

        //public MemoryStream PrintStream(string ReportId, List<CriteriaField> listCriteria, string userName)
        //{

        //    // MrtDataSet = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(HelperCls.ConnString.ToString(), CommandType.Text, "SELECT TOP 1 * FROM mms_OperationTypesPrintsOut where IsDefault = 1 and OperationTypeID = " & Request.QueryString("OperationTypeID"))
        //    // ReportDataSet = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(HelperCls.ConnString.ToString(), "mms_Prn_Operations", OperationHeadID, 0)

        //    MrtDataSet = DataHelper.ExcuteDataSet(@"SELECT code,ReportData AS PrintReportFile,DataSource,ISNULL(P.name,'') ParameterUser
        //            FROM sys_StiReports R
        //            LEFT JOIN sys.Objects O ON O.name = R.DataSource
        //            LEFT JOIN sys.parameters P  ON O.object_id = P.object_id  AND P.name IN ( '@User','@CurrentUserCode')
        //            WHERE R.ID = " + ReportId);
        //    string sql = @"set dateformat dmy ; Exec " + MrtDataSet.Tables[0].Rows[0]["DataSource"] + " ";
        //    foreach (DataRow row in MrtDataSet.Tables[0].Rows)
        //    {
        //        if (row["ParameterUser"].ToString() == "@User")
        //            sql += " @User='" + DataHelper.GetUserName() + "',";

        //        if (row["ParameterUser"].ToString() == "@CurrentUserCode")
        //            sql += " @CurrentUserCode='" + DataHelper.GetUserName() + "',";
        //    }


        //    foreach (var item in listCriteria)
        //    {
        //        sql += " @" + item.Name + "='" + item.Value + "',";
        //    }
        //    sql = sql.TrimEnd(',');
        //    ReportDataSet = DataHelper.ExcuteDataSet(sql);
        //    return GetStream(listCriteria, MrtDataSet.Tables[0].Rows[0]["code"].ToString(), userName);
        //}

        private string GetPath(List<CriteriaField> listCriteria, string reportCode, string userName, bool isArabic, string fileType)
        {

            try
            {
                Stimulsoft.Report.StiReport StiReport1 = new Stimulsoft.Report.StiReport();
                System.IO.Stream strm;
                if (MrtDataSet.Tables[0].Rows != null && MrtDataSet.Tables[0].Rows[0]["PrintReportFile"] != null)
                {
                    //byte[] rByte = (byte[])MrtDataSet.Tables[0].Rows[0]["PrintReportFile"]; // dr("ReportData")
                    //strm = new System.IO.MemoryStream(rByte);
                    //StiReport1.Load(strm);

                    //StiReport1.Load(@"C:\WORK\NewERP\DataOceanCloud-ERP\Venus_ERP_UI\UI\public\reports\Ledgers.mrt");
                    StiReport1.Load(@"C:\Data Ocean\ERPCode\Venus_ERP_UI\UI\public\" + MrtDataSet.Tables[0].Rows[0]["ReportPath"]);

                    List<ArrayList> ListCalColList = new List<ArrayList>();
                    ListCalColList.Clear();
                    int ListID = 0;
                    foreach (StiDataSource SData in StiReport1.Dictionary.DataSources)
                    {
                        ListCalColList.Add(new ArrayList());
                        for (int i = 0; i <= SData.Columns.Count - 1; i++)
                        {
                            object Col = SData.Columns[i];
                            if (Col.GetType().Name == "StiCalcDataColumn")
                                ListCalColList[ListID].Add(Col);
                        }
                        ListID = ListID + 1;
                    }
                    //  ArrayList VariableList = new ArrayList();
                    for (int i = 0; i <= StiReport1.Dictionary.Variables.Count - 1; i++)
                    {
                        object Var = StiReport1.Dictionary.Variables[i];
                        if (Var.GetType().Name == "StiVariable")
                        {
                            if (listCriteria.Exists(x => x.Name == ((StiVariable)Var).Name))
                                ((StiVariable)Var).Value = listCriteria.Find(x => x.Name == ((StiVariable)Var).Name).Value;
                        }
                    }
                    ArrayList RelationList = new ArrayList();
                    for (int r = 0; r <= StiReport1.Dictionary.Relations.Count - 1; r++)
                    {
                        object Rel = StiReport1.Dictionary.Relations[r];
                        if (Rel.GetType().Name == "StiDataRelation")
                            RelationList.Add(Rel);
                    }
                    List<StiDataSource> DataSourcesList = new List<StiDataSource>();
                    for (int i = 0; i <= StiReport1.Dictionary.DataSources.Count - 1; i++)
                    {
                        StiDataSource Var = StiReport1.Dictionary.DataSources[i];
                        DataSourcesList.Add(Var);
                    }
                    List<StiDatabase> DatabasesList = new List<StiDatabase>();
                    for (int i = 0; i <= StiReport1.Dictionary.Databases.Count - 1; i++)
                    {
                        StiDatabase DB = StiReport1.Dictionary.Databases[i];
                        DatabasesList.Add(DB);
                    }

                    StiReport1.Dictionary.Databases.Clear();
                    StiReport1.Dictionary.DataSources.Clear();
                    //StiReport1.Dictionary.Variables.Clear();
                    StiReport1.RegData(ReportDataSet);
                    StiReport1.Dictionary.Synchronize();
                    foreach (ArrayList iLest in ListCalColList)
                    {
                        foreach (Stimulsoft.Report.Dictionary.StiCalcDataColumn CalCol in iLest)
                            StiReport1.Dictionary.DataSources[ListID].Columns.Add(CalCol);
                        ListID = ListID + 1;
                    }


                    // StiReport1.Dictionary.Synchronize();
                    if (isArabic)
                    {
                        SetArabic(StiReport1);
                        SetRTL(StiReport1);

                    }


                    StiReport1.PrinterSettings.ShowDialog = false;

                    StiReport1.PreviewMode = Stimulsoft.Report.StiPreviewMode.DotMatrix;
                    StiReport1.Compile();
                    if (StiReport1.IsCompiled)
                    {
                        StiReport1.Render(true);
                        string guid = Guid.NewGuid().ToString();
                        string localFilePath = Path.Combine(_hostEnvironment.ContentRootPath, @"Reports\");
                        var fileDirectory = Path.Combine(localFilePath + userName);

                        bool exists = System.IO.Directory.Exists(fileDirectory);

                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(fileDirectory);
                        }
                        else
                        {
                            #region//لحذف الملفات القديمة عند حد معين
                            try
                            {

                                string[] files = Directory.GetFiles(fileDirectory);
                                if ((files.Length > 15))
                                {
                                    foreach (var f in files)
                                    {
                                        if (File.Exists(f))
                                            File.Delete(f);
                                    }
                                }
                            }
                            catch (Exception EX)
                            {
                                DataHelper.Log(EX.Message);
                            }
                            #endregion


                        }

                        string reportName = reportCode + "_" + DateTime.Now.ToString("ddMMMMyy hhmmssfff");
                        string fileName = Path.Combine(fileDirectory, reportName + "." + fileType);
                        if (fileType == "pdf")
                        {
                            StiReport1.ExportDocument(StiExportFormat.Pdf, fileName);
                        }
                        else if (fileType == "xlsx")
                        {
                            StiReport1.ExportDocument(StiExportFormat.Excel2007, fileName);
                        }

                        return fileName;
                    }

                }
                DataHelper.Log("Err: StiReport not compiled correctly.");
                return "Err: StiReport not compiled correctly.";
            }
            catch (Exception ex)
            {
                DataHelper.Log("GetPath() :::" + ex.Message);
                return "Err: " + ex.Message;
            }
        }
        //private MemoryStream GetStream(List<CriteriaField> listCriteria, string reportCode, string userName)
        //{
        //    try
        //    {
        //        Stimulsoft.Report.StiReport StiReport1 = new Stimulsoft.Report.StiReport();
        //        System.IO.Stream strm;
        //        if (MrtDataSet.Tables[0].Rows != null && MrtDataSet.Tables[0].Rows[0]["PrintReportFile"] != null)
        //        {
        //            byte[] rByte = (byte[])MrtDataSet.Tables[0].Rows[0]["PrintReportFile"]; // dr("ReportData")
        //            strm = new System.IO.MemoryStream(rByte);

        //            StiReport1.Load(strm);

        //            List<ArrayList> ListCalColList = new List<ArrayList>();
        //            ListCalColList.Clear();
        //            int ListID = 0;
        //            foreach (StiDataSource SData in StiReport1.Dictionary.DataSources)
        //            {
        //                ListCalColList.Add(new ArrayList());
        //                for (int i = 0; i <= SData.Columns.Count - 1; i++)
        //                {
        //                    object Col = SData.Columns[i];
        //                    if (Col.GetType().Name == "StiCalcDataColumn")
        //                        ListCalColList[ListID].Add(Col);
        //                }
        //                ListID = ListID + 1;
        //            }
        //            //  ArrayList VariableList = new ArrayList();
        //            for (int i = 0; i <= StiReport1.Dictionary.Variables.Count - 1; i++)
        //            {
        //                object Var = StiReport1.Dictionary.Variables[i];
        //                if (Var.GetType().Name == "StiVariable")
        //                {
        //                    if (listCriteria.Exists(x => x.Name == ((StiVariable)Var).Name))
        //                        ((StiVariable)Var).Value = listCriteria.Find(x => x.Name == ((StiVariable)Var).Name).Value;
        //                }
        //            }
        //            ArrayList RelationList = new ArrayList();
        //            for (int r = 0; r <= StiReport1.Dictionary.Relations.Count - 1; r++)
        //            {
        //                object Rel = StiReport1.Dictionary.Relations[r];
        //                if (Rel.GetType().Name == "StiDataRelation")
        //                    RelationList.Add(Rel);
        //            }
        //            List<StiDataSource> DataSourcesList = new List<StiDataSource>();
        //            for (int i = 0; i <= StiReport1.Dictionary.DataSources.Count - 1; i++)
        //            {
        //                StiDataSource Var = StiReport1.Dictionary.DataSources[i];
        //                DataSourcesList.Add(Var);
        //            }
        //            List<StiDatabase> DatabasesList = new List<StiDatabase>();
        //            for (int i = 0; i <= StiReport1.Dictionary.Databases.Count - 1; i++)
        //            {
        //                StiDatabase DB = StiReport1.Dictionary.Databases[i];
        //                DatabasesList.Add(DB);
        //            }

        //            StiReport1.Dictionary.Databases.Clear();
        //            StiReport1.Dictionary.DataSources.Clear();
        //            //StiReport1.Dictionary.Variables.Clear();
        //            StiReport1.RegData(ReportDataSet);
        //            StiReport1.Dictionary.Synchronize();
        //            foreach (ArrayList iLest in ListCalColList)
        //            {
        //                foreach (Stimulsoft.Report.Dictionary.StiCalcDataColumn CalCol in iLest)
        //                    StiReport1.Dictionary.DataSources[ListID].Columns.Add(CalCol);
        //                ListID = ListID + 1;
        //            }


        //            StiReport1.Dictionary.Synchronize();

        //            StiReport1.PrinterSettings.ShowDialog = false;

        //            StiReport1.PreviewMode = Stimulsoft.Report.StiPreviewMode.DotMatrix;
        //            StiReport1.Compile();
        //            if (StiReport1.IsCompiled)
        //            {
        //                StiReport1.Render(true);
        //                string guid = Guid.NewGuid().ToString();
        //                string localFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~") + @"Reports\";
        //                var fileDirectory = Path.Combine(localFilePath + userName);

        //                bool exists = System.IO.Directory.Exists(fileDirectory);

        //                if (!exists)
        //                {
        //                    System.IO.Directory.CreateDirectory(fileDirectory);
        //                }
        //                else
        //                {
        //                    #region//لحذف الملفات القديمة عند حد معين
        //                    try
        //                    {

        //                        string[] files = Directory.GetFiles(fileDirectory);
        //                        if ((files.Length > 100))
        //                        {
        //                            foreach (var f in files)
        //                            {
        //                                if (File.Exists(f))
        //                                    File.Delete(f);
        //                            }
        //                        }
        //                    }
        //                    catch (Exception EX)
        //                    {
        //                        DataHelper.Log(EX.Message);
        //                    }
        //                    #endregion


        //                }

        //                string reportName = DateTime.Now.ToString("ddMMMMyy hhmmssfff");
        //                string s = reportCode + "_" + reportName + ".pdf";
        //                var fileName = Path.Combine(fileDirectory, s);
        //                MemoryStream stream = new MemoryStream();
        //                StiReport1.ExportDocument(StiExportFormat.Pdf, stream);

        //                return stream;
        //            }

        //        }
        //        DataHelper.Log("Err: StiReport not compiled correctly.");
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        DataHelper.Log("GetPath() :::" + ex.Message);
        //        throw ex;
        //    }
        //}
        #endregion

        //public MemoryStream GenerateReport(string ReportId, List<CriteriaField> listCriteria, string userName)
        //{
        //    MrtDataSet = DataHelper.ExcuteDataSet(@"SELECT code,ReportData AS PrintReportFile,DataSource,ISNULL(P.name,'') ParameterUser
        //            FROM sys_StiReports R
        //            LEFT JOIN sys.Objects O ON O.name = R.DataSource
        //            LEFT JOIN sys.parameters P  ON O.object_id = P.object_id  AND P.name IN ( '@User','@CurrentUserCode')
        //            WHERE R.ID = " + ReportId);
        //    string sql = @"set dateformat dmy ; Exec " + MrtDataSet.Tables[0].Rows[0]["DataSource"] + " ";
        //    foreach (DataRow row in MrtDataSet.Tables[0].Rows)
        //    {
        //        if (row["ParameterUser"].ToString() == "@User")
        //            sql += " @User='" + DataHelper.GetUserName() + "',";

        //        if (row["ParameterUser"].ToString() == "@CurrentUserCode")
        //            sql += " @CurrentUserCode='" + DataHelper.GetUserName() + "',";
        //    }


        //    foreach (var item in listCriteria)
        //    {
        //        sql += " @" + item.Name + "='" + item.Value + "',";
        //    }
        //    sql = sql.TrimEnd(',');
        //    ReportDataSet = DataHelper.ExcuteDataSet(sql);
        //    return GetReportStream(listCriteria, MrtDataSet.Tables[0].Rows[0]["code"].ToString(), userName);
        //}
        //private MemoryStream GetReportStream(List<CriteriaField> listCriteria, string reportCode, string userName)
        //{
        //    try
        //    {
        //        Stimulsoft.Report.StiReport StiReport1 = new Stimulsoft.Report.StiReport();
        //        System.IO.Stream strm;
        //        if (MrtDataSet.Tables[0].Rows != null && MrtDataSet.Tables[0].Rows[0]["PrintReportFile"] != null)
        //        {
        //            byte[] rByte = (byte[])MrtDataSet.Tables[0].Rows[0]["PrintReportFile"]; // dr("ReportData")
        //            strm = new System.IO.MemoryStream(rByte);

        //            StiReport1.Load(strm);

        //            List<ArrayList> ListCalColList = new List<ArrayList>();
        //            ListCalColList.Clear();
        //            int ListID = 0;
        //            foreach (StiDataSource SData in StiReport1.Dictionary.DataSources)
        //            {
        //                ListCalColList.Add(new ArrayList());
        //                for (int i = 0; i <= SData.Columns.Count - 1; i++)
        //                {
        //                    object Col = SData.Columns[i];
        //                    if (Col.GetType().Name == "StiCalcDataColumn")
        //                        ListCalColList[ListID].Add(Col);
        //                }
        //                ListID = ListID + 1;
        //            }
        //            //  ArrayList VariableList = new ArrayList();
        //            for (int i = 0; i <= StiReport1.Dictionary.Variables.Count - 1; i++)
        //            {
        //                object Var = StiReport1.Dictionary.Variables[i];
        //                if (Var.GetType().Name == "StiVariable")
        //                {
        //                    if (listCriteria.Exists(x => x.Name == ((StiVariable)Var).Name))
        //                        ((StiVariable)Var).Value = listCriteria.Find(x => x.Name == ((StiVariable)Var).Name).Value;
        //                }
        //            }
        //            ArrayList RelationList = new ArrayList();
        //            for (int r = 0; r <= StiReport1.Dictionary.Relations.Count - 1; r++)
        //            {
        //                object Rel = StiReport1.Dictionary.Relations[r];
        //                if (Rel.GetType().Name == "StiDataRelation")
        //                    RelationList.Add(Rel);
        //            }
        //            List<StiDataSource> DataSourcesList = new List<StiDataSource>();
        //            for (int i = 0; i <= StiReport1.Dictionary.DataSources.Count - 1; i++)
        //            {
        //                StiDataSource Var = StiReport1.Dictionary.DataSources[i];
        //                DataSourcesList.Add(Var);
        //            }
        //            List<StiDatabase> DatabasesList = new List<StiDatabase>();
        //            for (int i = 0; i <= StiReport1.Dictionary.Databases.Count - 1; i++)
        //            {
        //                StiDatabase DB = StiReport1.Dictionary.Databases[i];
        //                DatabasesList.Add(DB);
        //            }

        //            StiReport1.Dictionary.Databases.Clear();
        //            StiReport1.Dictionary.DataSources.Clear();
        //            //StiReport1.Dictionary.Variables.Clear();
        //            StiReport1.RegData(ReportDataSet);
        //            StiReport1.Dictionary.Synchronize();
        //            foreach (ArrayList iLest in ListCalColList)
        //            {
        //                foreach (Stimulsoft.Report.Dictionary.StiCalcDataColumn CalCol in iLest)
        //                    StiReport1.Dictionary.DataSources[ListID].Columns.Add(CalCol);
        //                ListID = ListID + 1;
        //            }


        //            StiReport1.Dictionary.Synchronize();

        //            StiReport1.PrinterSettings.ShowDialog = false;

        //            StiReport1.PreviewMode = Stimulsoft.Report.StiPreviewMode.DotMatrix;
        //            StiReport1.Compile();
        //            if (StiReport1.IsCompiled)
        //            {
        //                StiReport1.Render(true);
        //                MemoryStream strm2 = new MemoryStream();
        //                StiReport1.ExportDocument(StiExportFormat.Pdf, strm2);
        //                return strm2;
        //            }
        //            else
        //            {
        //                DataHelper.Log("Err: StiReport not compiled correctly.");
        //                throw new Exception("Err: StiReport not compiled correctly.");
        //            }

        //        }
        //        else
        //        {
        //            DataHelper.Log("Err: PrintReportFile not found.");
        //            throw new Exception("Err: PrintReportFile not found.");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        DataHelper.Log("GetPath() :::" + ex.Message);
        //        throw ex;
        //    }
        //}

        List<String> ListOfArBox = new List<String>();
        private bool SetArabic(StiReport mReport)
        {
            DataTable dtCriteria = new DataTable();

            foreach (StiPage Pag in mReport.Pages)
            {
                foreach (StiComponent Comp in Pag.GetComponents())
                {
                    if (Comp.GetType().Name == "StiText")
                    {
                        StiText Txt = (StiText)Comp;
                        if (ListOfArBox.Contains(Txt.Name) == false)
                        {
                            ListOfArBox.Add(Txt.Name);
                            if (Txt.Alias != "")
                            {
                                Txt.Text = Txt.Alias;
                                //if (ClsAlias.GetValue(Txt.Alias, "ArabicTitle") != "")
                                //    Txt.Text = ClsAlias.GetValue(Txt.Alias, "ArabicTitle");
                            }
                        }
                    }
                }
            }
            return true;
        }
        public bool SetRTL(StiReport mReport)
        {
            List<string> ListOfRTLBox = new List<string>();

            try
            {
                foreach (StiPage Pag in mReport.Pages)
                {
                    foreach (StiComponent Comp in Pag.GetComponents())
                    {
                        try
                        {
                            if (Comp.GetType().Name == "StiText" | Comp.GetType().Name == "StiImage"
                                | Comp.GetType().Name == "StiChart" | Comp.GetType().Name == "StiRichText"
                                | Comp.GetType().Name == "StiBarCode" | Comp.GetType().Name == "StiShape"
                                | Comp.GetType().Name == "StiPanel" | Comp.GetType().Name == "StiCheckBox"
                                | Comp.GetType().Name == "StiRectanglePrimitive" | Comp.GetType().Name == "StiStartPointPrimitive"
                                | Comp.GetType().Name == "StiEndPointPrimitive" | Comp.GetType().Name == "StiRoundedRectanglePrimitive"
                                | Comp.GetType().Name == "StiHorizontalLinePrimitive" | Comp.GetType().Name == "StiVerticalLinePrimitive")
                            {
                                if (ListOfRTLBox.Contains(Comp.Name) == false)
                                {
                                    double OldLeft = Comp.Left;
                                    double OldWidth = Comp.Width;
                                    Comp.Left = Comp.Parent.Width;
                                    Comp.Left -= OldLeft;
                                    Comp.Left = Comp.Left - OldWidth;
                                    ListOfRTLBox.Add(Comp.Name);
                                }
                            }
                            else if (Comp.GetType().Name == "StiCrossTab")
                            {
                                Stimulsoft.Report.CrossTab.StiCrossTab mCromss = (Stimulsoft.Report.CrossTab.StiCrossTab)Comp;
                                mCromss.RightToLeft = true;
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
    public class CriteriaField
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
