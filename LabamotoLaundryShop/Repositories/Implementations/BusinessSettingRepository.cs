using Dapper;
using LabamotoLaundryShop.Data;
using LabamotoLaundryShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LabamotoLaundryShop.Repositories.Implementations
{
    public class BusinessSettingsRepository
    {
        private readonly DapperContext _context;

        public BusinessSettingsRepository(DapperContext context)
        {
            _context = context;
        }

        public BusinessSettingsViewModel GetSettings()
        {
            using (var connection = _context.CreateConnection())
            {
                return new BusinessSettingsViewModel
                {
                    GeneralInfo = connection.Query<GeneralInfo>("SELECT * FROM GeneralInfo ORDER BY Id").ToList(),
                    BusinessHours = connection.Query<BusinessHour>("SELECT * FROM BusinessHours ORDER BY Id").ToList(),
                    Holidays = connection.Query<Holiday>("SELECT * FROM Holidays ORDER BY Date").ToList(),
                    PaymentMethods = connection.Query<PaymentMethod>("SELECT * FROM PaymentMethods ORDER BY Id").ToList(),
                    SystemConfig = connection.Query<SystemConfig>("SELECT * FROM SystemConfig ORDER BY Id").ToList()
                };
            }
        }

        public void SaveGeneralInfo(List<GeneralInfo> items)
        {
            using (var connection = _context.CreateConnection())
            {
                foreach (var item in items)
                {
                    var existing = connection.QueryFirstOrDefault<GeneralInfo>("SELECT * FROM GeneralInfo WHERE Id=@Id", new { item.Id });
                    if (existing != null)
                        connection.Execute("UPDATE GeneralInfo SET SettingKey=@SettingKey, SettingValue=@SettingValue WHERE Id=@Id", item);
                    else
                        connection.Execute("INSERT INTO GeneralInfo (SettingKey, SettingValue) VALUES (@SettingKey, @SettingValue)", item);
                }
            }
        }

        public void SaveBusinessHours(List<BusinessHour> items)
        {
            using (var connection = _context.CreateConnection())
            {
                foreach (var item in items)
                {
                    var existing = connection.QueryFirstOrDefault<BusinessHour>("SELECT * FROM BusinessHours WHERE Id=@Id", new { item.Id });
                    if (existing != null)
                        connection.Execute("UPDATE BusinessHours SET Day=@Day, OpenTime=@OpenTime, CloseTime=@CloseTime, Status=@Status WHERE Id=@Id", item);
                    else
                        connection.Execute("INSERT INTO BusinessHours (Day, OpenTime, CloseTime, Status) VALUES (@Day, @OpenTime, @CloseTime, @Status)", item);
                }
            }
        }

        public void SaveHolidays(List<Holiday> items)
        {
            using (var connection = _context.CreateConnection())
            {
                foreach (var item in items)
                {
                    if (item.Id == 0)
                        connection.Execute("INSERT INTO Holidays (Date, Description) VALUES (@Date, @Description)", item);
                    else
                        connection.Execute("UPDATE Holidays SET Date=@Date, Description=@Description WHERE Id=@Id", item);
                }
            }
        }

        public void SavePaymentMethods(List<PaymentMethod> items)
        {
            using (var connection = _context.CreateConnection())
            {
                foreach (var item in items)
                {
                    var existing = connection.QueryFirstOrDefault<PaymentMethod>("SELECT * FROM PaymentMethods WHERE Id=@Id", new { item.Id });
                    if (existing != null)
                        connection.Execute("UPDATE PaymentMethods SET Name=@Name, Enabled=@Enabled, Details=@Details WHERE Id=@Id", item);
                    else
                        connection.Execute("INSERT INTO PaymentMethods (Name, Enabled, Details) VALUES (@Name, @Enabled, @Details)", item);
                }
            }
        }

        public void SaveSystemConfig(List<SystemConfig> items)
        {
            using (var connection = _context.CreateConnection())
            {
                foreach (var item in items)
                {
                    var existing = connection.QueryFirstOrDefault<SystemConfig>("SELECT * FROM SystemConfig WHERE Id=@Id", new { item.Id });
                    if (existing != null)
                        connection.Execute("UPDATE SystemConfig SET SettingKey=@SettingKey, SettingValue=@SettingValue WHERE Id=@Id", item);
                    else
                        connection.Execute("INSERT INTO SystemConfig (SettingKey, SettingValue) VALUES (@SettingKey, @SettingValue)", item);
                }
            }
        }

        // ========================
        // GET ORDERS FOR CSV EXPORT
        // ========================
        public List<Order> GetOrders()
        {
            using (var connection = _context.CreateConnection())
            {
                return connection.Query<Order>(
                    "SELECT OrderID, OrderNumber, CustomerID, OrderDate, TotalAmount FROM Orders ORDER BY OrderID"
                ).ToList();
            }
        }

        // ========================
        // INSERT ORDER FROM CSV
        // ========================
        public void InsertOrderFromCSV(string[] values)
        {
            using (var connection = _context.CreateConnection())
            {
                // Example CSV format: OrderNumber,CustomerID,OrderDate,TotalAmount
                connection.Execute(
                    @"INSERT INTO Orders (OrderNumber, CustomerID, OrderDate, TotalAmount) 
              VALUES (@OrderNumber, @CustomerID, @OrderDate, @TotalAmount)",
                    new
                    {
                        OrderNumber = values[0],
                        CustomerID = int.Parse(values[1]),
                        OrderDate = DateTime.Parse(values[2]),
                        TotalAmount = decimal.Parse(values[3])
                    });
            }
        }

        // ========================
        // CREATE BACKUP
        // ========================
        public string CreateBackup()
        {
            var dbFileName = $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
            var backupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups", dbFileName);
            Directory.CreateDirectory(Path.GetDirectoryName(backupPath));

            using (var connection = _context.CreateConnection())
            {
                var tables = connection.Query<string>("SHOW TABLES").ToList();
                using (var writer = new StreamWriter(backupPath))
                {
                    foreach (var table in tables)
                    {
                        var createSql = connection.QueryFirstOrDefault<string>($"SHOW CREATE TABLE {table}");
                        var createStatement = createSql.Split(new[] { "CREATE TABLE" }, StringSplitOptions.None)[1];
                        writer.WriteLine($"CREATE TABLE {table} {createStatement};");

                        var rows = connection.Query($"SELECT * FROM {table}").ToList();
                        foreach (var row in rows)
                        {
                            var dict = (IDictionary<string, object>)row;
                            var columns = string.Join(",", dict.Keys);
                            var values = string.Join(",", dict.Values.Select(v => $"'{v?.ToString().Replace("'", "''")}'"));
                            writer.WriteLine($"INSERT INTO {table} ({columns}) VALUES ({values});");
                        }
                    }
                }
            }

            return backupPath;
        }
    }
}
