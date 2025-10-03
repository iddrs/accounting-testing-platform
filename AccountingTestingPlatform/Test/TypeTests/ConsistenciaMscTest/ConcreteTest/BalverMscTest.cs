using AccountingTestingPlatform.Report;
using AccountingTestingPlatform.Test.TypeTests.ConsistenciaTxtPadTest;
using Npgsql;
using System.Data;
using System.Windows.Markup;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistenciaMscTest.ConcreteTest
{
    class BalverMscTest : ConsistenciaMscTestBase
    {

        public BalverMscTest(NpgsqlConnection connection, string remessa, IReport report)
        {
            _connection = connection;
            _remessa = remessa;
            _report = report;
        }

        protected override (string, ITestResult) ExecuteTest(string entidade)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CC bal ver", typeof(string));
            dt.Columns.Add("CC msc", typeof(string));
            dt.Columns.Add("Saldo anterior bal ver", typeof(decimal));
            dt.Columns.Add("Saldo anterior msc", typeof(decimal));
            dt.Columns.Add("Diferença saldo anterior", typeof(decimal));
            dt.Columns.Add("Débito bal ver", typeof(decimal));
            dt.Columns.Add("Débito msc", typeof(decimal));
            dt.Columns.Add("Diferença débito", typeof(decimal));
            dt.Columns.Add("Crédito bal ver", typeof(decimal));
            dt.Columns.Add("Crédito msc", typeof(decimal));
            dt.Columns.Add("Diferença crédito", typeof(decimal));
            dt.Columns.Add("Saldo atual bal ver", typeof(decimal));
            dt.Columns.Add("Saldo atual msc", typeof(decimal));
            dt.Columns.Add("Diferença saldo atual", typeof(decimal));

            string inicioMes = $"{_remessa.Substring(0, 4)}-{_remessa.Substring(4, 2)}-01";
            string fimMes = DateTime.Parse(inicioMes).AddMonths(1).ToString("yyyy-MM-dd");
            string remessaAnterior = GetRemessaAnterior(_remessa);

            ITestResult result = new ConsistenciaMscTestResult(this);

            string sql = @$"-- SET statement_timeout = '1200s';
                            WITH t1 AS (
                                SELECT DISTINCT conta_contabil
                                FROM PAD.bal_ver
                                WHERE remessa = {_remessa}
                                  AND escrituracao = 'S'
                            ),
                            lancamentos_agregados AS (
                                SELECT 
                                    conta_contabil,
                                    SUM(CASE WHEN tipo_lancamento = 'D' THEN valor_lancamento ELSE 0::money END) AS debitos,
                                    SUM(CASE WHEN tipo_lancamento = 'C' THEN valor_lancamento ELSE 0::money END) AS creditos
                                FROM PAD.tce_4111
                                WHERE remessa = {_remessa}
                                  AND data_lancamento >= '{inicioMes}'
                                  AND data_lancamento < '{fimMes}'
                                  AND tipo_lancamento IN ('D', 'C')
                                GROUP BY conta_contabil
                            ),
                            saldos_anteriores AS (
                                SELECT 
                                    conta_contabil,
                                    SUM(saldo_atual) AS saldo_anterior
                                FROM PAD.bal_ver
                                WHERE remessa = {remessaAnterior}
                                GROUP BY conta_contabil
                            ),
                            t2 AS (
                            SELECT 
                                t1.conta_contabil,
                                COALESCE(sa.saldo_anterior, 0::money) AS saldo_anterior,
                                COALESCE(la.debitos, 0::money) AS debitos,
                                COALESCE(la.creditos, 0::money) AS creditos
                            FROM t1
                            LEFT JOIN saldos_anteriores sa ON sa.conta_contabil = t1.conta_contabil
                            LEFT JOIN lancamentos_agregados la ON la.conta_contabil = t1.conta_contabil
                            ORDER BY t1.conta_contabil
                            ),
                            t3 AS (
                            SELECT
                            conta_contabil,
                            SUM(saldo_atual) AS saldo_atual
                            FROM PAD.bal_ver
                            WHERE remessa = {_remessa}
                            GROUP BY conta_contabil
                            ),
                            t4 AS (
                            SELECT
                            t2.*,
                            t3.saldo_atual
                            FROM t2
                            INNER JOIN t3
                            ON t2.conta_contabil LIKE t3.conta_contabil
                            ),
                            t5 AS (
                            SELECT
                            t4.*,
                            COALESCE(m.conta_contabil_msc, LEFT(t4.conta_contabil, 9)) AS cc
                            FROM t4
                            LEFT JOIN msc.mapeamento_cc M
                            ON m.conta_contabil_pad = t4.conta_contabil
                            ),
                            balver_mensal AS (
                            SELECT
                            cc AS conta_contabil_balver,
                            SUM(saldo_anterior) AS saldo_anterior_balver,
                            SUM(debitos) AS debitos_balver,
                            SUM(creditos) AS creditos_balver,
                            SUM(saldo_atual) AS saldo_atual_balver
                            FROM t5
                            GROUP BY cc
                            ORDER BY conta_contabil_balver ASC
                            ),
                            t6 AS (
                            SELECT
                            conta_contabil AS conta_contabil_msc,
                            SUM(saldo_inicial) AS saldo_anterior_msc,
                            SUM(movimento_debito) AS debitos_msc,
                            SUM(movimento_credito) AS creditos_msc,
                            SUM(saldo_final) AS saldo_atual_msc
                            FROM msc.msc
                            WHERE remessa = {_remessa}
                            GROUP BY conta_contabil
                            ),
                            t7 AS (
                            SELECT
                            b.*,
                            t6.*
                            FROM balver_mensal b
                            FULL JOIN t6
                            ON b.conta_contabil_balver = t6.conta_contabil_msc
                            ),
                            t8 AS (
                            SELECT
                            conta_contabil_balver,
                            conta_contabil_msc,
                            COALESCE(saldo_anterior_balver, 0::money) AS saldo_anterior_balver,
                            COALESCE(saldo_anterior_msc, 0::money) AS saldo_anterior_msc,
                            COALESCE(debitos_balver, 0::money) AS debitos_balver,
                            COALESCE(debitos_msc, 0::money) AS debitos_msc,
                            COALESCE(creditos_balver, 0::money) AS creditos_balver,
                            COALESCE(creditos_msc, 0::money) AS creditos_msc,
                            COALESCE(saldo_atual_balver, 0::money) AS saldo_atual_balver,
                            COALESCE(saldo_atual_msc, 0::money) AS saldo_atual_msc
                            FROM t7
                            ),
                            t9 AS (
                            SELECT
                            conta_contabil_balver,
                            conta_contabil_msc,
                            saldo_anterior_balver,
                            saldo_anterior_msc,
                            (saldo_anterior_balver - saldo_anterior_msc) AS saldo_anterior_diferenca,
                            debitos_balver,
                            debitos_msc,
                            (debitos_balver - debitos_msc) AS debitos_diferenca,
                            creditos_balver,
                            creditos_msc,
                            (creditos_balver - creditos_msc) AS creditos_diferenca,
                            saldo_atual_balver,
                            saldo_atual_msc,
                            (saldo_atual_balver - saldo_atual_msc) AS saldo_atual_diferenca
                            FROM t8
                            )

                            SELECT * FROM t9
                            WHERE
                            conta_contabil_balver IS NULL
                            OR conta_contabil_msc IS NULL
                            OR saldo_anterior_diferenca::DECIMAL <> 0.0
                            OR debitos_diferenca::DECIMAL <> 0.0
                            OR creditos_diferenca::DECIMAL <> 0.0
                            OR saldo_atual_diferenca::DECIMAL <> 0.0";

            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, _connection))
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                result.SetSuccess(!reader.HasRows);
                while (reader.Read())
                {
                    string cc_balver = (reader.IsDBNull(0))? string.Empty : reader.GetString(0);
                    string cc_msc = (reader.IsDBNull(1)) ? string.Empty : reader.GetString(1);


                    decimal saldo_anterior_balver = (reader.IsDBNull(2)) ? 0m : reader.GetDecimal(2);
                    decimal saldo_anterior_msc = (reader.IsDBNull(3)) ? 0m : reader.GetDecimal(3);
                    decimal saldo_anterior_diferenca = (reader.IsDBNull(4)) ? 0m : reader.GetDecimal(4);
                    decimal debito_balver = (reader.IsDBNull(5)) ? 0m : reader.GetDecimal(5);
                    decimal debito_msc = (reader.IsDBNull(6)) ? 0m : reader.GetDecimal(6);
                    decimal debito_diferenca = (reader.IsDBNull(7)) ? 0m : reader.GetDecimal(7);
                    decimal credito_balver = (reader.IsDBNull(8)) ? 0m : reader.GetDecimal(8);
                    decimal credito_msc = (reader.IsDBNull(9)) ? 0m : reader.GetDecimal(9);
                    decimal credito_diferenca = (reader.IsDBNull(10)) ? 0m : reader.GetDecimal(10);
                    decimal saldo_atual_balver = (reader.IsDBNull(11)) ? 0m : reader.GetDecimal(11);
                    decimal saldo_atual_msc = (reader.IsDBNull(12)) ? 0m : reader.GetDecimal(12);
                    decimal saldo_atual_diferenca = (reader.IsDBNull(13)) ? 0m : reader.GetDecimal(13);


                    dt.Rows.Add(cc_balver, cc_msc, saldo_anterior_balver, saldo_anterior_msc, saldo_anterior_diferenca, debito_balver, debito_msc, debito_diferenca, credito_balver, credito_msc, credito_diferenca, saldo_atual_balver, saldo_atual_msc, saldo_atual_diferenca);
                }
            }
            result.SetTestScope(_entidades[entidade]);
            result.SetResultComponent(new BalverMscTestResultTableComponent(dt));
            
            return ("Bal Ver x MSC", result);
        }


    }
}
