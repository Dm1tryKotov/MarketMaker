using System;
using JetBrains.Annotations;

namespace Idex.Commons.Logging
{
    public static class LoggerFactory
    {
        private static ILoggerFactory _instance;

        public static ILoggerFactory Instance
        {
            get
            {
                return LoggerFactory._instance ?? (LoggerFactory._instance = (ILoggerFactory) new NoLoggerFactory());
            }
            set
            {
                ILoggerFactory loggerFactory = value;
                if (loggerFactory == null)
                    throw new ArgumentNullException(nameof (value));
                LoggerFactory._instance = loggerFactory;
            }
        }

        [NotNull]
        public static ILogger Create([NotNull] Type type)
        {
            if (type == (Type) null)
                throw new ArgumentNullException(nameof (type));
            return LoggerFactory.Instance.Create(type);
        }

        [NotNull]
        public static ILogger Create<T>()
        {
            return LoggerFactory.Create(typeof (T));
        }
    }
}