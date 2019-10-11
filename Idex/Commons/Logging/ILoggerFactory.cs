using System;
using JetBrains.Annotations;

namespace Idex.Commons.Logging
{
    public interface ILoggerFactory
    {
        [NotNull]
        ILogger Logger { get; }

        [NotNull]
        ILogger Create([NotNull] Type context);

        void CloseAndFlush();
    }
}