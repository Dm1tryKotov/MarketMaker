using System;

namespace MMS.Commons.Logging
{
    internal class NoLogContext : ILogContext, IDisposable
    {
        public static readonly NoLogContext Instance = new NoLogContext();

        public void Dispose()
        {
        }
    }
}