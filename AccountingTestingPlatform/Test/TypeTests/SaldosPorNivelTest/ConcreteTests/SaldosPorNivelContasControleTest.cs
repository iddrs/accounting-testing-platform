using AccountingTestingPlatform.Report;
using AccountingTestingPlatform.Test.TypeTests.ConsistencyTest;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.SaldosPorNivelTest.ConcreteTests
{
    class SaldosPorNivelContasControleTest : SaldosPorNivelTestBase
    {
        private List<string> _cc = [
            "111101", "111102", "111103", "111104", "111201", "111202", "111203", "111204", "111301", "111302", "111303", "111303", "111304", "111401", "111402", "111403", "111404", "111501", "111502", "111503", "111504", "112101", "112102", "112199", "113101", "113102", "113103", "113104", "113105", "113108", "113112", "113113", "113199", "1141", "1142", "1143", "1144", "1145", "119104", "119105", "121101", "121102", "121103", "121104", "121201", "121202", "121203", "121204", "121301", "121302", "121303", "121304", "121401", "121402", "121403", "121404", "121501", "121502", "121503", "121504", "122101", "122102", "122199", "123101", "123102", "123103", "123104", "123105", "123106", "123107", "123108", "123109", "123110", "123111", "123112", "123113", "123199", "1241", "1242", "1243", "1244", "1245", "1291", "1292", "1293", "1294", "1295", "211", "212", "213", "219", "221101", "221102", "221199", "221201", "221299", "2219", "229", "23", "2411", "2413", "311101", "311102", "311103", "311199", "311201", "321101", "321102", "3219", "4111", "4112", "4113", "4114", "4115", "4119", "4211", "4212", "4213", "4214", "531", "532", "533", "534", "535", "536", "537", "611", "612", "613", "619", "62", "631", "632", "633", "641", "642", "646", "647", "9111", "9112", "9113", "9114", "9115", "9119", "9121", "9122", "9124", "9125", "9129", "929", "9511", "9512", "990001", "990002"];
        public SaldosPorNivelContasControleTest(NpgsqlConnection connection, string remessa, IReport report)
        {
            _connection = connection;
            _remessa = remessa;
            _report = report;
        }
        
        protected override (string, ITestResult) ExecuteTest(string entidade)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CD", typeof(string));
            dt.Columns.Add("Valor CD", typeof(decimal));
            dt.Columns.Add("CC", typeof(string));
            dt.Columns.Add("Valor CC", typeof(decimal));
            dt.Columns.Add("Diferença", typeof(decimal));

            bool fails = false;

            foreach(string cc in _cc)
            {
                string cc1 = $"7{cc}";
                string cc2 = $"8{cc}";
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

            return ("Igualdade de saldos por nível das contas de controle", result);
        }


    }
}
