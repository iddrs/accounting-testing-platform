using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.SaldosInvertidosTest;

class SaldosInvertidosTestResultTableComponent : IComponent
{
    private readonly DataTable _result;
    public SaldosInvertidosTestResultTableComponent(DataTable result)
    { 
        _result = result;
    }
    public void Compose(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(100);
                columns.RelativeColumn();
                columns.ConstantColumn(100);
                columns.ConstantColumn(10);
                columns.ConstantColumn(50);
            });

            table.Header(header =>
            {
                header.Cell().Row(1).Column(1).Element(HeaderCellStyle).Text("Código");
                header.Cell().Row(1).Column(2).Element(HeaderCellStyle).Text("Conta Contábil");
                header.Cell().Row(1).Column(3).Element(HeaderCellStyle).AlignRight().Text("Saldo");
                header.Cell().Row(1).Column(4).Element(HeaderCellStyle).Text("");
                header.Cell().Row(1).Column(5).Element(HeaderCellStyle).AlignCenter().Text("Natureza Esperada");
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
                table.Cell().Row(r).Column(3).Element(CellStyle).AlignRight().Text(decimal.Parse(row[2].ToString()).ToString("C2"));
                table.Cell().Row(r).Column(4).Element(CellStyle).Text(row[3].ToString());
                table.Cell().Row(r).Column(5).Element(CellStyle).AlignCenter().Text(row[4].ToString());
                r++;
            }
            static IContainer CellStyle(IContainer container)
            {
                return container
                    .BorderTop(1).BorderColor(Colors.Grey.Lighten1)
                    .BorderBottom(1).BorderColor(Colors.Grey.Lighten1)
                    .DefaultTextStyle(x => x.Bold())
                    .Padding(1);
            }
        });
    }
}
