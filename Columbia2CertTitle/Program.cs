using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace Columbia2CertTitle
{
    class Program
    {
        static void Main(string[] args)
        {
            //Sync Columbia File from TFS to Local
            SyncColumbiaFile.SyncFile();
            string Columbia_Excel = SyncColumbiaFile.ColumbiaClientPath;
            DataTable scheduled = UpdateDataFromColumbia.ReadExcelAsOleDB(Columbia_Excel, "Schedule");
            //get all updates for recent two days
            DataTable recentUpdated = UpdateDataFromColumbia.RecentUpdate(scheduled, -2);
            UpdateDataFromColumbia.UpdateCertTitle(recentUpdated);
            Console.WriteLine("update completed");
            //Console.ReadKey();
        }
       
    }
}
