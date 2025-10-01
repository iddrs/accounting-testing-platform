using AccountingTestingPlatform.Report;
using Npgsql;
using System.Data;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistencyTest.ConcreteTests
{
    class EstoqueSaidasTest : ConsistencyTestBase
    {

        public EstoqueSaidasTest(NpgsqlConnection connection, string remessa, IReport report)
        {
            _connection = connection;
            _remessa = remessa;
            _report = report;
            _entidades = new()
                        {
                            {"pm", "Prefeitura" },
                            {"cm", "Câmara" },
                            {"fpsm", "FPSM" },
                        };
        }

        private static string GetPreviousRemessa(string remessa)
        {
            if (remessa.Length != 6)
            {
                throw new ArgumentException($"O valor '{remessa}' informado não está no formado adequado (AAAAMM ex.: 202509)!");

            }
            int year = int.Parse(remessa.Substring(0, 4));
            int month = int.Parse(remessa.Substring(4, 2));

            month--;
            if (month == 0)
            {
                month = 12;
                year--;
            }

            return $"{year:D4}{month:D2}";
        }

        protected override (string, ITestResult) ExecuteTest(string entidade)
        {
            (string, string) datas = GetLimitesRemessa(_remessa);
            DateTime date = new DateTime(int.Parse(_remessa.Substring(0, 4)), int.Parse(_remessa.Substring(4, 2)), 1);
            string[] sql1 = [
                $"select sum(valor)::decimal from auxiliar.dados_manuais where remessa = {_remessa} and entidade like '{entidade}' and item like 'Estoque - saídas';",
            ];
            string[] sql2 = [
                $"select sum(valor_lancamento)::decimal from pad.tce_4111 where remessa = {_remessa} and entidade like '{entidade}' and data_lancamento between '{date.ToString("yyyy-MM-dd")}' and '{datas.Item2}' and conta_contabil like '1156199%' and tipo_lancamento like 'C';",
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

            
            return ("Estoques: saídas", result);
        }


    }
}
