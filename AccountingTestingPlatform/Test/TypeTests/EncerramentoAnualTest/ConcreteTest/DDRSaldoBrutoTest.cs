using AccountingTestingPlatform.Report;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.EncerramentoAnualTest.ConcreteTests
{
    class DDRSaldoBrutoTest : DDRSaldoBrutoTestBase
    {
        public DDRSaldoBrutoTest(NpgsqlConnection connection, string remessa, IReport report)
        {
            _connection = connection;
            _remessa = remessa;
            _report = report;
        }
        
        protected override (string, ITestResult) ExecuteTest(string entidade)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FR", typeof(int));
            dt.Columns.Add("Saldo financeiro bruto", typeof(decimal));
            dt.Columns.Add("Saldo inicial de RP", typeof(decimal));
            dt.Columns.Add("Superávit financeiro inicial", typeof(decimal));
            dt.Columns.Add("Cancelamento de RP", typeof(decimal));
            dt.Columns.Add("Créditos abertos", typeof(decimal));
            dt.Columns.Add("Saldo de superávit", typeof(decimal));
            dt.Columns.Add("Saldo atual contábil", typeof(decimal));
            dt.Columns.Add("Diferença", typeof(decimal));

            bool fails = false;

            string sql = $@"WITH t1 AS (
                        SELECT DISTINCT
                        recurso_vinculado
                        from pad.recursos
                        WHERE remessa = {_remessa}
                        ORDER BY recurso_vinculado ASC
                        ),
                        t2 AS (
                        SELECT
                        t1.*,
                        COALESCE(
                            (SELECT SUM(saldo_inicial)::DECIMAL FROM PAD.bver_enc
                            WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
                            AND fonte_recurso = t1.recurso_vinculado
                            AND escrituracao LIKE 'S' AND (conta_contabil LIKE '111%' OR conta_contabil LIKE '114%') AND indicador_superavit_financeiro LIKE 'F')
                        , 0.0) AS saldo_financeiro_bruto
                        FROM t1
                        ),
                        t3 AS (
                        SELECT t2.*,
                        COALESCE(
                            (SELECT SUM(saldo_atual)::DECIMAL FROM PAD.bver_enc
                            WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
                            AND fonte_recurso = t2.recurso_vinculado
                            AND escrituracao LIKE 'S' AND conta_contabil LIKE '7211%')
                        , 0.0) AS saldo_atual_contabil
                        FROM t2
                        )

                        SELECT t3.*,
                        (saldo_financeiro_bruto - saldo_atual_contabil) AS diferenca
                        FROM t3";

            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, _connection))
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string fr = reader.GetInt16(0).ToString();
                    decimal saldo_bruto = reader.GetDecimal(1);
                    decimal saldo_contabil = reader.GetDecimal(2);
                    decimal diferenca = reader.GetDecimal(3);

                    if (diferenca != 0m)
                    {
                        fails = true;
                    }

                    dt.Rows.Add(fr, saldo_bruto, saldo_contabil, diferenca);
                }
            }






            ITestResult result = new DDRSaldoBrutoTestResult(this);
            result.SetTestScope(_entidades[entidade]);
            result.SetSuccess(!fails);
            result.SetResultComponent(new DDRSaldoBrutoTestResultTableComponent(dt));

            return ("Conferência do saldo bruto das DDR por fonte de recurso", result);
        }


    }
}
