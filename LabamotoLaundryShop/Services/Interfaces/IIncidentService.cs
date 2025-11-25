using LabamotoLaundryShop.Models;
using System;
using System.Collections.Generic;

namespace LabamotoLaundryShop.Services.Interfaces
{
    public interface IIncidentService
    {
        List<Incident> GetIncidents(DateTime startDate, DateTime endDate);
        Incident GetIncidentById(int id);
        void CreateIncident(Incident incident);
        Incident GetIncidentByIdWithDetails(int id);

        // Unified method to update status
        void UpdateIncidentStatus(int id, int staffId, string status);

        Staff GetStaffById(int id);

        void UpdateIncident(Incident incident);
    }
}
