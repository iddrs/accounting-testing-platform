using AccountingTestingPlatform.Test;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AccountingTestingPlatform.Report
{
    interface IReport
    {
        public void AddTest(string testName, List<ITestResult> testResults);

        public void Save();

        public void Open();
    }
}
