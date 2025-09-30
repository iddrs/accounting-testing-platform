using AccountingTestingPlatform.Report;
using AccountingTestingPlatform.Test.TypeTests.ConsistencyTest;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.SaldosPorNivelTest.ConcreteTests
{
    class SaldosPorNivelContasOrcamentariasTest : SaldosPorNivelTestBase
    {
        private List<string> _cc = [
            "111", "112", "121", "122", "21", "221", "222101", "222102", "222109", "222199", "222201", "222202", "222209", "222299", "222901", "222902", "222999", "229", "31", "32"];
        public SaldosPorNivelContasOrcamentariasTest(NpgsqlConnection connection, string remessa, IReport report)
        {
            _connection = connection;
            _remessa = remessa;
            _report = report;
        }
        
        protected override (string, ITestResult) ExecuteTest(string entidade)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CAPO", typeof(string));
            dt.Columns.Add("Valor CAPO", typeof(decimal));
            dt.Columns.Add("CEPO", typeof(string));
            dt.Columns.Add("Valor CEPO", typeof(decimal));
            dt.Columns.Add("Diferença", typeof(decimal));

            bool fails = false;

            foreach(string cc in _cc)
            {
                string cc1 = $"5{cc}";
                string cc2 = $"6{cc}";
                string sql1 = $"select sum(saldo_atual)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '{cc1}%';";
                string sql2 = $"select sum(saldo_atual)::decimal from pad.bal_ver where remessa = {_remessa} and entidade like '{entidade}' and escrituracao like 'S' and conta_contabil like '{cc2}%';";

                using NpgsqlCommand cmd1 = new(sql1, _connection);
                decimal val1 = ExecuteSql(cmd1);
                
                using NpgsqlCommand cmd2 = new(sql2, _connection);
                decimal val2 = ExecuteSql(cmd2);

                decimal diferenca = val1 - val2;

                dt.Rows.Add(cc1, val1, cc2, val2, diferenca);

                if(diferenca != 0m) fails = true;


            }

            

            
            

            ITestResult result = new SaldosPorNivelTestResult(this);
            result.SetTestScope(_entidades[entidade]);
            result.SetSuccess(!fails);
            result.SetResultComponent(new SaldosPorNivelTestResultTableComponent(dt));

            return ("Igualdade de saldos por nível das contas orçamentárias", result);
        }


    }
}
