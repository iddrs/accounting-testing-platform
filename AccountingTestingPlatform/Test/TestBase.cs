using AccountingTestingPlatform.Report;
using Npgsql;

namespace AccountingTestingPlatform.Test;

internal abstract class TestBase : ITest
{
    protected string _remessa;
    protected NpgsqlConnection _connection;
    protected IReport _report;
    public abstract void Run();

    protected static int GetAnoRemessa(string remessa)
    {
        return int.Parse(remessa.Substring(0, 4));
    }

    protected static (string, string) GetLimitesRemessa(string remessa)
    {
        int ano = GetAnoRemessa(remessa);
        int mes = int.Parse(remessa.Substring(4, 2));
        DateTime dt0 = new DateTime(ano, 1, 1);
        DateTime dt1 = new DateTime(ano, mes, 31);

        return (dt0.ToString("yyyy-MM-dd"), dt1.ToString("yyyy-MM-dd"));
    }

}
