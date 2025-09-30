using AccountingTestingPlatform.Report;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.SaldosSuperavitTest.ConcreteTests
{
    class SaldosSuperavitTest : SaldosSuperavitTestBase
    {
        public SaldosSuperavitTest(NpgsqlConnection connection, string remessa, IReport report)
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
	                            (SELECT SUM(saldo_inicial)::DECIMAL FROM PAD.bal_ver
	                            WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
	                            AND fonte_recurso = t1.recurso_vinculado
	                            AND escrituracao LIKE 'S' AND conta_contabil LIKE '11%' AND indicador_superavit_financeiro LIKE 'F')
                            , 0.0) AS saldo_financeiro_bruto,
                            COALESCE(
	                            (SELECT SUM(rp_saldo_inicial)::DECIMAL FROM PAD.restos_pagar
	                            WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
	                            AND fonte_recurso = t1.recurso_vinculado)
                            , 0.0) AS saldo_inicial_rp
                            FROM t1
                            ),
                            t3 AS (
                            SELECT t2.*,
                            (
                            saldo_financeiro_bruto - saldo_inicial_rp) AS superavit_inicial
                            FROM t2
                            ),
                            t4 AS (
                            SELECT t3.*,
                            COALESCE(
	                            (SELECT SUM(rp_cancelado)::DECIMAL FROM PAD.restos_pagar
	                            WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
	                            AND fonte_recurso = t3.recurso_vinculado)
                            , 0.0) AS cancelamento_rp,
                            COALESCE(
	                            (SELECT SUM(valor_credito_adicional)::DECIMAL FROM PAD.decreto
	                            WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
	                            AND fonte_recurso_suplementacao = t3.recurso_vinculado
	                            AND origem_recurso = 1)
                            , 0.0) AS creditos_abertos
                            FROM t3
                            ),
                            t5 AS (
                            SELECT t4.*,
                            (superavit_inicial + cancelamento_rp - creditos_abertos) AS saldo_superavit
                            FROM t4
                            ),
                            t6 AS (
                            SELECT t5.*,
                            COALESCE(
	                            (SELECT SUM(saldo_atual)::DECIMAL FROM PAD.bal_ver
	                            WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
	                            AND fonte_recurso = t5.recurso_vinculado
	                            AND escrituracao LIKE 'S' AND conta_contabil LIKE '8211102%')
                            , 0.0) AS saldo_atual_contabil
                            FROM t5
                            )

                            SELECT t6.*,
                            (saldo_superavit - saldo_atual_contabil) AS diferenca
                            FROM t6";

            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, _connection))
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string fr = reader.GetInt16(0).ToString();
                    decimal saldo_bruto = reader.GetDecimal(1);
                    decimal saldo_rp = reader.GetDecimal(2);
                    decimal superavit_inicial = reader.GetDecimal(3);
                    decimal cancelamento_rp = reader.GetDecimal(4);
                    decimal creditos_abertos = reader.GetDecimal(5);
                    decimal saldo_superavit = reader.GetDecimal(6);
                    decimal saldo_contabil = reader.GetDecimal(7);
                    decimal diferenca = reader.GetDecimal(8);

                    if (diferenca != 0m)
                    {
                        fails = true;
                    }

                    dt.Rows.Add(fr, saldo_bruto, saldo_rp, superavit_inicial, cancelamento_rp, creditos_abertos, saldo_superavit, saldo_contabil, diferenca);
                }
            }






            ITestResult result = new SaldosSuperavitTestResult(this);
            result.SetTestScope(_entidades[entidade]);
            result.SetSuccess(!fails);
            result.SetResultComponent(new SaldosSuperavitTestResultTableComponent(dt));

            return ("Conferência dos saldos de superávit por fonte de recurso", result);
        }


    }
}
