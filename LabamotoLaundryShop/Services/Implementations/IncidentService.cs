using LabamotoLaundryShop.Data;
using LabamotoLaundryShop.Models;
using LabamotoLaundryShop.Services.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LabamotoLaundryShop.Services.Implementations
{
    public class IncidentService : IIncidentService
    {
        private readonly DapperContext _context;

        public IncidentService(DapperContext context)
        {
            _context = context;
        }

        public List<Incident> GetIncidents(DateTime startDate, DateTime endDate)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                SELECT i.*, o.CustomerID, c.FullName AS CustomerName
                FROM incidents i
                JOIN orders o ON i.OrderID = o.OrderID
                JOIN customers c ON o.CustomerID = c.CustomerID
                WHERE DATE(i.ReportedDate) BETWEEN @StartDate AND @EndDate
                ORDER BY i.ReportedDate DESC;
            ";
                return connection.Query<Incident>(sql, new { StartDate = startDate, EndDate = endDate }).ToList();
            }
        }

        public Incident GetIncidentById(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                SELECT i.*, o.CustomerID, c.FullName AS CustomerName
                FROM incidents i
                JOIN orders o ON i.OrderID = o.OrderID
                JOIN customers c ON o.CustomerID = c.CustomerID
                WHERE i.IncidentID = @Id;
            ";
                return connection.QueryFirstOrDefault<Incident>(sql, new { Id = id });
            }
        }

        public void CreateIncident(Incident incident)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                INSERT INTO incidents
                (OrderID, IncidentType, Severity, Description, ReportedByStaffID, ReportedDate, Status, IssueSummary, EstimatedItemValue)
                VALUES
                (@OrderID, @IncidentType, @Severity, @Description, @ReportedByStaffID, @ReportedDate, @Status, @IssueSummary, @EstimatedItemValue);
            ";
                incident.Status = "Open";
                incident.ReportedDate = DateTime.Now;
                connection.Execute(sql, incident);
            }
        }

        public Incident GetIncidentByIdWithDetails(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                SELECT i.*, o.CustomerID, c.FullName AS CustomerName, s.FullName AS StaffName
                FROM incidents i
                JOIN orders o ON i.OrderID = o.OrderID
                JOIN customers c ON o.CustomerID = c.CustomerID
                LEFT JOIN staff s ON i.ReportedByStaffID = s.StaffID
                WHERE i.IncidentID = @Id
            ";
                return connection.QueryFirstOrDefault<Incident>(sql, new { Id = id });
            }
        }

        // ✅ New unified method for status update
        public void UpdateIncidentStatus(int id, int staffId, string status)
        {
            using (var connection = _context.CreateConnection())
            {
                string sql;
                if (status == "Resolved")
                {
                    sql = @"
                UPDATE incidents
                SET Status = @Status,
                    ResolvedByStaffID = @StaffID
                WHERE IncidentID = @Id;
            ";
                    connection.Execute(sql, new { Status = status, StaffID = staffId, Id = id });
                }
                else
                {
                    sql = @"
                UPDATE incidents
                SET Status = @Status
                WHERE IncidentID = @Id;
            ";
                    connection.Execute(sql, new { Status = status, Id = id });
                }
            }
        }

        public Staff GetStaffById(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = "SELECT StaffID, FullName FROM staff WHERE StaffID = @Id";
                return connection.QueryFirstOrDefault<Staff>(sql, new { Id = id });
            }
        }

        public void UpdateIncident(Incident incident)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
            UPDATE incidents
            SET 
                Status = @Status,
                Resolution = @Resolution,
                ResolutionDetails = @ResolutionDetails,
                ActionTaken = @ActionTaken,
                Satisfaction = @Satisfaction,
                InternalNotes = @InternalNotes,
                Compensation = @Compensation,
                ResolvedByStaffID = @ResolvedByStaffID,
                ResolvedDate = @ResolvedDate
            WHERE IncidentID = @IncidentID;
        ";

                connection.Execute(sql, new
                {
                    Status = incident.Status,
                    Resolution = incident.Resolution,
                    ResolutionDetails = incident.ResolutionDetails,
                    ActionTaken = incident.ActionTaken,
                    Satisfaction = incident.Satisfaction,
                    InternalNotes = incident.InternalNotes,
                    Compensation = incident.Compensation,
                    ResolvedByStaffID = incident.ResolvedByStaffID,
                    ResolvedDate = incident.ResolvedDate,
                    IncidentID = incident.IncidentID
                });
            }
        }

    }
}
