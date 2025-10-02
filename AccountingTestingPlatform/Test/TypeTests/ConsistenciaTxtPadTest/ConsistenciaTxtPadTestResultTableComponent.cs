using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistenciaTxtPadTest;

class ConsistenciaTxtPadTestResultTableComponent : IComponent
{
    private readonly DataTable _result;
    public ConsistenciaTxtPadTestResultTableComponent(DataTable result)
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
                columns.ConstantColumn(100);
                columns.ConstantColumn(100);
                columns.ConstantColumn(100);
            });

            table.Header(header =>
            {
                header.Cell().Row(1).Column(1).Element(HeaderCellStyle).Text("Item");
                header.Cell().Row(1).Column(2).Element(HeaderCellStyle).AlignRight().Text("Esperado");
                header.Cell().Row(1).Column(3).Element(HeaderCellStyle).AlignRight().Text("Encontrado");
                header.Cell().Row(1).Column(4).Element(HeaderCellStyle).AlignRight().Text("Diferença");
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
                table.Cell().Row(r).Column(2).Element(CellStyle).AlignRight().Text(decimal.Parse(row[1].ToString()).ToString("C2"));
                table.Cell().Row(r).Column(3).Element(CellStyle).AlignRight().Text(decimal.Parse(row[2].ToString()).ToString("C2"));
                if (decimal.Parse(row[3].ToString()) == 0m)
                {
                    table.Cell().Row(r).Column(4).Element(DiferencaSuccessCellStyle).AlignRight().Text(decimal.Parse(row[3].ToString()).ToString("C2"));
                }
                else
                {
                    table.Cell().Row(r).Column(4).Element(DiferencaFailCellStyle).AlignRight().Text($"{decimal.Parse(row[3].ToString()).ToString("C2")} ⚠️");
                }
                r++;
            }
            static IContainer CellStyle(IContainer container)
            {
                return container
                    .DefaultTextStyle(x => x.FontColor(Colors.Black))
                    .Padding(1);
            }
            static IContainer DiferencaSuccessCellStyle(IContainer container)
            {
                return container
                    .DefaultTextStyle(x => x.Bold())
                    .Padding(1);
            }
            static IContainer DiferencaFailCellStyle(IContainer container)
            {
                return container
                    .Background(Colors.Red.Lighten5)
                    .DefaultTextStyle(x => x.FontColor(Colors.Black).Bold())
                    .Padding(1);
            }
        });
    }
}
