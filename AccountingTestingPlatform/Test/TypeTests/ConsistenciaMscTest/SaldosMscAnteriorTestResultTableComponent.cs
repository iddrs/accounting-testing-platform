using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistenciaMscTest;

class SaldosMscAnteriorTestResultTableComponent : IComponent
{
    private readonly DataTable _result;
    public SaldosMscAnteriorTestResultTableComponent(DataTable result)
    { 
        _result = result;
    }
    public void Compose(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(50);//cc
                columns.RelativeColumn();//po
                columns.RelativeColumn();//fp
                columns.RelativeColumn();//dc
                columns.RelativeColumn();//exfr
                columns.RelativeColumn();//fr
                columns.RelativeColumn();//co
                columns.ConstantColumn(50);//nro
                columns.ConstantColumn(50);//ndo
                columns.RelativeColumn();//f
                columns.RelativeColumn();//sf
                columns.RelativeColumn();//arp
                columns.ConstantColumn(80);//sf
                columns.ConstantColumn(80);//si
                columns.ConstantColumn(60);//d

            });

            table.Header(header =>
            {
                header.Cell().Row(1).Column(1).Element(HeaderCellStyle).Text("Conta contábil");
                header.Cell().Row(1).Column(2).Element(HeaderCellStyle).Text("Poder e órgão");
                header.Cell().Row(1).Column(3).Element(HeaderCellStyle).Text("F/P");
                header.Cell().Row(1).Column(4).Element(HeaderCellStyle).Text("DC");
                header.Cell().Row(1).Column(5).Element(HeaderCellStyle).Text("Exercício FR");
                header.Cell().Row(1).Column(6).Element(HeaderCellStyle).Text("FR");
                header.Cell().Row(1).Column(7).Element(HeaderCellStyle).Text("CO");
                header.Cell().Row(1).Column(8).Element(HeaderCellStyle).Text("NRO");
                header.Cell().Row(1).Column(9).Element(HeaderCellStyle).Text("NDO");
                header.Cell().Row(1).Column(10).Element(HeaderCellStyle).Text("Função");
                header.Cell().Row(1).Column(11).Element(HeaderCellStyle).Text("Subfunção");
                header.Cell().Row(1).Column(12).Element(HeaderCellStyle).Text("Ano RP");
                header.Cell().Row(1).Column(13).Element(HeaderCellStyle).AlignRight().Text("Saldo final");
                header.Cell().Row(1).Column(14).Element(HeaderCellStyle).AlignRight().Text("Saldo inicial");
                header.Cell().Row(1).Column(15).Element(HeaderCellStyle).AlignRight().Text("Diferença");
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
                table.Cell().Row(r).Column(3).Element(CellStyle).Text(row[2].ToString());
                table.Cell().Row(r).Column(4).Element(CellStyle).Text(row[3].ToString());
                table.Cell().Row(r).Column(5).Element(CellStyle).Text(row[4].ToString());
                table.Cell().Row(r).Column(6).Element(CellStyle).Text(row[5].ToString());
                table.Cell().Row(r).Column(7).Element(CellStyle).Text(row[6].ToString());
                table.Cell().Row(r).Column(8).Element(CellStyle).Text(row[7].ToString());
                table.Cell().Row(r).Column(9).Element(CellStyle).Text(row[8].ToString());
                table.Cell().Row(r).Column(10).Element(CellStyle).Text(row[9].ToString());
                table.Cell().Row(r).Column(11).Element(CellStyle).Text(row[10].ToString());
                table.Cell().Row(r).Column(12).Element(CellStyle).Text(row[11].ToString());
                table.Cell().Row(r).Column(13).Element(CellStyle).AlignRight().Text(decimal.Parse(row[12].ToString()).ToString("N2"));
                table.Cell().Row(r).Column(14).Element(CellStyle).AlignRight().Text(decimal.Parse(row[13].ToString()).ToString("N2"));
                table.Cell().Row(r).Column(15).Element(CellStyle).AlignRight().Text(decimal.Parse(row[14].ToString()).ToString("N2"));
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
