using AccountingTestingPlatform.Report;
using AccountingTestingPlatform.Test.TypeTests.ConsistenciaMscTest.ConcreteTest;
using Npgsql;

namespace AccountingTestingPlatform.Profile;

class ConsistenciaMscProfileTest : ProfileTestBase
{
    public ConsistenciaMscProfileTest(NpgsqlConnection connection, string remessa, IReport report)
    {
        TestList.Add(new BalverMscTest(connection, remessa, report));
        TestList.Add(new SaldosMscAnteriorTest(connection, remessa, report));



        //TestList.Add(new FailTest(connection, remessa, report));
    }
}
