using AccountingTestingPlatform.Report;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistenciaTxtPadTest.ConcreteTests
{
    class RestosPagarTest : ConsistenciaTxtPadTestBase
    {

        public RestosPagarTest(NpgsqlConnection connection, string remessa, IReport report)
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
                {"RP Não Processados - inscritos em exercícios anteriores", $"select sum(saldo_nao_processado_inscritos_exercicios_anteriores)::decimal from pad.restos_pagar where remessa = {_remessa} and entidade like '{entidade}';" },
                {"RP Não Processados - inscritos no exercício", $"select sum(nao_processado_inscritos_ultimo_exercicio)::decimal from pad.restos_pagar where remessa = {_remessa} and entidade like '{entidade}';" },
                {"RP Não Processados - liquidados", $"select sum(rp_liquidado)::decimal from pad.restos_pagar where remessa = {_remessa} and entidade like '{entidade}';" },
                {"RP Não Processados - pagos", $"select sum(nao_processado_pago)::decimal from pad.restos_pagar where remessa = {_remessa} and entidade like '{entidade}';" },
                {"RP Não Processados - cancelados", $"select sum(nao_processado_cancelado)::decimal from pad.restos_pagar where remessa = {_remessa} and entidade like '{entidade}';" },
                {"RP Não Processados - saldo final", $"select sum(saldo_final_nao_processado)::decimal from pad.restos_pagar where remessa = {_remessa} and entidade like '{entidade}';" },
                {"RP Processados - inscritos em exercícios anteriores", $"select sum(saldo_processado_inscritos_exercicios_anteriores)::decimal from pad.restos_pagar where remessa = {_remessa} and entidade like '{entidade}';" },
                {"RP Processados - no exercício", $"select sum(processado_inscritos_ultimo_exercicio)::decimal from pad.restos_pagar where remessa = {_remessa} and entidade like '{entidade}';" },
                {"RP Processados - pago", $"select sum(processado_pago)::decimal from pad.restos_pagar where remessa = {_remessa} and entidade like '{entidade}';" },
                {"RP Processados - cancelado", $"select sum(processado_cancelado)::decimal from pad.restos_pagar where remessa = {_remessa} and entidade like '{entidade}';" },
                {"RP Processados - saldo final", $"select sum(saldo_final_processado)::decimal from pad.restos_pagar where remessa = {_remessa} and entidade like '{entidade}';" },
                
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
            
            return ("RESTOS_PAGAR", result);
        }


    }
}
