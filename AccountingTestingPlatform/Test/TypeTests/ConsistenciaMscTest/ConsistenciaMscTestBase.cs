using Npgsql;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistenciaMscTest;

internal abstract class ConsistenciaMscTestBase : TestBase
{
    protected Dictionary<string, string> _entidades = new()
    {
        {"mun", "Agregado" },
    };

    public override void Run()
    {
        List<ITestResult> results = new();
        string testName = "";
        foreach (KeyValuePair<string, string> item in _entidades)
        {
            (string, ITestResult) result = ExecuteTest(item.Key);
            results.Add(result.Item2);
            testName = result.Item1;
        }
        _report.AddTest(testName, results);
    }

    protected abstract (string, ITestResult) ExecuteTest(string entidade);

    protected decimal ExecuteSql(NpgsqlCommand cmd)
    {
        object result = cmd.ExecuteScalar();
        decimal valor = 0m;
        if (result != null && !string.IsNullOrEmpty(result.ToString()) && !string.IsNullOrWhiteSpace(result.ToString()))
        {
            valor = decimal.Parse(result.ToString());
        }
        return valor;
    }

    protected string GetRemessaAnterior(string remessa)
    {
        int ano = int.Parse(remessa.Substring(0, 4));
        int mes = int.Parse(remessa.Substring(4, 2));
        mes--;
        if (mes == 0)
        {
            mes = 13;
            ano--;
        }
        return $"{ano}{mes.ToString("D2")}";
    }
}
