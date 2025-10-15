using AccountingTestingPlatform.NaturezaSaldos;
using AccountingTestingPlatform.Profile;
using AccountingTestingPlatform.Report;
using Microsoft.Extensions.Configuration;
using Npgsql;
using QuestPDF.Infrastructure;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace AccountingTestingPlatform;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    private NpgsqlConnection _connection;

    private DataTable _tblNaturezaSaldo;
    private DataTable _tblValoresManuais;
    public MainWindow()
    {
        InitializeComponent();

        LoadConfiguration();

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

        entryRemessa2.Text = entryRemessa.Text;
    }

    private void LoadConfiguration()
    {
        var config = new ConfigurationBuilder()
                .AddIniFile("appconfig.ini", optional: false, reloadOnChange: true)
                .Build();
        entryDbHostConfig.Text = config["DbHost"];
        entryDbUserConfig.Text = config["DbUser"];
        entryDbPasswordConfig.Password = config["DbPassword"];
        entryDbNameConfig.Text = config["DbName"];

        NpgsqlConnectionStringBuilder connStr = new($"Host={config["DbHost"]};Username={config["DbUser"]};Password={config["DbPassword"]};Database={config["DbName"]}");
        _connection = new(connStr.ToString());
        _connection.Open();
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
        tabConfig.IsEnabled = tabConfig.IsEnabled;

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
            report = new PdfReport("Testes de Consistência Contábil do Encerramento Anual", remessa);
            try
            {
                TestRemessa(entryRemessa.Text, 12);
                if(!entryRemessa.Text.EndsWith("12"))
                {
                    throw new ArgumentException("A remessa de encerramento anual precisa ser a de dezembro.");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message,
                button: MessageBoxButton.OK,
                caption: "Erro na remessa:",
                icon: MessageBoxImage.Exclamation);
                ToggleEnableUI();
                entryRemessa.Focus();
                entryRemessa.SelectAll();
                return;
            }

            if (DataContext is ProgressMonitor monitor)
            {
                await Task.Run(() =>
                {
                    IProfileTest profile = new EncerramentoAnualProfileTest(_connection, remessa, report);
                    profile.Run(monitor);
                    monitor.UpdateProgress(99, "Testes terminados. Gerando relatório...");
                    report.Save();
                    monitor.UpdateProgress(100, "Relatório gerado.");
                });
            }
        }
        if (radioPad.IsChecked == true)
        {
            report = new PdfReport("Testes de Consistência dos *.txt do PAD", remessa);
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
                ToggleEnableUI();
                entryRemessa.Focus();
                entryRemessa.SelectAll();
                return;
            }

            if (DataContext is ProgressMonitor monitor)
            {
                await Task.Run(() =>
                {
                    IProfileTest profile = new ConsistenciaTxtPadProfileTest(_connection, remessa, report);
                    profile.Run(monitor);
                    monitor.UpdateProgress(99, "Testes terminados. Gerando relatório...");
                    report.Save();
                    monitor.UpdateProgress(100, "Relatório gerado.");
                });
            }
        }
        if (radioMsc.IsChecked == true)
        {
            report = new PdfReport("Testes de Consistência da MSC", remessa, true);
            try
            {
                TestRemessa(entryRemessa.Text, 13);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message,
                button: MessageBoxButton.OK,
                caption: "Erro na remessa:",
                icon: MessageBoxImage.Exclamation);
                ToggleEnableUI();
                entryRemessa.Focus();
                entryRemessa.SelectAll();
                return;
            }

            if (DataContext is ProgressMonitor monitor)
            {
                await Task.Run(() =>
                {
                    IProfileTest profile = new ConsistenciaMscProfileTest(_connection, remessa, report);
                    profile.Run(monitor);
                    monitor.UpdateProgress(99, "Testes terminados. Gerando relatório...");
                    report.Save();
                    monitor.UpdateProgress(100, "Relatório gerado.");
                });
            }
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
                ToggleEnableUI();
                entryRemessa.Focus();
                entryRemessa.SelectAll();
                return;
            }

            if(DataContext is ProgressMonitor monitor)
            {
                await Task.Run(() =>
                {
                    IProfileTest profile = new EncerramentoMensalProfileTest(_connection, remessa, report);
                    profile.Run(monitor);
                    monitor.UpdateProgress(99, "Testes terminados. Gerando relatório...");
                    report.Save();
                    monitor.UpdateProgress(100, "Relatório gerado.");
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

    private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
    {
        using (StreamWriter writer = new StreamWriter("appconfig.ini"))
        {
            writer.WriteLine($"DbHost={entryDbHostConfig.Text}");
            writer.WriteLine($"DbUser={entryDbUserConfig.Text}");
            writer.WriteLine($"DbPassword={entryDbPasswordConfig.Password}");
            writer.WriteLine($"DbName={entryDbNameConfig.Text}");
            writer.Flush();
            writer.Close();
        }

        LoadConfiguration();
        MessageBox.Show("Configurações salvas e recarregadas!");
    }

    private void btnCarregarValoresManuais_Click(object sender, RoutedEventArgs e)
    {
        string entidade;
        if (radioCm.IsChecked == true)
        {
            entidade = "cm";
        }
        else if (radioPm.IsChecked == true)
        {
            entidade = "pm";
        }
        else if (radioFpsm.IsChecked == true)
        {
            entidade = "fpsm";
        }
        else
        {
            MessageBox.Show("Precisa selecionar a entidade.",
                button: MessageBoxButton.OK,
            caption: "Erro na entidade:",
            icon: MessageBoxImage.Exclamation);
            return;
        }

        try
        {
            TestRemessa(entryRemessa2.Text, 12);
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message,
            button: MessageBoxButton.OK,
            caption: "Erro na remessa:",
            icon: MessageBoxImage.Exclamation);
            ToggleEnableUI();
            entryRemessa2.Focus();
            entryRemessa2.SelectAll();
            return;
        }

        _tblValoresManuais = ValoresManuais.ValoresManuais.MakeValoresManuaisTable(_connection, entryRemessa2.Text, entidade);

        gridValoresManuais.ItemsSource = _tblValoresManuais.DefaultView;
        btnSalvarValoresManuais.IsEnabled = true;
    }

    private void btnSalvarValoresManuais_Click(object sender, RoutedEventArgs e)
    {
        ValoresManuais.ValoresManuais.Update(_tblValoresManuais, _connection);
    }

    private void btnSelectMscMapFilepath_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.Title = "Selecione o arquivo de mapeamento da MSC";
        dialog.Multiselect = false;
        dialog.Filter = "Arquivos CSV (*.csv)|*.csv|Todos os arquivos (*.*)|*.*";
        bool? result = dialog.ShowDialog();
        if(result == true)
        {
            entryMscMapFilepath.Text = dialog.FileName;
            btnImportMscMapFilepath.IsEnabled = true;
        }
    }

    private void btnImportMscMapFilepath_Click(object sender, RoutedEventArgs e)
    {
        btnImportMscMapFilepath.IsEnabled = false;
        btnSelectMscMapFilepath.IsEnabled = false;
        entryMscMapFilepath.IsEnabled = false;
        progressImportMscMapFilepath.IsIndeterminate = true;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            MissingFieldFound = null,
            BadDataFound = null,
            HeaderValidated = null,
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim,
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };

        using (var reader = new StreamReader(entryMscMapFilepath.Text))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Context.RegisterClassMap<ColDefMap>();
            var records = csv.GetRecords<ColDef>();

            NpgsqlTransaction transaction3 = _connection.BeginTransaction();
            try
            {
                using (var cmd = new NpgsqlCommand("truncate msc.mapeamento_cc", _connection, transaction3))
                {
                    cmd.ExecuteNonQuery();
                }
                transaction3.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir registros antigos: {ex.Message}");
                transaction3.Rollback();
            }

            NpgsqlTransaction transaction2 = _connection.BeginTransaction();
            try
            {
                foreach (var record in records)
                {
                    if (record.N.ToLower() != "a")
                    {
                        continue;
                    }
                    if (!char.IsDigit(record.ContaSistema[0]))
                    {
                        continue;
                    }

                    string cc_pad = record.ContaSistema.Trim(' ').Replace(".", "").PadRight(15, '0');
                    string cc_msc = record.ContaMsc.Trim(' ').Replace(".", "").PadRight(9, '0');
                    string sql = $"insert into msc.mapeamento_cc (conta_contabil_pad, conta_contabil_msc) values ('{cc_pad}', '{cc_msc}')";
                    using (var cmd = new NpgsqlCommand(sql, _connection, transaction2))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                transaction2.Commit();
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Erro ao inserir registros: {ex.Message}");
                transaction2.Rollback();
            }

        }
        btnImportMscMapFilepath.IsEnabled = true;
        btnSelectMscMapFilepath.IsEnabled = true;
        entryMscMapFilepath.IsEnabled = true;
        MessageBox.Show("Importação concluída!");
        progressImportMscMapFilepath.IsIndeterminate = false;
    }
}

public class ColDef
{
    public string? Mapped { get; set; }
    public string? N { get; set; }
    public string? ContaSistema { get; set; }
    public string? DescricaoSistema { get; set; }
    public string? ContaMsc { get; set; }
    public string? DescricaoMsc { get; set; }
    public string? Dcl { get; set; }
}

public class ColDefMap : ClassMap<ColDef>
{
    public ColDefMap()
    {
        Map(m => m.Mapped).Index(0);
        Map(m => m.N).Index(1);
        Map(m => m.ContaSistema).Index(2);
        Map(m => m.DescricaoSistema).Index(3);
        Map(m => m.ContaMsc).Index(4);
        Map(m => m.DescricaoMsc).Index(5);
        Map(m => m.Dcl).Index(6);
    }
}