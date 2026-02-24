using AccountingTestingPlatform.Report;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.RecursosExercicioTest.ConcreteTests
{
    class RecursosExercicioTest : RecursosExercicioTestBase
    {
        public RecursosExercicioTest(NpgsqlConnection connection, string remessa, IReport report)
        {
            _connection = connection;
            _remessa = remessa;
            _report = report;
        }
        
        protected override (string, ITestResult) ExecuteTest(string entidade)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FR", typeof(int));
            dt.Columns.Add("Receita arrecadada", typeof(decimal));
            dt.Columns.Add("Empenhos do exercício", typeof(decimal));
            dt.Columns.Add("Créditos abertos por superávit", typeof(decimal));
            dt.Columns.Add("Créditos extraordinários", typeof(decimal));
            //dt.Columns.Add("Créditos reabertos", typeof(decimal));
            dt.Columns.Add("Transferências recebidas", typeof(decimal));
            dt.Columns.Add("Transferências concedidas", typeof(decimal));
            dt.Columns.Add("Ajustes", typeof(decimal));
            dt.Columns.Add("Saldo disponível", typeof(decimal));
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
	                        (SELECT SUM(receita_realizada)::DECIMAL FROM PAD.bal_rec
	                        WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
	                        AND fonte_recurso = t1.recurso_vinculado
	                        AND tipo_nivel_receita LIKE 'A')
                        , 0.0) AS receita_arrecadada,
                        COALESCE(
	                        (SELECT SUM(valor_empenhado)::DECIMAL FROM PAD.bal_desp
	                        WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
	                        AND fonte_recurso = t1.recurso_vinculado)
                        , 0.0) AS empenhos_exercicio,
                        COALESCE(
	                        (SELECT SUM(valor_credito_adicional)::DECIMAL FROM PAD.decreto
	                        WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
	                        AND fonte_recurso_suplementacao = t1.recurso_vinculado
	                        AND origem_recurso = 1)
                        , 0.0) AS credito_superavit,
                        COALESCE(
	                        (SELECT SUM(valor_credito_adicional)::DECIMAL FROM PAD.decreto
	                        WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
	                        AND fonte_recurso_suplementacao = t1.recurso_vinculado
	                        AND origem_recurso <> 1 AND tipo_credito_adicional = 3)
                        , 0.0) AS credito_extraordinario,
                        --COALESCE(
	                    --    (SELECT SUM(valor_saldo_reaberto)::DECIMAL FROM PAD.decreto
	                    --    WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
	                    --    AND fonte_recurso_suplementacao = t1.recurso_vinculado)
                        --, 0.0) AS credito_reaberto,
                        COALESCE(
	                        (SELECT SUM(saldo_atual)::DECIMAL FROM PAD.bal_ver
	                        WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
	                        AND fonte_recurso = t1.recurso_vinculado
	                        AND conta_contabil LIKE '45112%' AND escrituracao LIKE 'S')
                        , 0.0) AS transferencia_recebida,
                        COALESCE(
	                        (SELECT SUM(saldo_atual)::DECIMAL FROM PAD.bal_ver
	                        WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
	                        AND fonte_recurso = t1.recurso_vinculado
	                        AND conta_contabil LIKE '35112%' AND escrituracao LIKE 'S')
                        , 0.0) AS transferencia_concedida,
                        COALESCE(
	                        (SELECT SUM(valor_ajuste)::DECIMAL FROM auxiliar.ajustes_teste_fr_exercicio
	                        WHERE entidade LIKE '{entidade}'
	                        AND fonte_recurso = t1.recurso_vinculado)
                        , 0.0) AS ajustes
                        FROM t1
                        ),
                        t3 AS (
                        SELECT t2.*,
                        (
                        --receita_arrecadada - empenhos_exercicio + credito_superavit + credito_extraordinario
                        --+ credito_reaberto + transferencia_recebida - transferencia_concedida + ajustes
                        receita_arrecadada - empenhos_exercicio + credito_superavit + credito_extraordinario
                        + transferencia_recebida - transferencia_concedida + ajustes
                        ) AS disponivel
                        FROM t2
                        ),
                        t4 AS (
                        SELECT t3.*,
                        COALESCE(
	                        (SELECT SUM(saldo_atual)::DECIMAL FROM PAD.bal_ver
	                        WHERE remessa = {_remessa} AND entidade LIKE '{entidade}'
	                        AND fonte_recurso = t3.recurso_vinculado
	                        AND conta_contabil LIKE '8211101%' AND escrituracao LIKE 'S')
                        , 0.0) AS contabil
                        FROM t3
                        ),
                        t5 AS (
                        SELECT t4.*,
                        (disponivel - contabil) AS diferenca
                        FROM t4
                        )

                        SELECT * FROM t5";

            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, _connection))
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    //string fr = reader.GetInt16(0).ToString();
                    //decimal receita = reader.GetDecimal(1);
                    //decimal empenhos = reader.GetDecimal(2);
                    //decimal creditos_superavit = reader.GetDecimal(3);
                    //decimal extraordinarios = reader.GetDecimal(4);
                    //decimal reabertos = reader.GetDecimal(5);
                    //decimal transf_recebidas = reader.GetDecimal(6);
                    //decimal transf_concedidas = reader.GetDecimal(7);
                    //decimal ajustes = reader.GetDecimal(8);
                    //decimal disponivel = reader.GetDecimal(9);
                    //decimal saldo_contabil = reader.GetDecimal(10);
                    //decimal diferenca = reader.GetDecimal(11);
                    string fr = reader.GetInt16(0).ToString();
                    decimal receita = reader.GetDecimal(1);
                    decimal empenhos = reader.GetDecimal(2);
                    decimal creditos_superavit = reader.GetDecimal(3);
                    decimal extraordinarios = reader.GetDecimal(4);
                    decimal transf_recebidas = reader.GetDecimal(5);
                    decimal transf_concedidas = reader.GetDecimal(6);
                    decimal ajustes = reader.GetDecimal(7);
                    decimal disponivel = reader.GetDecimal(8);
                    decimal saldo_contabil = reader.GetDecimal(9);
                    decimal diferenca = reader.GetDecimal(10);

                    if (diferenca != 0m)
                    {
                        fails = true;
                    }

                    //dt.Rows.Add(fr, receita, empenhos, creditos_superavit, extraordinarios, reabertos, transf_recebidas, transf_concedidas, ajustes, disponivel, saldo_contabil, diferenca);
                    dt.Rows.Add(fr, receita, empenhos, creditos_superavit, extraordinarios, transf_recebidas, transf_concedidas, ajustes, disponivel, saldo_contabil, diferenca);
                }
            }






            ITestResult result = new RecursosExercicioTestResult(this);
            result.SetTestScope(_entidades[entidade]);
            result.SetSuccess(!fails);
            result.SetResultComponent(new RecursosExercicioTestResultTableComponent(dt));

            return ("Conferência dos recursos disponíveis para o exercício por fonte de recurso", result);
        }


    }
}
