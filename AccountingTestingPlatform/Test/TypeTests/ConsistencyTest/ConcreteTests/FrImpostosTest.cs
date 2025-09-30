using AccountingTestingPlatform.Report;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistencyTest.ConcreteTests
{
    class FrImpostosTest : ConsistencyTestBase
    {

        public FrImpostosTest(NpgsqlConnection connection, string remessa, IReport report)
        {
            _connection = connection;
            _remessa = remessa;
            _report = report;
            _entidades = new()
                        {
                            {"pm", "Prefeitura" },
                        };
        }
        

        protected override (string, ITestResult) ExecuteTest(string entidade)
        {
            string[] sql1 = [
                $"select sum(receita_realizada)::decimal from pad.bal_rec where remessa = {_remessa} and entidade like '{entidade}' and tipo_nivel_receita like 'A' and fonte_recurso in (500, 502) and (natureza_receita not like '111%' and natureza_receita not like '1711%' and natureza_receita not like '1721%' and natureza_receita not like '1729530101%' and natureza_receita not like '1321%');",
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

            ITestResult result = new ConsistencyTestResult(this);
            result.SetTestScope(_entidades[entidade]);
            if(diferenca == 0m)
            {
                result.SetSuccess(true);
            } else
            {
                result.SetSuccess(false);
            }
            result.SetResultComponent(new ConsistencyTestResultTableComponent(dt));

            return ("FR das receitas de impostos", result);
        }


    }
}
