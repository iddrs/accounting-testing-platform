using AccountingTestingPlatform.Report;
using AccountingTestingPlatform.Test.TypeTests.EncerramentoAnualTest.ConcreteTests;
using Npgsql;

namespace AccountingTestingPlatform.Profile;

class EncerramentoAnualProfileTest : ProfileTestBase
{
    public EncerramentoAnualProfileTest(NpgsqlConnection connection, string remessa, IReport report)
    {
        TestList.Add(new AtivoPassivoIntraTest(connection, remessa, report));
        TestList.Add(new AtivoSemMovimentoDevedorTest(connection, remessa, report));
        TestList.Add(new AtivoSemMovimentoCredorTest(connection, remessa, report));
        TestList.Add(new PassivoSemMovimentoDevedorTest(connection, remessa, report));
        TestList.Add(new ResultadoDoExercicioConsolidacaoTest(connection, remessa, report));
        TestList.Add(new ResultadoDoExercicioIntraTest(connection, remessa, report));
        TestList.Add(new ResultadoDoExercicioInterUniaoTest(connection, remessa, report));
        TestList.Add(new ResultadoDoExercicioInterEstadoTest(connection, remessa, report));
        TestList.Add(new ResultadoDoExercicioInterMunicipioTest(connection, remessa, report));
        TestList.Add(new VPDZeradaTest(connection, remessa, report));
        TestList.Add(new VPAZeradaTest(connection, remessa, report));
        TestList.Add(new CAPOZeradaTest(connection, remessa, report));
        TestList.Add(new CEPOZeradaTest(connection, remessa, report));
        TestList.Add(new RpNpInscricaoNoExercicioTest(connection, remessa, report));
        TestList.Add(new RppInscricaoNoExercicioTest(connection, remessa, report));
        TestList.Add(new RpNpPagosZeradoTest(connection, remessa, report));
        TestList.Add(new RpNpCanceladosZeradoTest(connection, remessa, report));
        TestList.Add(new RpNpInscritosNoExercicioTest(connection, remessa, report));
        TestList.Add(new RppInscritosNoExercicioTest(connection, remessa, report));
        TestList.Add(new RppPagosZeradoTest(connection, remessa, report));
        TestList.Add(new RppCanceladosZeradoTest(connection, remessa, report));
        TestList.Add(new DDRSaldoBrutoTest(connection, remessa, report));
        TestList.Add(new DDRUtilizadaZeradaTest(connection, remessa, report));



        //TestList.Add(new FailTest(connection, remessa, report));
    }
}
