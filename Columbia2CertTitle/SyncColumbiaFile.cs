using System;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Columbia2CertTitle
{
    public class SyncColumbiaFile
    {
        static string ColumbiaServerPath = @"$/Certification/TEAM/Ace_PjM/ExamSchedules/China/Columbia/Columbia_Tracking.xlsx";
        public static string ColumbiaClientPath = @"C:\CertTFS\Certification\TEAM\Ace_PjM\ExamSchedules\China\Columbia\Sync\Columbia_Tracking.xlsx";

        /// <summary>
        /// sync server file to local
        /// </summary>
        public static void SyncFile()
        {
            string tfs = @"https://vstf-us-wa-11.partners.extranet.microsoft.com:8443/tfs/MSLearning";
            TfsTeamProjectCollection tfsServer = new TfsTeamProjectCollection (new Uri(tfs));
            VersionControlServer controlServer= tfsServer.GetService<VersionControlServer>();

            string workSpaceTmpName = System.DateTime.UtcNow.ToString("MMddyyHHmmss");
            Workspace tfsWS = controlServer.CreateWorkspace(workSpaceTmpName, "fareast\\v-qixue");
            tfsWS.Map(ColumbiaServerPath,ColumbiaClientPath);

            bool isColumbiaFileExist = controlServer.ServerItemExists(ColumbiaServerPath,ItemType.File);
            if(isColumbiaFileExist)
            {
                //download latest
                GetStatus status = tfsWS.Get(new string[] {ColumbiaServerPath }, VersionSpec.Latest, RecursionType.Full, GetOptions.Overwrite);
            }

            tfsWS.Delete();
        }
    }
}
