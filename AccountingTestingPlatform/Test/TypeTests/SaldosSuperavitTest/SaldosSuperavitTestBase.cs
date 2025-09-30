namespace AccountingTestingPlatform.Test.TypeTests.SaldosSuperavitTest;

internal abstract class SaldosSuperavitTestBase : TestBase
{
    protected Dictionary<string, string> _entidades = new()
    {
        {"pm", "Prefeitura" },
        {"cm", "Câmara" },
        {"fpsm", "FPSM" },
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

}
