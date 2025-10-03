using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistenciaMscTest;

class BalverMscTestResultTableComponent : IComponent
{
    private readonly DataTable _result;
    public BalverMscTestResultTableComponent(DataTable result)
    { 
        _result = result;
    }
    public void Compose(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                
            });

            table.Header(header =>
            {
                header.Cell().Row(1).ColumnSpan(2).Column(1).Element(HeaderCellStyle).Text("Conta contábil");
                header.Cell().Row(2).Column(1).Element(HeaderCellStyle).Text("Bal Ver");
                header.Cell().Row(2).Column(2).Element(HeaderCellStyle).AlignRight().Text("Msc");
                header.Cell().Row(1).ColumnSpan(3).Column(3).Element(HeaderCellStyle).Text("Saldo anterior");
                header.Cell().Row(2).Column(3).Element(HeaderCellStyle).AlignRight().Text("Bal Ver");
                header.Cell().Row(2).Column(4).Element(HeaderCellStyle).AlignRight().Text("MSC");
                header.Cell().Row(2).Column(5).Element(HeaderCellStyle).AlignRight().Text("Diferença");
                header.Cell().Row(1).ColumnSpan(3).Column(6).Element(HeaderCellStyle).Text("Débitos");
                header.Cell().Row(2).Column(6).Element(HeaderCellStyle).AlignRight().Text("Bal Ver");
                header.Cell().Row(2).Column(7).Element(HeaderCellStyle).AlignRight().Text("MSC");
                header.Cell().Row(2).Column(8).Element(HeaderCellStyle).AlignRight().Text("Diferença");
                header.Cell().Row(1).ColumnSpan(3).Column(9).Element(HeaderCellStyle).Text("Créditos");
                header.Cell().Row(2).Column(9).Element(HeaderCellStyle).AlignRight().Text("Bal Ver");
                header.Cell().Row(2).Column(10).Element(HeaderCellStyle).AlignRight().Text("MSC");
                header.Cell().Row(2).Column(11).Element(HeaderCellStyle).AlignRight().Text("Diferença");
                header.Cell().Row(1).ColumnSpan(3).Column(12).Element(HeaderCellStyle).Text("Saldo atual");
                header.Cell().Row(2).Column(12).Element(HeaderCellStyle).AlignRight().Text("Bal Ver");
                header.Cell().Row(2).Column(13).Element(HeaderCellStyle).AlignRight().Text("MSC");
                header.Cell().Row(2).Column(14).Element(HeaderCellStyle).AlignRight().Text("Diferença");
            });

            static IContainer HeaderCellStyle(IContainer container)
            {
                return container
                    .BorderTop(1).BorderColor(Colors.Grey.Lighten1)
                    .BorderBottom(1).BorderColor(Colors.Grey.Lighten1)
                    .DefaultTextStyle(x => x.Bold())
                    .Padding(2);
            }

            uint r = 1;
            foreach (DataRow row in _result.Rows)
            {

                table.Cell().Row(r).Column(1).Element(CellStyle).Text(row[0].ToString());
                table.Cell().Row(r).Column(2).Element(CellStyle).Text(row[1].ToString());
                table.Cell().Row(r).Column(3).Element(CellStyle).AlignRight().Text(decimal.Parse(row[2].ToString()).ToString("N2"));
                table.Cell().Row(r).Column(4).Element(CellStyle).AlignRight().Text(decimal.Parse(row[3].ToString()).ToString("N2"));
                table.Cell().Row(r).Column(5).Element(CellStyle).AlignRight().Text(decimal.Parse(row[4].ToString()).ToString("N2"));
                table.Cell().Row(r).Column(6).Element(CellStyle).AlignRight().Text(decimal.Parse(row[5].ToString()).ToString("N2"));
                table.Cell().Row(r).Column(7).Element(CellStyle).AlignRight().Text(decimal.Parse(row[6].ToString()).ToString("N2"));
                table.Cell().Row(r).Column(8).Element(CellStyle).AlignRight().Text(decimal.Parse(row[7].ToString()).ToString("N2"));
                table.Cell().Row(r).Column(9).Element(CellStyle).AlignRight().Text(decimal.Parse(row[8].ToString()).ToString("N2"));
                table.Cell().Row(r).Column(10).Element(CellStyle).AlignRight().Text(decimal.Parse(row[9].ToString()).ToString("N2"));
                table.Cell().Row(r).Column(11).Element(CellStyle).AlignRight().Text(decimal.Parse(row[10].ToString()).ToString("N2"));
                table.Cell().Row(r).Column(12).Element(CellStyle).AlignRight().Text(decimal.Parse(row[11].ToString()).ToString("N2"));
                table.Cell().Row(r).Column(13).Element(CellStyle).AlignRight().Text(decimal.Parse(row[12].ToString()).ToString("N2"));
                table.Cell().Row(r).Column(14).Element(CellStyle).AlignRight().Text(decimal.Parse(row[13].ToString()).ToString("N2"));
                r++;
            }
            static IContainer CellStyle(IContainer container)
            {
                return container
                    .DefaultTextStyle(x => x.FontColor(Colors.Black))
                    .Padding(1);
            }
        });
    }
}
