using AccountingTestingPlatform.Report;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistenciaTxtPadTest.ConcreteTests
{
    class BalVerTest : ConsistenciaTxtPadTestBase
    {

        public BalVerTest(NpgsqlConnection connection, string remessa, IReport report)
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
                {"Ativo - saldo inicial", $"select sum(saldo_inicial)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '1%';" },
                {"Ativo - débitos", $"select sum(movimento_devedor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '1%';" },
                {"Ativo - créditos", $"select sum(movimento_credor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '1%';" },
                {"Ativo - saldo atual", $"select sum(saldo_atual)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '1%';" },
                {"Passivo - saldo inicial", $"select sum(saldo_inicial)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '2%';" },
                {"Passivo - débitos", $"select sum(movimento_devedor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '2%';" },
                {"Passivo - créditos", $"select sum(movimento_credor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '2%';" },
                {"Passivo - saldo atual", $"select sum(saldo_atual)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '2%';" },
                {"VPD - saldo inicial", $"select sum(saldo_inicial)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '3%';" },
                {"VPD - débitos", $"select sum(movimento_devedor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '3%';" },
                {"VPD - créditos", $"select sum(movimento_credor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '3%';" },
                {"VPD - saldo atual", $"select sum(saldo_atual)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '3%';" },
                {"VPA - saldo inicial", $"select sum(saldo_inicial)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '4%';" },
                {"VPA - débitos", $"select sum(movimento_devedor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '4%';" },
                {"VPA - créditos", $"select sum(movimento_credor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '4%';" },
                {"VPA - saldo atual", $"select sum(saldo_atual)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '4%';" },
                {"CAPO - saldo inicial", $"select sum(saldo_inicial)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '5%';" },
                {"CAPO - débitos", $"select sum(movimento_devedor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '5%';" },
                {"CAPO - créditos", $"select sum(movimento_credor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '5%';" },
                {"CAPO - saldo atual", $"select sum(saldo_atual)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '5%';" },
                {"CEPO - saldo inicial", $"select sum(saldo_inicial)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '6%';" },
                {"CEPO - débitos", $"select sum(movimento_devedor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '6%';" },
                {"CEPO - créditos", $"select sum(movimento_credor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '6%';" },
                {"CEPO - saldo atual", $"select sum(saldo_atual)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '6%';" },
                {"Controles Devedores - saldo inicial", $"select sum(saldo_inicial)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '7%';" },
                {"Controles Devedores - débitos", $"select sum(movimento_devedor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '7%';" },
                {"Controles Devedores - créditos", $"select sum(movimento_credor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '7%';" },
                {"Controles Devedores - saldo atual", $"select sum(saldo_atual)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '7%';" },
                {"Controles Credores - saldo inicial", $"select sum(saldo_inicial)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '8%';" },
                {"Controles Credores - débitos", $"select sum(movimento_devedor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '8%';" },
                {"Controles Credores - créditos", $"select sum(movimento_credor)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '8%';" },
                {"Controles Credores - saldo atual", $"select sum(saldo_atual)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '8%';" },
                
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
            
            return ("BAL_VER", result);
        }


    }
}
