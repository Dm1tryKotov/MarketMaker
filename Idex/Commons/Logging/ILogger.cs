using System;
using JetBrains.Annotations;

namespace MMS.Commons.Logging
{
    public interface ILogger
  {
    bool IsVerboseEnabled { get; }

    bool IsDebugEnabled { get; }

    bool IsInfoEnabled { get; }

    bool IsWarnEnabled { get; }

    bool IsErrorEnabled { get; }

    void Write(LogLevel level, string messageTemplate);

    void Write<T>(LogLevel level, string messageTemplate, T propertyValue);

    void Write<T0, T1>(
      LogLevel level,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1);

    void Write<T0, T1, T2>(
      LogLevel level,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Write(LogLevel level, string messageTemplate, params object[] propertyValues);

    void Write(LogLevel level, Exception exception, string messageTemplate);

    void Write<T>(LogLevel level, Exception exception, string messageTemplate, T propertyValue);

    void Write<T0, T1>(
      LogLevel level,
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1);

    void Write<T0, T1, T2>(
      LogLevel level,
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Write(
      LogLevel level,
      Exception exception,
      string messageTemplate,
      params object[] propertyValues);

    bool IsEnabled(LogLevel level);

    void Verbose(string messageTemplate);

    void Verbose<T>(string messageTemplate, T propertyValue);

    void Verbose<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1);

    void Verbose<T0, T1, T2>(
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Verbose(string messageTemplate, params object[] propertyValues);

    void Verbose(Exception exception, string messageTemplate);

    void Verbose<T>(Exception exception, string messageTemplate, T propertyValue);

    void Verbose<T0, T1>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1);

    void Verbose<T0, T1, T2>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Verbose(Exception exception, string messageTemplate, params object[] propertyValues);

    void Debug(string messageTemplate);

    void Debug<T>(string messageTemplate, T propertyValue);

    void Debug<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1);

    void Debug<T0, T1, T2>(
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Debug(string messageTemplate, params object[] propertyValues);

    void Debug(Exception exception, string messageTemplate);

    void Debug<T>(Exception exception, string messageTemplate, T propertyValue);

    void Debug<T0, T1>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1);

    void Debug<T0, T1, T2>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Debug(Exception exception, string messageTemplate, params object[] propertyValues);

    void Info(string messageTemplate);

    void Info<T>(string messageTemplate, T propertyValue);

    void Info<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1);

    void Info<T0, T1, T2>(
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Info(string messageTemplate, params object[] propertyValues);

    void Info(Exception exception, string messageTemplate);

    void Info<T>(Exception exception, string messageTemplate, T propertyValue);

    void Info<T0, T1>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1);

    void Info<T0, T1, T2>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Info(Exception exception, string messageTemplate, params object[] propertyValues);

    void Warn(string messageTemplate);

    void Warn<T>(string messageTemplate, T propertyValue);

    void Warn<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1);

    void Warn<T0, T1, T2>(
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Warn(string messageTemplate, params object[] propertyValues);

    void Warn(Exception exception, string messageTemplate);

    void Warn<T>(Exception exception, string messageTemplate, T propertyValue);

    void Warn<T0, T1>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1);

    void Warn<T0, T1, T2>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Warn(Exception exception, string messageTemplate, params object[] propertyValues);

    void Error(string messageTemplate);

    void Error<T>(string messageTemplate, T propertyValue);

    void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1);

    void Error<T0, T1, T2>(
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Error(string messageTemplate, params object[] propertyValues);

    void Error(Exception exception, string messageTemplate);

    void Error<T>(Exception exception, string messageTemplate, T propertyValue);

    void Error<T0, T1>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1);

    void Error<T0, T1, T2>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Error(Exception exception, string messageTemplate, params object[] propertyValues);

    void Fatal(string messageTemplate);

    void Fatal<T>(string messageTemplate, T propertyValue);

    void Fatal<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1);

    void Fatal<T0, T1, T2>(
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Fatal(string messageTemplate, params object[] propertyValues);

    void Fatal(Exception exception, string messageTemplate);

    void Fatal<T>(Exception exception, string messageTemplate, T propertyValue);

    void Fatal<T0, T1>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1);

    void Fatal<T0, T1, T2>(
      Exception exception,
      string messageTemplate,
      T0 propertyValue0,
      T1 propertyValue1,
      T2 propertyValue2);

    void Fatal(Exception exception, string messageTemplate, params object[] propertyValues);

    [NotNull]
    ILogContext PushContext([NotNull] string name, [NotNull] object context);
  }
}