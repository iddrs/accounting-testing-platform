using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingTestingPlatform.Test;

internal interface ITestResult
{
    public void SetResultComponent(IComponent component);

    public IComponent GetResultComponent();

    public void SetSuccess(bool success);

    public bool IsSuccess();

    public void SetTestScope(string scope);

    public string GetTestScope();

    public ITest GetTest();
}
