using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SAT242516033.Data;

namespace SAT242516033.Models.MyReports
{
    public class Report_Siparis : IDocument
    {
        private List<Siparis> _siparisler;

        public Report_Siparis(List<Siparis> siparisler)
        {
            _siparisler = siparisler;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text("Sipariş Raporu").SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50); // ID
                        columns.RelativeColumn();   // Müşteri ID
                        columns.RelativeColumn();   // Durum
                        columns.RelativeColumn();   // Tarih
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("ID");
                        header.Cell().Element(CellStyle).Text("Müşteri No");
                        header.Cell().Element(CellStyle).Text("Durum");
                        header.Cell().Element(CellStyle).Text("Tarih");
                    });

                    foreach (var s in _siparisler)
                    {
                        table.Cell().Element(CellStyle).Text(s.SiparisId.ToString());
                        table.Cell().Element(CellStyle).Text(s.MusteriId?.ToString() ?? "-");
                        table.Cell().Element(CellStyle).Text(string.IsNullOrWhiteSpace(s.Durum) ? "-" : s.Durum);
                        table.Cell().Element(CellStyle).Text(s.SiparisTarihi?.ToShortDateString() ?? "-");
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Sayfa ");
                    x.CurrentPageNumber();
                });
            });
        }

        static IContainer CellStyle(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5);
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    }
}
