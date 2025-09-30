using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingTestingPlatform.Test.TypeTests.ConsistencyTest
{
    internal abstract class ConsistencyTestBase : TestBase
    {
        protected Dictionary<string, string> _entidades = new()
        {
            {"pm", "Prefeitura" },
            {"cm", "Câmara" },
            {"fpsm", "FPSM" },
        };

        public override void Run()
        {
            List<ITestResult> results = new();
            string testName = "";
            foreach (KeyValuePair<string, string> item in _entidades)
            {
                (string, ITestResult) result = ExecuteTest(item.Key);
                results.Add(result.Item2);
                testName = result.Item1;
            }
            _report.AddTest(testName, results);
        }

        protected abstract (string, ITestResult) ExecuteTest(string entidade);

        protected decimal ExecuteSql(NpgsqlCommand cmd)
        {
            object result = cmd.ExecuteScalar();
            decimal valor = 0m;
            if (result != null && !string.IsNullOrEmpty(result.ToString()) && !string.IsNullOrWhiteSpace(result.ToString()))
            {
                valor = decimal.Parse(result.ToString());
            }
            return valor;
        }
    }
}
