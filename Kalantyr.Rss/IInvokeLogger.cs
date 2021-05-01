using System;
using System.Runtime.CompilerServices;

namespace Kalantyr.Rss
{
    public interface IInvokeLogger
    {
        void Log([CallerMemberName] string methodName = "");

        TimeSpan AverageInvokeInterval { get; }

        /// <summary>
        /// Количество вызовов
        /// </summary>
        uint InvokeCount { get; }
    }
}
