using System.Collections.Generic;

namespace LabamotoLaundryShop.Models
{
    public class PricingViewModel
    {
        public IEnumerable<PricingPackage> RegularLaundry { get; set; }
        public IEnumerable<SpecialItem> SpecialItems { get; set; }
        public IEnumerable<DryCleanItem> DryClean { get; set; }
        public IEnumerable<AddOnItem> AddOns { get; set; }

        // Fees & surcharges
        public decimal RushServiceFee { get; set; }
        public decimal PickupFee { get; set; }
        public decimal FreeDeliveryMinimum { get; set; }
        public decimal Discount { get; set; }
        public decimal LoyaltyDiscount { get; set; }
        public decimal VAT { get; set; }
    }

    public class PricingPackage
    {
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public decimal PricePerKg { get; set; }
        public decimal MinimumKg { get; set; }
        public string Status { get; set; }
        public string Unit { get; set; }
        public string Category { get; set; } // ✅ Add this line
    }

    public class SpecialItem
    {
        public int SpecialItemID { get; set; }
        public string ItemName { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public decimal PricePerPiece { get; set; }
        public string ProcessingTime { get; set; }
        public string Status { get; set; }
    }

    public class DryCleanItem
    {
        public int DryCleanItemID { get; set; }
        public string ItemName { get; set; }
        public string Type { get; set; }
        public decimal PricePerPiece { get; set; }
        public string ProcessingTime { get; set; }
        public string Status { get; set; }
    }

    public class AddOnItem
    {
        public int AddOnID { get; set; }
        public string ServiceName { get; set; }
        public string PriceType { get; set; } // NEW
        public string Price { get; set; }
        public string Status { get; set; }
    }
}
