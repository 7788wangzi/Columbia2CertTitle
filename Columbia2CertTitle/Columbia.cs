using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Columbia2CertTitle
{
    /// <summary>
    /// this is to map the column name in Columbia.xslt
    /// </summary>
    public class Columbia
    {
        public string ChangeType {
            get
            {
                return "Change Type";
            }
            private set { }
        }

        public string LastUpdated
        {
            get
            {
                return "Last Updated";
            }
            private set { }
        }

        public string Exam
        {
            get
            {
                return "Exam #";
            }
            private set { }
        }

        public string Version
        {
            get
            {
                return "v#";
            }
            private set { }
        }

        public string Title
        {
            get
            {
                return "Title";
            }
            private set { }
        }

        public string PjM
        {
            get
            {
                return "PjM";
            }
            private set { }
        }

        public string CDM
        {
            get
            {
                return "CDM";
            }
            private set { }
        }

        public string Language
        {
            get
            {
                return "Language";
            }
            private set { }
        }

        public string FileETA
        {
            get
            {
                return "File ETA";
            }
            private set { }
        }

        public string RequireHandofftoEDP
        {
            get
            {
                return "Required Handoff to EDP";
            }
            private set { }
        }

        public string FFSD
        {
            get
            {
                return "FFSD (Prometric target live)";
            }
            private set { }
        }
       
    }
}
