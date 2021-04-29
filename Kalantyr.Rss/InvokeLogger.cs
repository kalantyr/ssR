using System;
using System.Collections.Generic;
using System.Linq;

namespace Kalantyr.Rss
{
    public class InvokeLogger: IInvokeLogger
    {
        private readonly ICollection<DateTimeOffset> _times = new List<DateTimeOffset>();

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
    }
}
