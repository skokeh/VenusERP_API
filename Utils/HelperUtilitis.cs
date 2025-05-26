using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace VenusERP_API.Utils
{
    public static class HelperUtilitis
    {
        static void DeleteOldAttachments(long? formId, long transactionId, JToken[] attachments)
        {
            JObject file;

            List<long> exciptID = new List<long>();
            for (int i = 0; i < attachments.Length; i++)
            {
                file = (Newtonsoft.Json.Linq.JObject)attachments[i];
                if (file["data"] == null)
                    exciptID.Add(Convert.ToInt64(file["id"]));
            }

            string sqlDelete = $@"delete sys_FormAttachments WHERE FormID= {formId} AND TransactionId={transactionId}";
            if (exciptID.Count > 0)
                sqlDelete += $" AND ID NOT IN ({string.Join(", ", exciptID)})";

            DataHelper.ExecNonQuery(sqlDelete, CommandType.Text);
        }
        public static void saveAttachments(long? formId, long transactionId, JToken[] attachments)
        {
            //DeleteOldAttachments(formId, transactionId, attachments);

            JObject file;
            List<SqlParameter> listAttachments;

            for (int i = 0; i < attachments.Length; i++)
            {

                file = (Newtonsoft.Json.Linq.JObject)attachments[i];

                if (file["data"] == null)//skip untouched  file
                    continue;

                listAttachments = new List<SqlParameter>();
                listAttachments.Add(new SqlParameter("@User", "sa"));
                listAttachments.Add(new SqlParameter("@FormId", formId));
                listAttachments.Add(new SqlParameter("@TransactionId", transactionId));
                listAttachments.Add(new SqlParameter("@FileName", file["path"].ToString()));
                listAttachments.Add(new SqlParameter("@Size", file["size"].ToString()));
                listAttachments.Add(new SqlParameter("@type", file["type"].ToString()));

                byte[] imageData = Convert.FromBase64String(file["data"].ToString().Substring(("data:" + file["type"] + ";base64,").Length));
                listAttachments.Add(new SqlParameter("@AttachedFile", SqlDbType.Binary, imageData.Length) { Value = imageData });
                bool x = DataHelper.ExecNonQuery("sys_SaveFormAttachments", CommandType.StoredProcedure, listAttachments.ToArray());

            }
        }
        public static void saveNotes(long? formId, long? transactionId, string? userCode, JToken[] formNotes)
        {
            // Convert the JToken array to a JSON string
            string formNotesJson = JsonConvert.SerializeObject(new { formNotes });

            // Prepare the SQL parameters
            List<SqlParameter> listFormNotes = new List<SqlParameter>
     {
         new SqlParameter("@User", userCode),
         new SqlParameter("@FormId", formId),
         new SqlParameter("@TransactionId", transactionId),
         new SqlParameter("@Json", formNotesJson) { Size = -1 }
     };

            // Execute the stored procedure
            DataHelper.ExecScalar("sys_FormNotesSave", System.Data.CommandType.StoredProcedure, listFormNotes.ToArray());
        }
        public static DataTable ImportFromXls(string filePath)
        {
            //string filePath = @"C:\WORK\NewERP\VenusAPI\API\Reports\sa\imortLedgers.xlsx";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            String constring = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties=""Excel 12.0 Xml;HDR=YES;""";
            using (OleDbConnection con = new OleDbConnection(constring + ""))
            {
                con.Open();
                string myTableName = con.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();
                String sqlquery = String.Format("SELECT * FROM [{0}] where LedgerCode <> ''", myTableName);// "Select * From " & myTableName  
                var da = new OleDbDataAdapter(sqlquery, con);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            return dt;
        }

    }
}
