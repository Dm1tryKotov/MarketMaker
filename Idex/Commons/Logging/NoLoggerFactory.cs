using System;

namespace MMS.Commons.Logging
{
    public class NoLoggerFactory : ILoggerFactory
    {
        public ILogger Logger
        {
            get
            {
                return (ILogger) NoLogger.Instance;
            }
        }

        public ILogger Create(Type context)
        {
            return (ILogger) NoLogger.Instance;
        }

        public void CloseAndFlush()
        {
        }
    }
}