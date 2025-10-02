using AccountingTestingPlatform.Report;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistenciaTxtPadTest.ConcreteTests
{
    class EmpenhoTest : ConsistenciaTxtPadTestBase
    {

        public EmpenhoTest(NpgsqlConnection connection, string remessa, IReport report)
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

            decimal diferenca = 0m;

            Dictionary<string, string> items = new()
            {
                {"Empenhado", $"select sum(valor_empenho)::decimal from pad.empenho where remessa = {_remessa} and entidade like '{entidade}' and ano_empenho = {_remessa.Substring(0, 4)};" },
                
            };
            
            ITestResult result = new ConsistenciaTxtPadTestResult(this);
            
            foreach(KeyValuePair<string, string> item in items)
            {
                string sql1 = $"select sum(valor)::decimal from auxiliar.dados_manuais where remessa = {_remessa} and entidade like '{entidade}' and item like '{item.Key}';";
                string sql2 = item.Value;

                using NpgsqlCommand cmd1 = new(sql1, _connection);
                decimal esperado = ExecuteSql(cmd1);
                
                using NpgsqlCommand cmd2 = new(sql2, _connection);
                decimal encontrado = ExecuteSql(cmd2);

                diferenca = esperado - encontrado;
                
                dt.Rows.Add(item.Key, esperado, encontrado, diferenca);
                if(diferenca == 0m)
                {
                    result.SetSuccess(true);
                } else
                {
                    result.SetSuccess(false);
                }
            }
            
            result.SetTestScope(_entidades[entidade]);
            result.SetResultComponent(new ConsistenciaTxtPadTestResultTableComponent(dt));
            
            return ("EMPENHO", result);
        }


    }
}
