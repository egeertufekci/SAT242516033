using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SAT242516033.Data;
using System.Collections.Generic;

namespace SAT242516033.Models.MyReports
{
    public class Report_Urun : IDocument
    {
        private readonly List<Urun> _urunler;

        public Report_Urun(List<Urun> urunler)
        {
            _urunler = urunler;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.Header().Text("Ürün Stok Raporu").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.ConstantColumn(80);
                        columns.ConstantColumn(60);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("ID").Bold();
                        header.Cell().Text("Ürün Adı").Bold();
                        header.Cell().Text("Kategori").Bold();
                        header.Cell().Text("Fiyat").Bold();
                        header.Cell().Text("Stok").Bold();
                    });

                    foreach (var u in _urunler)
                    {
                        table.Cell().Text(u.UrunId.ToString());
                        table.Cell().Text(u.UrunAdi ?? "-");
                        table.Cell().Text(u.KategoriAdi ?? "-");
                        table.Cell().Text($"{u.BirimFiyat:N2} TL");
                        table.Cell().Text(u.StokAdet.ToString());
                    }
                });
            });
        }
    }
}