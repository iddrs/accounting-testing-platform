using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AccountingTestingPlatform
{
    internal interface IProgressMonitor
    {
        event PropertyChangedEventHandler PropertyChanged;

        public void UpdateProgress(int current, string? message = null);
    }
}
