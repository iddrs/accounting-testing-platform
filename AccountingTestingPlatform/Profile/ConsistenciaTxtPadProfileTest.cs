using AccountingTestingPlatform.Report;
using AccountingTestingPlatform.Test.TypeTests.ConsistenciaTxtPadTest.ConcreteTests;
using Npgsql;

namespace AccountingTestingPlatform.Profile;

class ConsistenciaTxtPadProfileTest : ProfileTestBase
{
    public ConsistenciaTxtPadProfileTest(NpgsqlConnection connection, string remessa, IReport report)
    {
        TestList.Add(new BalDespTest(connection, remessa, report));
        TestList.Add(new BalRecTest(connection, remessa, report));
        TestList.Add(new BalVerTest(connection, remessa, report));
        TestList.Add(new DecretoTest(connection, remessa, report));
        TestList.Add(new EmpenhoTest(connection, remessa, report));
        TestList.Add(new LiquidacTest(connection, remessa, report));
        TestList.Add(new PagamentTest(connection, remessa, report));
        TestList.Add(new RestosPagarTest(connection, remessa, report));
        TestList.Add(new OrgaosTest(connection, remessa, report));



        //TestList.Add(new FailTest(connection, remessa, report));
    }
}
