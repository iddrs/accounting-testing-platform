using AccountingTestingPlatform.Report;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistenciaTxtPadTest.ConcreteTests
{
    class OrgaosTest : ConsistenciaTxtPadTestBase
    {

        public OrgaosTest(NpgsqlConnection connection, string remessa, IReport report)
        {
            _connection = connection;
            _remessa = remessa;
            _report = report;
        }

        protected override (string, ITestResult) ExecuteTest(string entidade)
        {
            List<decimal> val1 = new();
            List<decimal> val2 = new();
            DataTable dt = new DataTable();
            dt.Columns.Add("Item", typeof(string));
            dt.Columns.Add("Esperado", typeof(decimal));
            dt.Columns.Add("Encontrado", typeof(decimal));
            dt.Columns.Add("Diferença", typeof(decimal));

            int diferenca = 0;

            Dictionary<string, string> orgaos = new()
            {
                {"cm", "2,3,4,5,6,7,8,9,10,11,12,13,50" },
                {"pm", "1,12,50" },
                {"fpsm", "1,2,3,4,5,6,7,8,9,10,11,13" },
                
            };
            List<(string, string)> tabelas = [
                ("bal_desp", "orgao"),
                ("bal_rec", "orgao"),
                ("bal_ver", "orgao"),
                ("bver_enc", "orgao"),
                ("empenho", "orgao"),
                ("pagament", "orgao_debito"),
                ("pagament", "orgao_credito")
            ];
            
            ITestResult result = new ConsistenciaTxtPadTestResult(this);
            
            foreach ((string, string) item in tabelas)
            {
                string orgaoValues = orgaos[entidade];
                string sql = $"select count(*) from pad.{item.Item1} where remessa = {_remessa} and entidade like '{entidade}' and {item.Item2} in ({orgaoValues});";

                using NpgsqlCommand cmd = new(sql, _connection);
                decimal encontrado = ExecuteSql(cmd);
                
                diferenca = 0 - Decimal.ToInt32(encontrado);
                
                dt.Rows.Add($"{item.Item1} : {item.Item2}", "0", encontrado, diferenca);
                if (diferenca == 0)
                {
                    result.SetSuccess(true);
                }
                else
                {
                    result.SetSuccess(false);
                }
            }
            
            result.SetTestScope(_entidades[entidade]);
            result.SetResultComponent(new OrgaoConsistenciaTxtPadTestResultTableComponent(dt));
            
            return ("ORGAOS", result);
        }


    }
}
