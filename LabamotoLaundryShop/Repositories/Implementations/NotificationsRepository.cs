using Dapper;
using LabamotoLaundryShop.Data;
using LabamotoLaundryShop.Models;
using System.Collections.Generic;
using System.Linq;

public class NotificationRepository
{
    private readonly DapperContext _context;

    public NotificationRepository(DapperContext context)
    {
        _context = context;
    }

    // ============================
    // GET ALL SMS TEMPLATES
    // ============================
    public IEnumerable<NotificationTemplate> GetAllTemplates()
    {
        using (var connection = _context.CreateConnection())
        {
            var sql = "SELECT * FROM notification_templates ORDER BY NotificationTemplateID";
            return connection.Query<NotificationTemplate>(sql).ToList();
        }
    }

    // ============================
    // GET MAIN NOTIFICATION SETTINGS
    // ============================
    public NotificationSetting GetSettings()
    {
        using (var connection = _context.CreateConnection())
        {
            var sql = "SELECT * FROM notification_settings LIMIT 1";
            return connection.QueryFirstOrDefault<NotificationSetting>(sql)
                   ?? new NotificationSetting();
        }
    }

    // ============================
    // ADD SMS CREDITS
    // ============================
    public void AddSmsCredits(int credits)
    {
        using (var connection = _context.CreateConnection())
        {
            string query = @"
                UPDATE notification_settings 
                SET SmsCredits = SmsCredits + @credits
                LIMIT 1;
            ";

            connection.Execute(query, new { credits });
        }
    }
}
