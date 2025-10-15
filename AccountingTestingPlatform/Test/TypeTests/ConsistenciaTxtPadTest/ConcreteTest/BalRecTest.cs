using AccountingTestingPlatform.Report;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistenciaTxtPadTest.ConcreteTests
{
    class BalRecTest : ConsistenciaTxtPadTestBase
    {

        public BalRecTest(NpgsqlConnection connection, string remessa, IReport report)
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
                {"Previsão inicial da receita (líquida)", $"select sum(receita_orcada)::decimal from pad.bal_rec where remessa = {_remessa} and entidade like '{entidade}' and tipo_nivel_receita like 'A';" },
                {"Previsão atualizada da receita (líquida)", $"select sum(previsao_atualizada)::decimal from pad.bal_rec where remessa = {_remessa} and entidade like '{entidade}' and tipo_nivel_receita like 'A';" },
                {"Receita arrecadada (líquida)", $"select sum(receita_realizada)::decimal from pad.bal_rec where remessa = {_remessa} and entidade like '{entidade}' and tipo_nivel_receita like 'A';" },
                {"Previsão inicial das deduções da receita", $"select sum(receita_orcada *-1)::decimal from pad.bal_rec where remessa = {_remessa} and entidade like '{entidade}' and tipo_nivel_receita like 'A' and caracteristica_peculiar_receita > 0;" },
                {"Previsão atualizada das deduções da receita", $"select sum(previsao_atualizada *-1)::decimal from pad.bal_rec where remessa = {_remessa} and entidade like '{entidade}' and tipo_nivel_receita like 'A' and caracteristica_peculiar_receita > 0;" },
                {"Deduções da receita realizadas", $"select sum(receita_realizada *-1)::decimal from pad.bal_rec where remessa = {_remessa} and entidade like '{entidade}' and tipo_nivel_receita like 'A' and caracteristica_peculiar_receita > 0;" },
            };

            ITestResult result = new ConsistenciaTxtPadTestResult(this);
            result.SetSuccess(true);

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
                if (diferenca != 0m)
                {
                    result.SetSuccess(false);
                }
            }
            
            result.SetTestScope(_entidades[entidade]);
            result.SetResultComponent(new ConsistenciaTxtPadTestResultTableComponent(dt));
            
            return ("BAL_REC", result);
        }


    }
}
