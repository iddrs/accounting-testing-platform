using AccountingTestingPlatform.Test;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingTestingPlatform.Profile
{
    internal abstract class ProfileTestBase : IProfileTest
    {
        protected List<ITest> TestList = [];

        protected IProgressMonitor _monitor;

        public void Run(IProgressMonitor monitor)
        {
            _monitor = monitor;
            _monitor.UpdateProgress(0, "Iniciando testes...");
            int total = TestList.Count();
            int percentual = 0;
            int testProcessed = 0;
            foreach (ITest test in TestList)
            {
                percentual = (int)(((double) testProcessed / total) * 100);
                testProcessed++;
                _monitor.UpdateProgress(percentual, $"Executando teste {test.GetType()} ({percentual}%)");
                test.Run();
            }
        }
    }
}
