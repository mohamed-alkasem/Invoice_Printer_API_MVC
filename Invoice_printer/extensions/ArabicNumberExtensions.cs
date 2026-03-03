using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    
    public static class Tafqeet
    {
        #region الأرقام الأساسية

        private static readonly string[] Ones =
        {
            "", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة",
            "ستة", "سبعة", "ثمانية", "تسعة",
            "عشرة", "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر", "خمسة عشر",
            "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر"
        };

        private static readonly string[] OnesF =
        {
            "", "واحدة", "اثنتان", "ثلاث", "أربع", "خمس",
            "ست", "سبع", "ثماني", "تسع",
            "عشر", "إحدى عشرة", "اثنتا عشرة", "ثلاث عشرة", "أربع عشرة", "خمس عشرة",
            "ست عشرة", "سبع عشرة", "ثماني عشرة", "تسع عشرة"
        };

        private static readonly string[] Tens =
        {
            "", "عشرة", "عشرون", "ثلاثون", "أربعون", "خمسون",
            "ستون", "سبعون", "ثمانون", "تسعون"
        };

        private static readonly string[] Hundreds =
        {
            "", "مائة", "مائتان", "ثلاثمائة", "أربعمائة", "خمسمائة",
            "ستمائة", "سبعمائة", "ثمانمائة", "تسعمائة"
        };

        #endregion

        #region المراتب (آلاف، ملايين، مليارات)

        // [مفرد، مثنى، جمع (3-10), جمع (11+)]
        private static readonly string[][] Groups =
        {
            new[] { "", "", "", "" },                           // وحدات
            new[] { "ألف", "ألفان", "آلاف", "ألفاً" },        // آلاف
            new[] { "مليون", "مليونان", "ملايين", "مليوناً" },  // ملايين
            new[] { "مليار", "ملياران", "مليارات", "ملياراً" }  // مليارات
        };

        #endregion

        /// <summary>
        /// تحويل رقم إلى كلمات عربية
        /// </summary>
        /// <param name="number">الرقم المراد تفقيطه</param>
        /// <param name="currency">العملة (اختياري)</param>
        /// <param name="subCurrency">العملة الفرعية (اختياري)</param>
        /// <param name="isFeminine">هل المعدود مؤنث</param>
        /// <returns>التفقيط بالعربي</returns>
        public static string ToArabicWords(
            decimal number,
            string currency = "",
            string subCurrency = "",
            bool isFeminine = false)
        {
            if (number == 0)
                return "صفر" + (string.IsNullOrEmpty(currency) ? "" : " " + currency);

            var result = new StringBuilder();
            bool isNegative = number < 0;
            number = Math.Abs(number);

            // فصل الجزء الصحيح عن الكسري
            long integerPart = (long)Math.Floor(number);
            int decimalPart = (int)Math.Round((number - integerPart) * 100);

            if (isNegative)
                result.Append("سالب ");

            // تفقيط الجزء الصحيح
            if (integerPart > 0)
            {
                result.Append(ConvertIntegerToWords(integerPart, isFeminine));

                if (!string.IsNullOrEmpty(currency))
                    result.Append(" " + currency);
            }
            else if (!string.IsNullOrEmpty(currency))
            {
                result.Append("صفر " + currency);
            }

            // تفقيط الجزء الكسري (القروش/الهللات/الفلسات)
            if (decimalPart > 0)
            {
                if (integerPart > 0)
                    result.Append(" و");

                result.Append(ConvertIntegerToWords(decimalPart, isFeminine));

                if (!string.IsNullOrEmpty(subCurrency))
                    result.Append(" " + subCurrency);
            }

            return result.ToString().Trim();
        }

        /// <summary>
        /// تفقيط مع عملة - اختصار سريع
        /// </summary>
        public static string ToArabicCurrency(
            decimal number,
            CurrencyInfo currency)
        {
            return ToArabicWords(
                number,
                GetCurrencyName(number, currency.MainUnit, currency.MainUnitPlural),
                GetCurrencyName(number % 1 * 100, currency.SubUnit, currency.SubUnitPlural),
                currency.IsFeminine);
        }

        #region التحويل الداخلي

        private static string ConvertIntegerToWords(long number, bool isFeminine)
        {
            if (number == 0) return "صفر";
            if (number < 0) return "سالب " + ConvertIntegerToWords(-number, isFeminine);

            var parts = new List<string>();

            // تقسيم الرقم إلى مجموعات (وحدات، آلاف، ملايين، مليارات)
            int groupIndex = 0;
            while (number > 0)
            {
                int group = (int)(number % 1000);
                number /= 1000;

                if (group > 0)
                {
                    string groupText = ConvertGroup(group, groupIndex, isFeminine);
                    if (!string.IsNullOrEmpty(groupText))
                        parts.Insert(0, groupText);
                }

                groupIndex++;
            }

            return string.Join(" و", parts);
        }

        private static string ConvertGroup(int number, int groupLevel, bool isFeminine)
        {
            if (number == 0) return "";

            var parts = new List<string>();

            int hundreds = number / 100;
            int remainder = number % 100;
            int tens = remainder / 10;
            int ones = remainder % 10;

            // المئات
            if (hundreds > 0)
                parts.Add(Hundreds[hundreds]);

            // العشرات والآحاد
            if (remainder > 0)
            {
                if (remainder < 20)
                {
                    // 1-19
                    // الآلاف والملايين مذكرة دائماً
                    bool useF = (groupLevel == 0) ? isFeminine : false;
                    string word = useF ? OnesF[remainder] : Ones[remainder];
                    parts.Add(word);
                }
                else
                {
                    // 20-99
                    if (ones > 0)
                    {
                        bool useF = (groupLevel == 0) ? isFeminine : false;
                        string oneWord = useF ? OnesF[ones] : Ones[ones];
                        parts.Add(oneWord + " و" + Tens[tens]);
                    }
                    else
                    {
                        parts.Add(Tens[tens]);
                    }
                }
            }

            string result = string.Join(" و", parts);

            // إضافة اسم المرتبة (ألف، مليون، مليار)
            if (groupLevel > 0)
            {
                result = AppendGroupName(result, number, groupLevel);
            }

            return result;
        }

        private static string AppendGroupName(string numberText, int number, int groupLevel)
        {
            if (groupLevel == 0 || groupLevel >= Groups.Length)
                return numberText;

            string[] groupNames = Groups[groupLevel];

            if (number == 1)
                return groupNames[0]; // ألف، مليون، مليار (بدون "واحد")

            if (number == 2)
                return groupNames[1]; // ألفان، مليونان، ملياران

            if (number >= 3 && number <= 10)
                return numberText + " " + groupNames[2]; // آلاف، ملايين، مليارات

            // 11 فما فوق
            return numberText + " " + groupNames[3]; // ألفاً، مليوناً، ملياراً
        }

        private static string GetCurrencyName(decimal amount, string singular, string plural)
        {
            long intAmount = (long)Math.Floor(Math.Abs(amount));
            if (intAmount == 0 || intAmount == 1 || intAmount == 2)
                return singular;
            return plural;
        }

        #endregion

        #region العملات الجاهزة

        /// <summary>
        /// عملات جاهزة للاستخدام
        /// </summary>
        public static class Currencies
        {
            public static CurrencyInfo SyrianPound => new CurrencyInfo
            {
                MainUnit = "ليرة سورية",
                MainUnitPlural = "ليرات سورية",
                SubUnit = "قرش",
                SubUnitPlural = "قروش",
                IsFeminine = true
            };

            public static CurrencyInfo TurkishLira => new CurrencyInfo
            {
                MainUnit = "ليرة تركية",
                MainUnitPlural = "ليرات تركية",
                SubUnit = "قرش",
                SubUnitPlural = "قروش",
                IsFeminine = true
            };

            public static CurrencyInfo USDollar => new CurrencyInfo
            {
                MainUnit = "دولار أمريكي",
                MainUnitPlural = "دولارات أمريكية",
                SubUnit = "سنت",
                SubUnitPlural = "سنتات",
                IsFeminine = false
            };

            public static CurrencyInfo SaudiRiyal => new CurrencyInfo
            {
                MainUnit = "ريال سعودي",
                MainUnitPlural = "ريالات سعودية",
                SubUnit = "هللة",
                SubUnitPlural = "هللات",
                IsFeminine = false
            };

            public static CurrencyInfo Euro => new CurrencyInfo
            {
                MainUnit = "يورو",
                MainUnitPlural = "يورو",
                SubUnit = "سنت",
                SubUnitPlural = "سنتات",
                IsFeminine = false
            };

            public static CurrencyInfo IraqiDinar => new CurrencyInfo
            {
                MainUnit = "دينار عراقي",
                MainUnitPlural = "دنانير عراقية",
                SubUnit = "فلس",
                SubUnitPlural = "فلوس",
                IsFeminine = false
            };
        }

        #endregion
    }

    /// <summary>
    /// معلومات العملة
    /// </summary>
    public class CurrencyInfo
    {
        public string MainUnit { get; set; } = "";
        public string MainUnitPlural { get; set; } = "";
        public string SubUnit { get; set; } = "";
        public string SubUnitPlural { get; set; } = "";
        public bool IsFeminine { get; set; }
    }

   

}
