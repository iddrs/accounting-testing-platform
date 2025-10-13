using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.EncerramentoAnualTest;

class EncerramentoAnualTestResultTableComponent : IComponent
{
    private readonly DataTable _result;
    public EncerramentoAnualTestResultTableComponent(DataTable result)
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
            });

            table.Header(header =>
            {
                header.Cell().Row(1).Column(1).Element(HeaderCellStyle).Text("SQL");
                header.Cell().Row(1).Column(2).Element(HeaderCellStyle).AlignRight().Text("Valor");
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
                if (row[0].ToString() == "Total")
                {
                    table.Cell().Row(r).Column(1).Element(TotalCellStyle).Text(row[0].ToString());
                    table.Cell().Row(r).Column(2).Element(TotalCellStyle).AlignRight().Text(decimal.Parse(row[1].ToString()).ToString("C2"));
                }
                else if(row[0].ToString() == "Diferença")
                {
                    if (decimal.Parse(row[1].ToString()) == 0m)
                    {
                        table.Cell().Row(r).Column(1).Element(DiferencaSuccessCellStyle).Text(row[0].ToString());
                        table.Cell().Row(r).Column(2).Element(DiferencaSuccessCellStyle).AlignRight().Text(decimal.Parse(row[1].ToString()).ToString("C2"));
                    }
                    else
                    {
                        table.Cell().Row(r).Column(1).Element(DiferencaFailCellStyle).Text(row[0].ToString());
                        table.Cell().Row(r).Column(2).Element(DiferencaFailCellStyle).AlignRight().Text($"{decimal.Parse(row[1].ToString()).ToString("C2")} ⚠️");
                    }

                }
                else
                {
                    table.Cell().Row(r).Column(1).Element(CellStyle).Text(row[0].ToString());
                    table.Cell().Row(r).Column(2).Element(CellStyle).AlignRight().Text(decimal.Parse(row[1].ToString()).ToString("C2"));
                }
                r++;
            }
            static IContainer CellStyle(IContainer container)
            {
                return container
                    .DefaultTextStyle(x => x.FontColor(Colors.Black))
                    .Padding(1);
            }
            static IContainer TotalCellStyle(IContainer container)
            {
                return container
                    .BorderTop(1).BorderColor(Colors.Grey.Lighten1)
                    .BorderBottom(1).BorderColor(Colors.Grey.Lighten1)
                    .DefaultTextStyle(x => x.Bold())
                    .Padding(1);
            }
            static IContainer DiferencaSuccessCellStyle(IContainer container)
            {
                return container
                    .BorderTop(1).BorderColor(Colors.Grey.Lighten1)
                    .BorderBottom(1).BorderColor(Colors.Grey.Lighten1)
                    .DefaultTextStyle(x => x.Bold())
                    .Padding(1);
            }
            static IContainer DiferencaFailCellStyle(IContainer container)
            {
                return container
                    .Background(Colors.Red.Lighten5)
                    .BorderTop(1).BorderColor(Colors.Red.Medium)
                    .BorderBottom(1).BorderColor(Colors.Red.Medium)
                    .DefaultTextStyle(x => x.FontColor(Colors.Black).Bold())
                    .Padding(1);
            }
        });
    }
}
