using System;

namespace GameBase
{
    public class SummaryProfiler
    {
        public static SummaryProfiler Current { get; set; }

        static SummaryProfiler()
        {
            Current = new SummaryProfiler();
        }

        public IDisposable Step(string description)
        {
            return null;
        }

        public class SummaryProfilerTimer : IDisposable
        {
            public void Dispose()
            {
                
            }
        }
    }
}