using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabamotoLaundryShop.Models
{
    public class Incident
    {
        public int IncidentID { get; set; }
        public int? OrderID { get; set; }
        public string IncidentType { get; set; }
        public string CustomerName { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
        public int? ReportedByStaffID { get; set; }
        public DateTime ReportedDate { get; set; }
        public string Status { get; set; }
        public int? ResolvedByStaffID { get; set; }

        public string ResolutionType { get; set; }    // new
        public string ResolutionDetails { get; set; } // new
        public string ActionTaken { get; set; }       // new
        public string Satisfaction { get; set; }      // new
        public string InternalNotes { get; set; }     // new

        public string Resolution { get; set; }

        public decimal Compensation { get; set; } = 0;
        public DateTime ReportDate => ReportedDate;

        public string StaffName { get; set; }
        public string IssueSummary { get; set; }
        public decimal EstimatedItemValue { get; set; } = 0;

        public string ReportedByName { get; set; }
        public string ResolvedByName { get; set; }

        public DateTime? ResolvedDate { get; set; }
    }
}
