using System;
using System.Collections.Generic;
using Invoice_printer.Models;

namespace Invoice_printer.DTO_S
{
    public class ReceiptPrintViewModel
    {
        // Logo
        public string LogoUrl { get; set; } = "";

        // Company (User)
        public string CompanyName { get; set; } = "";
        public string CompanyAddres { get; set; } = "";
        public string WebsiteUrl { get; set; } = "";

        // CompanyProfile
        public string Phone { get; set; } = "";
        public string TaxNo { get; set; } = "";
        public string Currency { get; set; } = "TRY";

        // Receipt
        public string ReceiptNo { get; set; } = "";
        public DateTime Date { get; set; }
        public string Type { get; set; } = "";
        public string Status { get; set; } = "";
        public string PartyName { get; set; } = "";
        public string Description { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public string SignatureName { get; set; } = "";

        // Items
        public List<ReceiptItem> Items { get; set; } = new();
        public decimal Total { get; set; }
        public string TotalInWords { get; set; } = "";
        public string QrCodeDataUrl { get; set; } = ""; 
        public string VerifyUrl { get; set; } = "";
    }
}
