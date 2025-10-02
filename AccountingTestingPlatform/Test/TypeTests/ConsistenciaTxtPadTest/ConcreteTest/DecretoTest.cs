using AccountingTestingPlatform.Report;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistenciaTxtPadTest.ConcreteTests
{
    class DecretoTest : ConsistenciaTxtPadTestBase
    {

        public DecretoTest(NpgsqlConnection connection, string remessa, IReport report)
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
                {"Crédito Suplementar", $"select sum(valor_credito_adicional)::decimal from pad.decreto where remessa = {_remessa} and entidade like '{entidade}' and tipo_credito_adicional = 1;" },
                {"Crédito Especial", $"select sum(valor_credito_adicional)::decimal from pad.decreto where remessa = {_remessa} and entidade like '{entidade}' and tipo_credito_adicional = 2;" },
                {"Crédito Extraordinário", $"select sum(valor_credito_adicional)::decimal from pad.decreto where remessa = {_remessa} and entidade like '{entidade}' and tipo_credito_adicional = 3;" },
                {"Redução na mesma entidade", $"select sum(valor_reducao_dotacao)::decimal from pad.decreto where remessa = {_remessa} and entidade like '{entidade}' and origem_recurso = 5;" },
                {"Redução total", $"select sum(valor_reducao_dotacao)::decimal from pad.decreto where remessa = {_remessa} and entidade like '{entidade}';" },
                {"Crédito aberto por superávit", $"select sum(valor_credito_adicional)::decimal from pad.decreto where remessa = {_remessa} and entidade like '{entidade}' and origem_recurso = 1;" },
                {"Crédito aberto por excesso de arrecadação", $"select sum(valor_credito_adicional)::decimal from pad.decreto where remessa = {_remessa} and entidade like '{entidade}' and origem_recurso = 2;" },
                {"Reabertura de créditos", $"select sum(valor_saldo_reaberto)::decimal from pad.decreto where remessa = {_remessa} and entidade like '{entidade}';" },
                
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
            
            return ("DECRETO", result);
        }


    }
}
