using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingTestingPlatform.Test.TypeTests.SaldosInvertidosTest;

internal class SaldosInvertidosTestResult : ITestResult
{
    private IComponent _component;

    private bool _success;

    private string _scope;

    private ITest _test;

    public SaldosInvertidosTestResult(ITest test)
    {
        _test = test;
    }

    public void SetResultComponent(IComponent component)
    {
        _component = component;
    }

    public IComponent GetResultComponent()
    {
        return _component;
    }

    public void SetSuccess(bool success)
    {
        _success = success;
    }

    public bool IsSuccess()
    {
        return _success;
    }

    public void SetTestScope(string testName)
    {
        _scope = testName;
    }

    public string GetTestScope()
    {
        return _scope;
    }

    public ITest GetTest()
    {
        return _test;
    }
}
