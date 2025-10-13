using AccountingTestingPlatform.Report;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.EncerramentoAnualTest.ConcreteTests
{
    class DDRUtilizadaZeradaTest : EncerramentoAnualTestBase
    {

        public DDRUtilizadaZeradaTest(NpgsqlConnection connection, string remessa, IReport report)
        {
            _connection = connection;
            _remessa = remessa;
            _report = report;
        }
        

        protected override (string, ITestResult) ExecuteTest(string entidade)
        {
            string[] sql1 = [
                $"select sum(saldo_atual)::decimal from pad.bver_enc where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '82114%';",
            ];
            string[] sql2 = [
                $"select 0::decimal;",
            ];

            List<decimal> val1 = new();
            List<decimal> val2 = new();
            DataTable dt = new DataTable();
            dt.Columns.Add("Item", typeof(string));
            dt.Columns.Add("Valor", typeof(decimal));

            foreach (string sql in sql1)
            {
                using NpgsqlCommand cmd = new(sql, _connection);
                decimal val = ExecuteSql(cmd);
                val1.Add(val);
                dt.Rows.Add(cmd.CommandText, val);
            }
            decimal total1 = 0m;
            foreach(decimal val in val1)
            {
                total1 += val;
            }
            dt.Rows.Add("Total", total1);
            
            foreach (string sql in sql2)
            {
                using NpgsqlCommand cmd = new(sql, _connection);
                decimal val = ExecuteSql(cmd);
                val2.Add(val);
                dt.Rows.Add(cmd.CommandText, val);
            }
            decimal total2 = 0m;
            foreach(decimal val in val2)
            {
                total2 += val;
            }
            dt.Rows.Add("Total", total2);

            decimal diferenca = total1 - total2;
            dt.Rows.Add("Diferença", diferenca);

            ITestResult result = new EncerramentoAnualTestResult(this);
            result.SetTestScope(_entidades[entidade]);
            if(diferenca == 0m)
            {
                result.SetSuccess(true);
            } else
            {
                result.SetSuccess(false);
            }
            result.SetResultComponent(new EncerramentoAnualTestResultTableComponent(dt));

            return ("DDR utilizada com saldo zero após encerramento", result);
        }


    }
}
