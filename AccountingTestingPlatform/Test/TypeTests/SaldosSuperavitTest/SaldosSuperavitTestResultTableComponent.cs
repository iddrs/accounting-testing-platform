using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.SaldosSuperavitTest;

class SaldosSuperavitTestResultTableComponent : IComponent
{
    private readonly DataTable _result;
    public SaldosSuperavitTestResultTableComponent(DataTable result)
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
            });

            table.Header(header =>
            {
                header.Cell().Row(1).Column(1).Element(HeaderCellStyle).AlignCenter().Text("FR");
                header.Cell().Row(1).Column(2).Element(HeaderCellStyle).AlignRight().Text("Saldo financeiro bruto");
                header.Cell().Row(1).Column(3).Element(HeaderCellStyle).AlignRight().Text("Saldo inicial de RP");
                header.Cell().Row(1).Column(4).Element(HeaderCellStyle).AlignRight().Text("Superávit financeiro inicial");
                header.Cell().Row(1).Column(5).Element(HeaderCellStyle).AlignRight().Text("Cancelamento de RP");
                header.Cell().Row(1).Column(6).Element(HeaderCellStyle).AlignRight().Text("Créditos abertos");
                header.Cell().Row(1).Column(7).Element(HeaderCellStyle).AlignRight().Text("Saldo do superávit");
                header.Cell().Row(1).Column(8).Element(HeaderCellStyle).AlignRight().Text("Saldo atual contábil");
                header.Cell().Row(1).Column(9).Element(HeaderCellStyle).AlignRight().Text("Diferença");
            });

            static IContainer HeaderCellStyle(IContainer container)
            {
                return container
                    .BorderTop(1).BorderColor(Colors.Grey.Lighten1)
                    .BorderBottom(1).BorderColor(Colors.Grey.Lighten1)
                    .DefaultTextStyle(x => x.Bold().FontSize(8))
                    .Padding(2);
            }

            uint r = 1;
            foreach (DataRow row in _result.Rows)
            {
                table.Cell().Row(r).Column(1).Element(CellStyle).Text(row[0].ToString());
                table.Cell().Row(r).Column(2).Element(CellStyle).AlignRight().Text(decimal.Parse(row[1].ToString()).ToString("C2"));
                table.Cell().Row(r).Column(3).Element(CellStyle).AlignRight().Text(decimal.Parse(row[2].ToString()).ToString("C2"));
                table.Cell().Row(r).Column(4).Element(CellStyle).AlignRight().Text(decimal.Parse(row[3].ToString()).ToString("C2"));
                table.Cell().Row(r).Column(5).Element(CellStyle).AlignRight().Text(decimal.Parse(row[4].ToString()).ToString("C2"));
                table.Cell().Row(r).Column(6).Element(CellStyle).AlignRight().Text(decimal.Parse(row[5].ToString()).ToString("C2"));
                table.Cell().Row(r).Column(7).Element(CellStyle).AlignRight().Text(decimal.Parse(row[6].ToString()).ToString("C2"));
                table.Cell().Row(r).Column(8).Element(CellStyle).AlignRight().Text(decimal.Parse(row[7].ToString()).ToString("C2"));
                table.Cell().Row(r).Column(9).Element(CellStyle).AlignRight().Text(decimal.Parse(row[8].ToString()).ToString("C2"));
                r++;
            }
            static IContainer CellStyle(IContainer container)
            {
                return container
                    .BorderTop(1).BorderColor(Colors.Grey.Lighten1)
                    .BorderBottom(1).BorderColor(Colors.Grey.Lighten1)
                    .DefaultTextStyle(x => x.Bold().FontSize(8))
                    .Padding(1);
            }
        });
    }
}
