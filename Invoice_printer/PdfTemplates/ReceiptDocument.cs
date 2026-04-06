using Invoice_printer.DTO_S;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Invoice_printer.PdfTemplates
{
    public class ReceiptDocument : IDocument
    {
        private readonly ReceiptPrintViewModel _model;

        public ReceiptDocument(ReceiptPrintViewModel model)
        {
            _model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(24);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(6);

                column.Item().AlignRight().Text("فاتورة / إيصال")
                    .SemiBold()
                    .FontSize(18);

                column.Item().AlignRight().Text(text =>
                {
                    text.Span("رقم الإيصال: ").SemiBold();
                    text.Span(string.IsNullOrWhiteSpace(_model.ReceiptNo) ? "—" : _model.ReceiptNo);
                });

                column.Item().AlignRight().Text(text =>
                {
                    text.Span("التاريخ: ").SemiBold();
                    text.Span(_model.Date == default ? DateTime.UtcNow.ToString("yyyy-MM-dd") : _model.Date.ToString("yyyy-MM-dd"));
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(16).Column(column =>
            {
                column.Spacing(14);

                column.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(info =>
                {
                    info.Spacing(4);
                    info.Item().AlignRight().Text("بيانات الشركة").SemiBold();
                    info.Item().AlignRight().Text(string.IsNullOrWhiteSpace(_model.CompanyName) ? "اسم الشركة" : _model.CompanyName);
                    info.Item().AlignRight().Text(string.IsNullOrWhiteSpace(_model.CompanyAddres) ? "العنوان" : _model.CompanyAddres);
                });

                column.Item().Element(ComposeItemsTable);

                column.Item().AlignRight().Text(text =>
                {
                    text.Span("الإجمالي: ").SemiBold().FontSize(13);
                    text.Span($"{_model.Total:N2} {_model.Currency}").FontSize(13);
                });
            });
        }

        private void ComposeItemsTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // اسم الصنف
                    columns.RelativeColumn(1); // الكمية
                    columns.RelativeColumn(2); // سعر الوحدة
                    columns.RelativeColumn(2); // الإجمالي
                });

                static IContainer CellStyle(IContainer c) =>
                    c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6).PaddingHorizontal(4);

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).AlignRight().Text("الصنف").SemiBold();
                    header.Cell().Element(CellStyle).AlignRight().Text("الكمية").SemiBold();
                    header.Cell().Element(CellStyle).AlignRight().Text("سعر الوحدة").SemiBold();
                    header.Cell().Element(CellStyle).AlignRight().Text("الإجمالي").SemiBold();
                });

                if (_model.Items is { Count: > 0 })
                {
                    foreach (var item in _model.Items)
                    {
                        table.Cell().Element(CellStyle).AlignRight().Text(string.IsNullOrWhiteSpace(item.Title) ? "عنصر" : item.Title);
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity:N2}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.UnitPrice:N2}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.LineTotal:N2}");
                    }
                }
                else
                {
                    table.Cell().ColumnSpan(4).Element(CellStyle).AlignCenter().Text("لا توجد عناصر").Italic();
                }
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.PaddingTop(8).BorderTop(1).BorderColor(Colors.Grey.Lighten2).Row(row =>
            {
                row.RelativeItem().AlignRight().Text("شكراً لتعاملكم معنا").FontSize(10).FontColor(Colors.Grey.Darken1);
            });
        }
    }
}
