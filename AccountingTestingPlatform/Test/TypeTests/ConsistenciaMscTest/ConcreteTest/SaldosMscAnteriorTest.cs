using AccountingTestingPlatform.Report;
using AccountingTestingPlatform.Test.TypeTests.ConsistenciaTxtPadTest;
using Npgsql;
using System.Data;
using System.Windows.Markup;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistenciaMscTest.ConcreteTest
{
    class SaldosMscAnteriorTest : ConsistenciaMscTestBase
    {

        public SaldosMscAnteriorTest(NpgsqlConnection connection, string remessa, IReport report)
        {
            _connection = connection;
            _remessa = remessa;
            _report = report;
        }

        protected override (string, ITestResult) ExecuteTest(string entidade)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("cc", typeof(string));
            dt.Columns.Add("po", typeof(string));
            dt.Columns.Add("fp", typeof(string));
            dt.Columns.Add("dc", typeof(string));
            dt.Columns.Add("efr", typeof(string));
            dt.Columns.Add("fr", typeof(string));
            dt.Columns.Add("co", typeof(string));
            dt.Columns.Add("nro", typeof(string));
            dt.Columns.Add("ndo", typeof(string));
            dt.Columns.Add("f", typeof(string));
            dt.Columns.Add("sf", typeof(string));
            dt.Columns.Add("arp", typeof(string));
            dt.Columns.Add("saldo_final", typeof(decimal));
            dt.Columns.Add("saldo_inicial", typeof(decimal));
            dt.Columns.Add("diferenca", typeof(decimal));

            string remessaAnterior = GetRemessaAnterior(_remessa);

            ITestResult result = new ConsistenciaMscTestResult(this);

            string sql = @$"WITH msc_anterior AS (
                            SELECT
                            CONCAT(
                            conta_contabil,
                            poder_orgao,
                            financeiro_permanente,
                            divida_consolidada,
                            indicador_exercicio_fonte_recurso,
                            fonte_recurso,
                            codigo_acompanhamento_orcamentario,
                            natureza_receita,
                            natureza_despesa,
                            funcao,
                            subfuncao,
                            ano_inscricao_restos_a_pagar
                            ) AS chave,
                            conta_contabil,
                            poder_orgao,
                            financeiro_permanente,
                            divida_consolidada,
                            indicador_exercicio_fonte_recurso,
                            fonte_recurso,
                            codigo_acompanhamento_orcamentario,
                            natureza_receita,
                            natureza_despesa,
                            funcao,
                            subfuncao,
                            ano_inscricao_restos_a_pagar,
                            saldo_final
                            FROM msc.msc
                            WHERE remessa = {remessaAnterior}
                            ),
                            msc_atual AS (
                            SELECT
                            CONCAT(
                            conta_contabil,
                            poder_orgao,
                            financeiro_permanente,
                            divida_consolidada,
                            indicador_exercicio_fonte_recurso,
                            fonte_recurso,
                            codigo_acompanhamento_orcamentario,
                            natureza_receita,
                            natureza_despesa,
                            funcao,
                            subfuncao,
                            ano_inscricao_restos_a_pagar
                            ) AS chave,
                            conta_contabil,
                            poder_orgao,
                            financeiro_permanente,
                            divida_consolidada,
                            indicador_exercicio_fonte_recurso,
                            fonte_recurso,
                            codigo_acompanhamento_orcamentario,
                            natureza_receita,
                            natureza_despesa,
                            funcao,
                            subfuncao,
                            ano_inscricao_restos_a_pagar,
                            saldo_inicial
                            FROM msc.msc
                            WHERE remessa = {_remessa}
                            ),
                            comparativo AS (
                            SELECT
                            msc_anterior.chave AS chave_anterior,
                            msc_anterior.conta_contabil AS conta_contabil_anterior,
                            msc_anterior.poder_orgao AS poder_orgao_anterior,
                            msc_anterior.financeiro_permanente AS financeiro_permanente_anterior,
                            msc_anterior.divida_consolidada AS divida_consolidada_anterior,
                            msc_anterior.indicador_exercicio_fonte_recurso AS indicador_exercicio_fonte_recurso_anterior,
                            msc_anterior.fonte_recurso AS fonte_recurso_anterior,
                            msc_anterior.codigo_acompanhamento_orcamentario AS codigo_acompanhamento_orcamentario_anterior,
                            msc_anterior.natureza_receita AS natureza_receita_anterior,
                            msc_anterior.natureza_despesa AS natureza_despesa_anterior,
                            msc_anterior.funcao AS funcao_anterior,
                            msc_anterior.subfuncao AS subfuncao_anterior,
                            msc_anterior.ano_inscricao_restos_a_pagar AS ano_inscricao_restos_a_pagar_anterior,
                            msc_anterior.saldo_final,
                            msc_atual.chave AS chave_atual,
                            msc_atual.conta_contabil AS conta_contabil_atual,
                            msc_atual.poder_orgao AS poder_orgao_atual,
                            msc_atual.financeiro_permanente AS financeiro_permanente_atual,
                            msc_atual.divida_consolidada AS divida_consolidada_atual,
                            msc_atual.indicador_exercicio_fonte_recurso AS indicador_exercicio_fonte_recurso_atual,
                            msc_atual.fonte_recurso AS fonte_recurso_atual,
                            msc_atual.codigo_acompanhamento_orcamentario AS codigo_acompanhamento_orcamentario_atual,
                            msc_atual.natureza_receita AS natureza_receita_atual,
                            msc_atual.natureza_despesa AS natureza_despesa_atual,
                            msc_atual.funcao AS funcao_atual,
                            msc_atual.subfuncao AS subfuncao_atual,
                            msc_atual.ano_inscricao_restos_a_pagar AS ano_inscricao_restos_a_pagar_atual,
                            msc_atual.saldo_inicial
                            FROM msc_anterior
                            FULL JOIN msc_atual ON msc_anterior.chave = msc_atual.chave
                            )

                            SELECT
                            conta_contabil_anterior,
                            poder_orgao_anterior,
                            financeiro_permanente_anterior,
                            divida_consolidada_anterior,
                            indicador_exercicio_fonte_recurso_anterior,
                            fonte_recurso_anterior,
                            codigo_acompanhamento_orcamentario_anterior,
                            natureza_receita_anterior,
                            natureza_despesa_anterior,
                            funcao_anterior,
                            subfuncao_anterior,
                            ano_inscricao_restos_a_pagar_anterior,
                            saldo_final::decimal,
                            saldo_inicial::decimal,
                            (saldo_final::decimal - saldo_inicial::decimal) AS diferenca
                            FROM comparativo
                            WHERE saldo_inicial::DECIMAL <> saldo_final::DECIMAL
                            ORDER BY conta_contabil_anterior ASC";
            
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, _connection))
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                result.SetSuccess(!reader.HasRows);
                while (reader.Read())
                {
                    string cc = reader.GetString(0);
                    int po = reader.GetInt32(1);
                    int fp = (reader.IsDBNull(2))? 0 : reader.GetInt32(2);
                    int dc = (reader.IsDBNull(3))? 0 : reader.GetInt32(3);
                    int efr = (reader.IsDBNull(4)) ? 0 : reader.GetInt32(4);
                    int fr = (reader.IsDBNull(5)) ? 0 : reader.GetInt32(5);
                    int co = (reader.IsDBNull(6)) ? 0 : reader.GetInt32(6);
                    string nro = (reader.IsDBNull(7)) ? string.Empty : reader.GetString(7);
                    string ndo = (reader.IsDBNull(8)) ? string.Empty : reader.GetString(8);
                    int f = (reader.IsDBNull(9)) ? 0 : reader.GetInt32(9);
                    int sf = (reader.IsDBNull(10)) ? 0 : reader.GetInt32(10);
                    int arp = (reader.IsDBNull(11)) ? 0 : reader.GetInt32(11);
                    decimal saldo_final = (reader.IsDBNull(12)) ? 0m : reader.GetDecimal(12);
                    decimal saldo_incial = (reader.IsDBNull(13)) ? 0m : reader.GetDecimal(13);
                    decimal diferenca = (reader.IsDBNull(14)) ? 0m : reader.GetDecimal(14);

                    dt.Rows.Add(cc, po, fp, dc, efr, fr, co, nro, ndo, f, sf, arp, saldo_final, saldo_incial, diferenca);
                }
            }
            result.SetTestScope(_entidades[entidade]);
            result.SetResultComponent(new SaldosMscAnteriorTestResultTableComponent(dt));
            
            return ("MSC Anterior x MSC Atual", result);
        }


    }
}
