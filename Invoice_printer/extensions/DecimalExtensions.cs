using Utils;

namespace Invoice_printer.Extensions
{
    public static class DecimalExtensions
    {
        public static string ToArabicInvoiceText(
            this decimal number,
            CurrencyInfo currency)
        {
            var text = Tafqeet.ToArabicCurrency(number, currency);

            if (!string.IsNullOrWhiteSpace(text))
                return text + " فقط لا غير";

            return text;
        }
    }
}