using AccountingTestingPlatform.Test;
using QuestPDF.Elements.Table;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace AccountingTestingPlatform.Report;

class PdfReport : IReport
{
    public readonly string _filepath;
    public readonly string _title;
    public readonly string _remessa;
    private int _totalTest = 0;
    private int _failsTest = 0;
    private readonly Dictionary<string, string> _fails = [];



    private readonly Dictionary<string, List<ITestResult>> _tests = [];

    private Document _doc;
    public PdfReport(string title, string remessa) 
    {
        _filepath = $"{Path.GetTempFileName()}.pdf";
        _title = title ;
        _remessa = remessa ;
    }
    public void AddTest(string testName, List<ITestResult> testResults)
    {
        _tests.Add(testName, testResults);
    }

    public void Save()
    {
        BuildDocument();
    }

    private void BuildDocument()
    {
        var doc = Document.Create(document =>
        {
            static string GetCompetencia(string remessa)
            {
                int month = int.Parse(remessa.Substring(4, 2));
                int year = int.Parse(remessa.Substring(0, 4));
                DateTime date = new DateTime(year, month, 1);
                return date.ToString("MMMM 'de' yyyy");
            }

            void ComposeHeader(IContainer container)
            {
                container
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Lighten1)
                    .PaddingBottom(1)
                    .Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item()
                                .AlignLeft()
                                .Text(_title)
                                .Bold();
                        });
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().AlignRight().Text($"{GetCompetencia(_remessa)} ({_remessa})").SemiBold();
                        });
                    });
            }

            void ComposeFooter(IContainer container)
            {
                container
                    .BorderTop(1)
                    .BorderColor(Colors.Grey.Lighten1)
                    .PaddingBottom(1)
                    .Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().AlignLeft().Text($"Gerado em {DateTime.Now.ToString("dd/MM/yyyy 'às' HH:mm:ss")}");
                        });
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().AlignRight().Text(x =>
                            {
                                x.Span("pág. ");
                                x.CurrentPageNumber();
                                x.Span(" / ");
                                x.TotalPages();
                            });
                        });
                    });
            }

            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Calibri));

                page.Header().Element(ComposeHeader);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(8);

                        foreach (KeyValuePair<string, List<ITestResult>> test in _tests)
                        {
                            x.Item()
                                .Section($"section-{test.Key}")
                                .Element(TestTitleStyle)
                                .Background(Colors.Grey.Lighten3)
                                .Padding(2)
                                .Text($"Teste: {test.Key}");
                            foreach(ITestResult result in test.Value)
                            {
                                _totalTest++;
                                if (result.IsSuccess() == false)
                                {
                                    _failsTest++;
                                    _fails.Add($"{test.Key} ({result.GetTestScope()})", test.Key);
                                }
                                ITest testClass = result.GetTest();
                                x.Item().Element(TestScopeStyle).Text(result.GetTestScope());
                                x.Item().Element(TestScopeStyle).Text(testClass.ToString());
                                x.Item().Component(result.GetResultComponent());
                            }
                            x.Item()
                            .PaddingVertical(4)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten1);
                        }
                    });

                static IContainer TestTitleStyle(IContainer container)
                {
                    return container
                        .DefaultTextStyle(x => x.Bold());
                }
                static IContainer TestScopeStyle(IContainer container)
                {
                    return container
                        .DefaultTextStyle(x => x.SemiBold());
                }

                page.Footer().Element(ComposeFooter);
            });

            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Calibri));

                page.Header().Element(ComposeHeader);

                page.Content()
                    .PaddingVertical(4)
                    .Column(x =>
                    {
                        //x.Spacing(4);

                        x.Item()
                        .BorderTop(1)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten1)
                        .Background(Colors.Grey.Lighten3)
                        .Padding(4)
                        .Text("Resumo")
                        .Bold();

                        x.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(100);
                                    columns.ConstantColumn(50);
                                });

                                table.Cell().Row(1).Column(1).Text("Testes realizados:");
                                table.Cell().Row(1).Column(2).AlignRight().Text(_totalTest.ToString("D0"));
                                table.Cell().Row(1).Column(3).Text("");
                                table.Cell().Row(2).Column(1).Text("Testes que passaram:");
                                table.Cell().Row(2).Column(2).AlignRight().Text((_totalTest - _failsTest).ToString("D0"));
                                table.Cell().Row(2).Column(3).AlignRight().Text(((_totalTest - _failsTest)/(double)_totalTest).ToString("p0")).FontColor(Colors.Green.Medium);
                                table.Cell().Row(3).Column(1).Text("Testes que falharam:");
                                table.Cell().Row(3).Column(2).AlignRight().Text(_failsTest.ToString("D0"));
                                table.Cell().Row(3).Column(3).AlignRight().Text((_failsTest/(double)_totalTest).ToString("p0")).FontColor(Colors.Red.Medium);
                            });
                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        //x.Spacing(8);
                        if(_fails.Count() > 0)
                        {
                            x.Item()
                            .PaddingTop(4)
                            .BorderTop(1)
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten1)
                            .Background(Colors.Grey.Lighten3)
                            .Padding(4)
                            .Text("Testes que falharam:")
                            .Bold();

                            foreach(KeyValuePair<string, string> test in _fails)
                            {
                                x.Item()
                                    .SectionLink($"section-{test.Value}")
                                    .Text(test.Key);
                            }

                            x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        }

                    });

                page.Footer().Element(ComposeFooter);

            });
        })
        .WithMetadata(new DocumentMetadata
        {
            Title = _title,
            Author = "Everton da Rosa",
            Subject = _title,
            Creator = "Accounting Test Platform Application",
            Language = "pt-BR",
            CreationDate = DateTimeOffset.Now,
            ModifiedDate = DateTimeOffset.Now,
        })
        .WithSettings(new DocumentSettings
        {
            PdfA = false,
            CompressDocument = true,
        });

        _doc = doc;            
    }

    public void Open()
    {
        _doc.GeneratePdfAndShow();
    }
}
