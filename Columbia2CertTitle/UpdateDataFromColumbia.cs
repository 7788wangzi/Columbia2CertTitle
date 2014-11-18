using System;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace Columbia2CertTitle
{
    public class UpdateDataFromColumbia
    {
        public static Columbia columbiaField = new Columbia();

        // get all data in schedule
        public static DataTable ReadExcelAsOleDB(string fileName, string sheetName)
        {
            DataTable dataTableResult = new DataTable();
            string conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            string query = "select * from [Sheet1$]";
            if (!string.IsNullOrEmpty(sheetName))
            {
                query = string.Format("select * from [{0}$]", sheetName);
            }


            using (OleDbConnection con = new OleDbConnection(conString))
            {
                con.Open();
                OleDbCommand cmd = new OleDbCommand(query, con);
                OleDbDataAdapter oda = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();
                try
                {
                    oda.Fill(ds);
                    dataTableResult = ds.Tables[0];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(@"Error occur when fill datatable - {0}", ex.Message));
                }
            }

            return dataTableResult;
        }

        // get recent updated data
        public static DataTable RecentUpdate(DataTable dt, int syncDays)
        {            
            DataTable dataTableResult = new DataTable();

            // clone heading
            for (int c = 0; c < dt.Columns.Count; c++)
            {
                dataTableResult.Columns.Add(new DataColumn(dt.Columns[c].ColumnName));
            }

            DateTime syncStart = new DateTime ();
            if (syncDays >= 0)
            {
                syncStart = DateTime.UtcNow.AddDays(-1);
            }
            else
            {
                syncStart = DateTime.UtcNow.AddDays(syncDays);
            }

            DateTime ? entryUpdate = new DateTime ();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                entryUpdate = ConvertDate(dt.Rows[i][columbiaField.LastUpdated].ToString());
                if (entryUpdate >= syncStart)
                {
                    CloneNewRow(dataTableResult, dt.Rows[i]);
                }
            }

            return dataTableResult;

        }

        private static void CloneNewRow(DataTable dt, DataRow dr)
        {
            DataRow newDR = dt.NewRow();
            for(int i = 0; i<dt.Columns.Count;i++)
            {
                newDR[i]=dr[i];                
            }
            dt.Rows.Add (newDR);
        }

        public static void UpdateCertTitle(DataTable dt)
        {
            if (dt.Rows.Count == 0)
                return;
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool isInCertDB = IsExistInCertTitle(dt.Rows[i][columbiaField.Exam].ToString(), dt.Rows[i][columbiaField.Version].ToString());

                // new data
                if (!isInCertDB)
                {
                    if (dt.Rows[i][columbiaField.ChangeType].ToString().ToUpper() == "delete".ToUpper())
                        continue;

                    // insert to cert title database
                    ExamEntry newExam = new ExamEntry()
                    {
                        ExamID=dt.Rows[i][columbiaField.Exam].ToString(),
                        Version =dt.Rows[i][columbiaField.Version].ToString(),
                        Title =dt.Rows[i][columbiaField.Title].ToString(),
                        Language=dt.Rows[i][columbiaField.Language].ToString(),
                        PjM =dt.Rows[i][columbiaField.PjM].ToString(),
                        CDM=dt.Rows[i][columbiaField.CDM].ToString(),
                        PlanFinishDate =ConvertDate(dt.Rows[i][columbiaField.RequireHandofftoEDP].ToString()),
                        Status=0,
                        CreatedDate =System.DateTime.UtcNow,
                        IssueCount=0,
                        IssueCountToEDP=0
                    };

                    // end of the day
                    newExam.PlanFinishDate = newExam.PlanFinishDate.Value.AddHours(23);

                    InsertToCertTitle(newExam);                   
                }
                
                // exist data
                if (isInCertDB)
                {
                    ExamEntry currentExam = new ExamEntry();
                    // update to cert title database
                    using (var db = new CertTitleEntities())
                    {
                        string exam = dt.Rows[i][columbiaField.Exam].ToString();
                        string version = dt.Rows[i][columbiaField.Version].ToString();
                        var item = db.ExamEntries.Where(e => e.ExamID == exam && e.Version == version).FirstOrDefault();
                        if (item != null)
                            currentExam = item;
                    }
                    currentExam.Title = dt.Rows[i][columbiaField.Title].ToString();
                    currentExam.PjM = dt.Rows[i][columbiaField.PjM].ToString();
                    currentExam.PlanFinishDate = ConvertDate(dt.Rows[i][columbiaField.RequireHandofftoEDP].ToString());
                    currentExam.PlanFinishDate = currentExam.PlanFinishDate.Value.AddHours(23);
                    UpdateCertTitle(currentExam);
                }

            }
            
        }

        private static bool IsExistInCertTitle(string exam, string version)
        {
            bool exist = false;
            using (var db = new CertTitleEntities())
            {
                var item = db.ExamEntries.Where(e => e.ExamID == exam && e.Version == version).FirstOrDefault();
                if (item != null)
                    exist = true;
            }
            return exist;
        }

        private static void InsertToCertTitle(ExamEntry exam)
        {
            using (var db = new CertTitleEntities())
            {
                db.ExamEntries.Add(exam);
                db.SaveChanges();
            }
        }

        private static void UpdateCertTitle(ExamEntry exam)
        {
            using (var db = new CertTitleEntities())
            {
                db.Entry(exam).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        private static DateTime ConvertDate(string dateFromFile)
        {
            try
            {
                return DateTime.Parse(dateFromFile);
            }
            catch
            {
                return new DateTime(2000, 1, 1);
            }
        }
    }
}
