using System.Collections.Generic;
using System.Linq;
using Dapper;
using LabamotoLaundryShop.Data;
using LabamotoLaundryShop.Models;

namespace LabamotoLaundryShop.Repositories.Implementations
{
    public class PricingRepository
    {
        private readonly DapperContext _context;

        public PricingRepository(DapperContext context)
        {
            _context = context;
        }

        // ======================
        // REGULAR LAUNDRY
        // ======================
        public IEnumerable<PricingPackage> GetAllRegularLaundry()
        {
            using (var conn = _context.CreateConnection())
            {
                return conn.Query<PricingPackage>("SELECT * FROM PRICING_PACKAGES ORDER BY PackageID ASC").ToList();
            }
        }

        public void AddRegular(PricingPackage model)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Execute("INSERT INTO PRICING_PACKAGES (PackageName, PricePerKg, MinimumKg, Status) VALUES (@PackageName,@PricePerKg,@MinimumKg, 'Active')", model);
            }
        }

        public void EditRegular(PricingPackage model)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Execute("UPDATE PRICING_PACKAGES SET PackageName=@PackageName, PricePerKg=@PricePerKg, MinimumKg=@MinimumKg, Status=@Status WHERE PackageID=@PackageID", model);
            }
        }

        public void ToggleRegularStatus(int id)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Execute("UPDATE PRICING_PACKAGES SET Status = CASE WHEN Status='Active' THEN 'Inactive' ELSE 'Active' END WHERE PackageID=@id", new { id });
            }
        }

        // ======================
        // SPECIAL ITEMS
        // ======================
        public IEnumerable<SpecialItem> GetAllSpecialItems()
        {
            using (var conn = _context.CreateConnection())
            {
                return conn.Query<SpecialItem>("SELECT * FROM SPECIAL_ITEMS ORDER BY SpecialItemID ASC").ToList();
            }
        }

        public void AddSpecialItem(SpecialItem item)
        {
            var sql = @"INSERT INTO special_items 
                (ItemName, PricePerPiece, Category, ProcessingTime, Status)
                VALUES (@ItemName, @PricePerPiece, @Category, @ProcessingTime, 'Active')";
            using (var connection = _context.CreateConnection())
            {
                connection.Execute(sql, item);
            }
        }

        public void UpdateSpecialItem(SpecialItem model)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Execute(@"UPDATE SPECIAL_ITEMS SET ItemName=@ItemName, PricePerPiece=@PricePerPiece, Category=@Category, ProcessingTime=@ProcessingTime, Status=@Status WHERE SpecialItemID=@SpecialItemID", model);
            }
        }

        public void DeleteSpecialItem(int id)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Execute("DELETE FROM SPECIAL_ITEMS WHERE SpecialItemID=@id", new { id });
            }
        }

        // ======================
        // DRY CLEAN
        // ======================
        public IEnumerable<DryCleanItem> GetAllDryClean()
        {
            using (var conn = _context.CreateConnection())
            {
                return conn.Query<DryCleanItem>("SELECT * FROM DRYCLEAN_ITEMS ORDER BY DryCleanItemID ASC").ToList();
            }
        }

        public void AddDryCleanItem(DryCleanItem item)
        {
            var sql = @"INSERT INTO dryclean_items 
                (ItemName, PricePerPiece, ProcessingTime, Status)
                VALUES (@ItemName, @PricePerPiece, @ProcessingTime, 'Active')";
            using (var connection = _context.CreateConnection())
            {
                connection.Execute(sql, item);
            }
        }

        public void UpdateDryClean(DryCleanItem model)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Execute(@"UPDATE DRYCLEAN_ITEMS SET ItemName=@ItemName, PricePerPiece=@PricePerPiece, ProcessingTime=@ProcessingTime, Status=@Status WHERE DryCleanItemID=@DryCleanItemID", model);
            }
        }

        // ======================
        // ADD-ONS
        // ======================
        public IEnumerable<AddOnItem> GetAllAddOns()
        {
            using (var conn = _context.CreateConnection())
            {
                return conn.Query<AddOnItem>("SELECT * FROM ADDON_SERVICES ORDER BY AddOnID ASC").ToList();
            }
        }

        public void AddAddOn(AddOnItem item)
        {
            var sql = @"INSERT INTO addon_services 
                (ServiceName, Price, Status, PriceType)
                VALUES (@ServiceName, @Price, 'Active', @PriceType)";
            using (var connection = _context.CreateConnection())
            {
                connection.Execute(sql, item);
            }
        }

        public void UpdateAddOn(AddOnItem model)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Execute("UPDATE ADDON_SERVICES SET ServiceName=@ServiceName, Price=@Price, Status=@Status WHERE AddOnID=@AddOnID", model);
            }
        }

        // ======================
        // FEES / SURCHARGES
        // ======================
        public decimal GetFee(string feeName)
        {
            using (var conn = _context.CreateConnection())
            {
                var result = conn.QueryFirstOrDefault<string>("SELECT SettingValue FROM BUSINESS_SETTINGS WHERE SettingKey=@feeName", new { feeName });
                return decimal.TryParse(result, out var value) ? value : 0;
            }
        }

        public void UpdateFee(string feeName, decimal amount)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Execute("UPDATE BUSINESS_SETTINGS SET SettingValue=@amount WHERE SettingKey=@feeName", new { feeName, amount });
            }
        }

        public IEnumerable<PricingPackage> GetAllPricing()
        {
            var allPricing = new List<PricingPackage>();

            // Regular Laundry
            allPricing.AddRange(GetAllRegularLaundry());

            // Special Items
            allPricing.AddRange(GetAllSpecialItems().Select(x => new PricingPackage
            {
                PackageName = x.ItemName,
                PricePerKg = x.PricePerPiece, // map PricePerPiece to PricePerKg
                MinimumKg = 0, // not applicable
                Status = x.Status,
                Unit = "piece"
            }));

            // Dry Clean Items
            allPricing.AddRange(GetAllDryClean().Select(x => new PricingPackage
            {
                PackageName = x.ItemName,
                PricePerKg = x.PricePerPiece, // map PricePerPiece to PricePerKg
                MinimumKg = 0,
                Status = x.Status,
                Unit = "piece"
            }));

            // Add-Ons
            allPricing.AddRange(GetAllAddOns().Select(x => new PricingPackage
            {
                PackageName = x.ServiceName,
                PricePerKg = decimal.TryParse(x.Price, out decimal price) ? price : 0, // convert string to decimal
                MinimumKg = 0,
                Status = x.Status,
                Unit = "unit"
            }));

            return allPricing;
        }

        public void AddOrUpdatePricing(PricingPackage item)
        {
            var existing = GetAllRegularLaundry().FirstOrDefault(p => p.PackageName == item.PackageName);
            if (existing != null)
            {
                existing.PricePerKg = item.PricePerKg;
                existing.MinimumKg = item.MinimumKg;
                existing.Status = item.Status;
                existing.Unit = item.Unit;
                EditRegular(existing); // reuse your existing update method
            }
            else
            {
                AddRegular(item); // reuse your existing add method
            }
        }

        // ==========================
        // GET REGULAR LAUNDRY
        // ==========================
        public IEnumerable<PricingPackage> GetRegularLaundry()
        {
            var query = "SELECT * FROM PRICING_PACKAGES";
            using (var connection = _context.CreateConnection())
            {
                return connection.Query<PricingPackage>(query).ToList();
            }
        }

        // ==========================
        // GET SPECIAL ITEMS
        // ==========================
        public IEnumerable<SpecialItem> GetSpecialItems()
        {
            var query = "SELECT * FROM SPECIAL_ITEMS";
            using (var connection = _context.CreateConnection())
            {
                return connection.Query<SpecialItem>(query).ToList();
            }
        }

        // ==========================
        // GET DRY CLEAN ITEMS
        // ==========================
        public IEnumerable<DryCleanItem> GetDryCleanItems()
        {
            var query = "SELECT * FROM DRYCLEAN_ITEMS";
            using (var connection = _context.CreateConnection())
            {
                return connection.Query<DryCleanItem>(query).ToList();
            }
        }

        // ==========================
        // GET ADD-ONS
        // ==========================
        public IEnumerable<AddOnItem> GetAddOns()
        {
            var query = "SELECT * FROM ADDON_SERVICES";
            using (var connection = _context.CreateConnection())
            {
                return connection.Query<AddOnItem>(query).ToList();
            }
        }


    }
}
