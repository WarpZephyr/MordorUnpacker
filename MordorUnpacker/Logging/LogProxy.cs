using System;
using System.IO;

namespace MordorUnpacker.Logging
{
    /// <summary>
    /// Reports to multiple logging channels;<br/>
    /// This currently includes:<br/>
    /// 1. Any app logs.<br/>
    /// 2. An app log file.
    /// </summary>
    internal class LogProxy : IDisposable
    {
        #region Members

        private readonly TimedLogger Log;
        private readonly StreamWriter? FileLog;
        private readonly object LogLock;
        private DateTime StartTime;
        private bool disposedValue;

        #endregion

        #region Constructors

        public LogProxy(TimedLogger log, StreamWriter? fileLog)
        {
            LogLock = new object();
            Log = log;
            FileLog = fileLog;
            StartTime = DateTime.Now;

            PrintTimeInfo();
            PrintAppInfo();
        }

        public LogProxy(TimedLogger log) : this(log, null) { }

        #endregion

        #region Info Methods

        private void PrintTimeInfo()
        {
            WriteLine($"[Log Started: {StartTime:MM-dd-yyyy-hh:mm:ss}]");
        }

        private void PrintAppInfo()
        {
            WriteLine($"[App Platform: {AppInfo.Platform}]");
            WriteLine($"[App Version: {AppInfo.Version}]");
#if DEBUG
            WriteLine("[App Build: Debug]");
#else
            WriteLine("[App Build: Release]");
#endif
            WriteLine($"[App Name: {AppInfo.AppName}]");
            WriteLine($"[App File Path: \"{AppInfo.AppFilePath}\"]");
            WriteLine($"[App Directory: \"{AppInfo.AppDirectory}\"]");
        }

        #endregion

        #region Logging Methods

        public void Write(string value)
        {
            lock (LogLock)
            {
                Log.Write(value);
                FileLog?.Write(value);
            }
        }

        public void WriteLine(string value)
        {
            lock (LogLock)
            {
                Log.WriteLine(value);
                FileLog?.WriteLine(value);
            }
        }

        public void WriteLine()
        {
            lock (LogLock)
            {
                Log.WriteLine();
                FileLog?.WriteLine();
            }
        }

        public void DirectWrite(string value)
        {
            lock (LogLock)
            {
                Log.DirectWrite(value);
                FileLog?.Write(value);
            }
        }

        public void DirectWriteLine(string value)
        {
            lock (LogLock)
            {
                Log.DirectWriteLine(value);
                FileLog?.WriteLine(value);
            }
        }

        public void DirectWriteLine()
        {
            lock (LogLock)
            {
                Log.DirectWriteLine();
                FileLog?.WriteLine();
            }
        }

        public void Flush()
        {
            lock (LogLock)
            {
                Log.Flush();
                FileLog?.Flush();
            }
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    var endTime = DateTime.Now;
                    WriteLine($"[Run Time: {endTime - StartTime}]");
                    WriteLine($"[Log Ended: {endTime:MM-dd-yyyy-hh:mm:ss}]");
                    WriteLine(); // Spacer between runs

                    Flush();
                    Log.Dispose();
                    FileLog?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
