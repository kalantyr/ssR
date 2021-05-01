using System;
using System.Collections.Generic;
using System.Linq;
using Kalantyr.Rss.Model;

namespace Kalantyr.Rss
{
    public class InvokeLogger: IInvokeLogger, IStatistics
    {
        private readonly ICollection<DateTimeOffset> _times = new List<DateTimeOffset>();
        private readonly DateTimeOffset _startTime = DateTimeOffset.Now;

        public void Log(string methodName = "")
        {
            _times.Add(DateTimeOffset.Now);
        }

        public TimeSpan AverageInvokeInterval
        {
            get
            {
                if (_times.Count < 2)
                    return TimeSpan.Zero;

                var min = _times.Min();
                var max = _times.Max();
                return TimeSpan.FromSeconds((max - min).TotalSeconds / _times.Count);
            }
        }

        public uint InvokeCount => (uint)_times.Count;

        public IReadOnlyList<string> GetStatistics()
        {
            return new[]
            {
                $"Average invoke interval (minutes): {Math.Round(AverageInvokeInterval.TotalMinutes, 1)}",
                $"Invoke count: {InvokeCount}",
                $"Elapsed (minutes): {Math.Round((DateTimeOffset.Now - _startTime).TotalMinutes, 1)}"
            };
        }
    }
}
