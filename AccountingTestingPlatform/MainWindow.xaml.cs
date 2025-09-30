using AccountingTestingPlatform.NaturezaSaldos;
using AccountingTestingPlatform.Profile;
using AccountingTestingPlatform.Report;
using Microsoft.Extensions.Configuration;
using Npgsql;
using QuestPDF.Infrastructure;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace AccountingTestingPlatform;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    private readonly NpgsqlConnection _connection;

    private readonly IConfigurationRoot _config;

    private DataTable _tblNaturezaSaldo;
    public MainWindow()
    {
        InitializeComponent();

        _config = new ConfigurationBuilder()
            .AddJsonFile("appconfig.json", optional: false, reloadOnChange: true)
            .Build();

        _connection = new(new NpgsqlConnectionStringBuilder(_config["Db:ConnectionStr"]).ToString());
        _connection.Open();

        DataContext = new ProgressMonitor();
        
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.EnableCaching = false;

        try
        {
            entryRemessa.Text = GetPreviousRemessa($"{DateTime.Now.Year:D4}{DateTime.Now.Month:D2}");
        }
        catch (ArgumentException e)
        {
            MessageBox.Show(e.Message,
                            button: MessageBoxButton.OK,
                            caption: "Erro na remessa:",
                            icon: MessageBoxImage.Exclamation);
        }

        BindNaturezaSaldoDataGrid();
    }

    private void BindNaturezaSaldoDataGrid()
    {
        _tblNaturezaSaldo = NaturezaSaldo.MakeNaturezaSaldoTable(_connection);
        NaturezaSaldoDataGrid.ItemsSource = _tblNaturezaSaldo.DefaultView;
    }

    private void ToggleEnableUI()
    {
        btnStartPerfilTest.IsEnabled = !btnStartPerfilTest.IsEnabled;
        entryRemessa.IsEnabled = !entryRemessa.IsEnabled;
        radioAberturaExercicio.IsEnabled = !radioAberturaExercicio.IsEnabled;
        radioEncerramentoExercicio.IsEnabled = !radioEncerramentoExercicio.IsEnabled;
        radioEncerramentoMensal.IsEnabled = radioEncerramentoMensal.IsEnabled;
        radioPad.IsEnabled = radioPad.IsEnabled;
        radioMsc.IsEnabled = radioMsc.IsEnabled;
        tabSaldosInvertidos.IsEnabled = tabSaldosInvertidos.IsEnabled;
        tabValoresManuais.IsEnabled = tabValoresManuais.IsEnabled;
        tabTestes.IsEnabled = tabTestes.IsEnabled;

        if (Cursor == Cursors.Wait)
        {
            Cursor = Cursors.Arrow;
        }
        else
        {
            Cursor = Cursors.Wait;
        }
    }

    private async void BtnStartPerfilTest_Click(object sender, RoutedEventArgs e)
    {
        ToggleEnableUI();
        IReport? report = null;

        string remessa = entryRemessa.Text;
        if(radioAberturaExercicio.IsChecked == true)
        {
            throw new NotImplementedException();
        }
        if (radioEncerramentoExercicio.IsChecked == true)
        {
            throw new NotImplementedException();
        }
        if (radioPad.IsChecked == true)
        {
            throw new NotImplementedException();
        }
        if (radioMsc.IsChecked == true)
        {
            throw new NotImplementedException();
        }
        if (radioEncerramentoMensal.IsChecked == true)
        {
            report = new PdfReport("Testes de Consistência Contábil do Encerramento Mensal", remessa);
            try
            {
                TestRemessa(entryRemessa.Text, 12);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message,
                button: MessageBoxButton.OK,
                caption: "Erro na remessa:",
                icon: MessageBoxImage.Exclamation);
                entryRemessa.Focus();
                entryRemessa.SelectAll();
                return;
            }

            if(DataContext is ProgressMonitor monitor)
            {
                await Task.Run(() =>
                {
                    //IReport report = new PdfReport("Testes de Consistência Contábil do Encerramento Mensal", remessa);
                    IProfileTest profile = new EncerramentoMensalProfileTest(_connection, remessa, report);
                    profile.Run(monitor);
                    monitor.UpdateProgress(99, "Testes terminados. Gerando relatório...");
                    report.Save();
                    monitor.UpdateProgress(100, "Relatório gerado.");
                    //report.Open();
                });
            }
        }

        ToggleEnableUI();
        await Task.Run(() => { report?.Open(); });
    }

    private static void TestRemessa(string remessa, int maxMonth)
    {
        if (remessa.Length != 6)
        {
            throw new ArgumentException($"O valor '{remessa}' informado não está no formado adequado (AAAAMM ex.: 202509)!");

        }

        int year = int.Parse(remessa.Substring(0, 4));
        int month = int.Parse(remessa.Substring(4, 2));

        if(month > maxMonth || month < 1 || year < 1 || year > 9999)
        {
            throw new ArgumentException($"A remessa '{remessa}' não tem um valor válido!");
        }
    }

    private static string GetPreviousRemessa(string remessa)
    {
        if(remessa.Length != 6)
        {
            throw new ArgumentException($"O valor '{remessa}' informado não está no formado adequado (AAAAMM ex.: 202509)!");

        }
        int year = int.Parse(remessa.Substring(0, 4));
        int month = int.Parse(remessa.Substring(4, 2));

        month--;
        if(month == 0)
        {
            month = 12;
            year--;
        }

        return $"{year:D4}{month:D2}";
    }

    private void BtnSalvarNaturezaSaldo_Click(object sender, RoutedEventArgs e)
    {
        if (_tblNaturezaSaldo.HasErrors)
        {
            MessageBox.Show("Existem erros nos dados. Por favor, revise-os.");
            _tblNaturezaSaldo.RejectChanges();
            return;
        }
        NaturezaSaldo.Update(_tblNaturezaSaldo, _connection);
        _tblNaturezaSaldo.AcceptChanges();

    }
}