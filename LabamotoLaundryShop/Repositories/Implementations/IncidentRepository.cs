using Dapper;
using LabamotoLaundryShop.Data;
using LabamotoLaundryShop.Models;
using LabamotoLaundryShop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LabamotoLaundryShop.Repositories.Implementations
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly DapperContext _context;

        public IncidentRepository(DapperContext context)
        {
            _context = context;
        }

        public List<Incident> GetIncidents(DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = "SELECT * FROM incidents WHERE 1=1";

                if (startDate.HasValue)
                    sql += " AND ReportedDate >= @StartDate";
                if (endDate.HasValue)
                    sql += " AND ReportedDate <= @EndDate";

                return connection.Query<Incident>(sql, new { StartDate = startDate, EndDate = endDate }).ToList();
            }
        }

        public Incident GetIncidentById(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                return connection.QueryFirstOrDefault<Incident>(
                "SELECT * FROM incidents WHERE IncidentID = @Id", new { Id = id });
            }
        }

        public void CreateIncident(Incident incident)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"INSERT INTO incidents
                    (OrderID, IncidentType, Severity, Description, ReportedByStaffID, ReportedDate, Status, IssueSummary, EstimatedItemValue)
                    VALUES (@OrderID, @IncidentType, @Severity, @Description, @ReportedByStaffID, @ReportedDate, @Status, @IssueSummary, @EstimatedItemValue)";

                incident.Status = "Open";
                incident.ReportedDate = DateTime.Now;

                connection.Execute(sql, incident);
            }
        }

        
    }
}
