using System;
using System.Text;

namespace Invoice_printer.Extensions
{
    public static class NumberExtensions
    {
        public static string ToEnglishMoneyWords(this decimal amount, string currency = "")
        {
            if (amount < 0) return "Minus " + ToEnglishMoneyWords(Math.Abs(amount), currency);

            var integerPart = (long)Math.Floor(amount);
            var fractionalPart = (int)Math.Round((amount - integerPart) * 100, 0);

            var words = new StringBuilder();
            words.Append(integerPart.ToEnglishWords());

            if (!string.IsNullOrWhiteSpace(currency))
                words.Append(" ").Append(currency);

            words.Append(" and ").Append(fractionalPart.ToString("00")).Append("/100");
            return words.ToString();
        }

        public static string ToEnglishWords(this long number)
        {
            if (number == 0) return "Zero";
            if (number < 0) return "Minus " + ToEnglishWords(Math.Abs(number));

            return Convert(number).Trim();

            static string Convert(long n)
            {
                string[] units =
                {
                    "Zero","One","Two","Three","Four","Five","Six","Seven","Eight","Nine","Ten",
                    "Eleven","Twelve","Thirteen","Fourteen","Fifteen","Sixteen","Seventeen","Eighteen","Nineteen"
                };

                string[] tens =
                {
                    "Zero","Ten","Twenty","Thirty","Forty","Fifty","Sixty","Seventy","Eighty","Ninety"
                };

                if (n < 20) return units[n];
                if (n < 100)
                {
                    var t = n / 10;
                    var u = n % 10;
                    return u == 0 ? tens[t] : $"{tens[t]}-{units[u]}";
                }
                if (n < 1000)
                {
                    var h = n / 100;
                    var r = n % 100;
                    return r == 0 ? $"{units[h]} Hundred" : $"{units[h]} Hundred {Convert(r)}";
                }
                if (n < 1_000_000)
                {
                    var th = n / 1000;
                    var r = n % 1000;
                    return r == 0 ? $"{Convert(th)} Thousand" : $"{Convert(th)} Thousand {Convert(r)}";
                }
                if (n < 1_000_000_000)
                {
                    var m = n / 1_000_000;
                    var r = n % 1_000_000;
                    return r == 0 ? $"{Convert(m)} Million" : $"{Convert(m)} Million {Convert(r)}";
                }

                var b = n / 1_000_000_000;
                var rr = n % 1_000_000_000;
                return rr == 0 ? $"{Convert(b)} Billion" : $"{Convert(b)} Billion {Convert(rr)}";
            }
        }
    }
}