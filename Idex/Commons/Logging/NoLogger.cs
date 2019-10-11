using System;

namespace MMS.Commons.Logging
{
    public class NoLogger : ILogger
  {
    public static readonly NoLogger Instance = new NoLogger();

    public bool IsVerboseEnabled
    {
      get
      {
        return false;
      }
    }

    public bool IsDebugEnabled
    {
      get
      {
        return false;
      }
    }

    public bool IsWarnEnabled
    {
      get
      {
        return false;
      }
    }

    public bool IsInfoEnabled
    {
      get
      {
        return false;
      }
    }

    public bool IsErrorEnabled
    {
      get
      {
        return false;
      }
    }

    public void Write(LogLevel level, string messageTemplate)
    {
    }

    public void Write<T>(LogLevel level, string messageTemplate, T propertyValue)
    {
    }

    public void Write<T0, T1>(
      LogLevel level,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1)
    {
    }

    public void Write<T0, T1, T2>(
      LogLevel level,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Write(LogLevel level, string messageTemplate, params object[] propertyValues)
    {
    }

    public void Write(LogLevel level, Exception exception, string messageTemplate)
    {
    }

    public void Write<T>(
      LogLevel level,
      Exception exception,
      string messageTemplate,
      T propertyValue)
    {
    }

    public void Write<T0, T1>(
      LogLevel level,
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1)
    {
    }

    public void Write<T0, T1, T2>(
      LogLevel level,
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Write(
      LogLevel level,
      Exception exception,
      string messageTemplate,
      params object[] propertyValues)
    {
    }

    public bool IsEnabled(LogLevel level)
    {
      return false;
    }

    public void Verbose(string messageTemplate)
    {
    }

    public void Verbose<T>(string messageTemplate, T propertyValue)
    {
    }

    public void Verbose<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
    }

    public void Verbose<T0, T1, T2>(
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Verbose(string messageTemplate, params object[] propertyValues)
    {
    }

    public void Verbose(Exception exception, string messageTemplate)
    {
    }

    public void Verbose<T>(Exception exception, string messageTemplate, T propertyValue)
    {
    }

    public void Verbose<T0, T1>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1)
    {
    }

    public void Verbose<T0, T1, T2>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Verbose(
      Exception exception,
      string messageTemplate,
      params object[] propertyValues)
    {
    }

    public void Debug(string messageTemplate)
    {
    }

    public void Debug<T>(string messageTemplate, T propertyValue)
    {
    }

    public void Debug<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
    }

    public void Debug<T0, T1, T2>(
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Debug<T0, T1, T2>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Debug(Exception exception, string message, params object[] values)
    {
    }

    public void Info(string messageTemplate)
    {
    }

    public void Info<T>(string messageTemplate, T propertyValue)
    {
    }

    public void Info<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
    }

    public void Info<T0, T1, T2>(
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Info<T0, T1, T2>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Info(Exception exception, string message, params object[] values)
    {
    }

    public void Warn(string messageTemplate)
    {
    }

    public void Warn<T>(string messageTemplate, T propertyValue)
    {
    }

    public void Warn<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
    }

    public void Warn<T0, T1, T2>(
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Warn<T0, T1, T2>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Warn(Exception exception, string message, params object[] values)
    {
    }

    public void Error(string messageTemplate)
    {
    }

    public void Error<T>(string messageTemplate, T propertyValue)
    {
    }

    public void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
    }

    public void Error<T0, T1, T2>(
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Error<T0, T1, T2>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Error(Exception exception, string message, params object[] values)
    {
    }

    public void Fatal(string messageTemplate)
    {
    }

    public void Fatal<T>(string messageTemplate, T propertyValue)
    {
    }

    public void Fatal<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
    }

    public void Fatal<T0, T1, T2>(
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Fatal(string messageTemplate, params object[] propertyValues)
    {
    }

    public void Fatal(Exception exception, string messageTemplate)
    {
    }

    public void Fatal<T>(Exception exception, string messageTemplate, T propertyValue)
    {
    }

    public void Fatal<T0, T1>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1)
    {
    }

    public void Fatal<T0, T1, T2>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2)
    {
    }

    public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
    {
    }

    public void Debug(string message, params object[] values)
    {
    }

    public void Debug(Exception exception, string messageTemplate)
    {
    }

    public void Debug<T>(Exception exception, string messageTemplate, T propertyValue)
    {
    }

    public void Debug<T0, T1>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1)
    {
    }

    public void Info(string message, params object[] values)
    {
    }

    public void Info(Exception exception, string messageTemplate)
    {
    }

    public void Info<T>(Exception exception, string messageTemplate, T propertyValue)
    {
    }

    public void Info<T0, T1>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1)
    {
    }

    public void Warn(string message, params object[] values)
    {
    }

    public void Warn(Exception exception, string messageTemplate)
    {
    }

    public void Warn<T>(Exception exception, string messageTemplate, T propertyValue)
    {
    }

    public void Warn<T0, T1>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1)
    {
    }

    public void Error(string message, params object[] values)
    {
    }

    public void Error(Exception exception, string messageTemplate)
    {
    }

    public void Error<T>(Exception exception, string messageTemplate, T propertyValue)
    {
    }

    public void Error<T0, T1>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1)
    {
    }

    public ILogContext PushContext(string name, object context)
    {
      return (ILogContext) NoLogContext.Instance;
    }
  }
}