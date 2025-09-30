using AccountingTestingPlatform.Report;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.SaldosInvertidosTest.ConcreteTests
{
    class SaldosInvertidosTest : SaldosInvertidosTestBase
    {
        public SaldosInvertidosTest(NpgsqlConnection connection, string remessa, IReport report)
        {
            _connection = connection;
            _remessa = remessa;
            _report = report;
        }
        
        protected override (string, ITestResult) ExecuteTest(string entidade)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Código", typeof(string));
            dt.Columns.Add("Conta Contábil", typeof(string));
            dt.Columns.Add("Saldo", typeof(decimal));
            dt.Columns.Add("Natureza Encontrada", typeof(string));
            dt.Columns.Add("Natureza Esperada", typeof(string));

            bool fails = false;

            string sql = $@"WITH t0 AS (
                            SELECT
	                            conta_contabil,
	                            especificacao_conta_contabil,
	                            SUM(saldo_atual)::DECIMAL AS saldo_atual
                            FROM PAD.bal_ver
                            WHERE remessa = {_remessa}
                            AND entidade LIKE '{entidade}'
                            GROUP BY
	                            conta_contabil,
	                            especificacao_conta_contabil
                            ORDER BY 
	                            conta_contabil ASC
                            ),
                            t1 AS (
                            SELECT
                            b.conta_contabil,
                            b.especificacao_conta_contabil,
                            b.saldo_atual,
                            CASE
	                            WHEN (SUBSTR(b.conta_contabil, 1, 1) IN ('1', '3', '5', '7') AND b.saldo_atual::DECIMAL > 0.0) THEN 'D'
	                            WHEN (SUBSTR(b.conta_contabil, 1, 1) IN ('1', '3', '5', '7') AND b.saldo_atual::DECIMAL < 0.0) THEN 'C'
	                            WHEN (SUBSTR(b.conta_contabil, 1, 1) IN ('2', '4', '6', '8') AND b.saldo_atual::DECIMAL > 0.0) THEN 'C'
	                            WHEN (SUBSTR(b.conta_contabil, 1, 1) IN ('2', '4', '6', '8') AND b.saldo_atual::DECIMAL < 0.0) THEN 'D'
	                            ELSE ''
                            END natureza_encontrada,
                            CASE
	                            WHEN
		                            (SELECT t.conta_contabil FROM auxiliar.natureza_saldos t WHERE t.conta_contabil = b.conta_contabil)
		                            IS NOT NULL
	                            THEN
		                            CASE 
			                            WHEN (SELECT t.devedora FROM auxiliar.natureza_saldos t WHERE t.conta_contabil = b.conta_contabil AND t.credora = 0) = 1
			                            THEN 'D'
		                            WHEN (SELECT t.credora FROM auxiliar.natureza_saldos t WHERE t.conta_contabil = b.conta_contabil AND t.devedora = 0) = 1
			                            THEN 'C'
		                            ELSE 
			                            'DC'
		                            END
	                            WHEN SUBSTR(b.conta_contabil, 1, 1) IN ('1', '3', '5', '7') THEN 'D'
	                            WHEN SUBSTR(b.conta_contabil, 1, 1) IN ('2', '4', '6', '8') THEN 'C'
	                            ELSE ''
                            END natureza_esperada
                            FROM t0 b
                            WHERE
                            b.saldo_atual::decimal <> 0.0
                            ORDER BY b.conta_contabil ASC
                            ),

                            t2 AS (
                            SELECT * FROM t1 WHERE natureza_esperada != natureza_encontrada AND natureza_esperada NOT LIKE 'DC'
                            )

                            SELECT * FROM t2";

            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, _connection))
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                if(reader.HasRows)
                {
                    fails = true;
                }
                while (reader.Read())
                {
                    string cod = reader.GetString(0);
                    string cc = reader.GetString(1);
                    decimal saldo = reader.GetDecimal(2);
                    string encontrado = reader.GetString(3);
                    string esperado = reader.GetString(4);
                    
                    dt.Rows.Add(cod, cc, saldo, encontrado, esperado);
                }
            }






            ITestResult result = new SaldosInvertidosTestResult(this);
            result.SetTestScope(_entidades[entidade]);
            result.SetSuccess(!fails);
            result.SetResultComponent(new SaldosInvertidosTestResultTableComponent(dt));

            return ("Conferência de contas contábeis com saldo invertido", result);
        }


    }
}
