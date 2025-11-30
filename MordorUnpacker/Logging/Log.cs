using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace MordorUnpacker.Logging
{
    /// <summary>
    /// A log interface with global access.
    /// </summary>
    internal static class Log
    {
        private const string FileName = $"{AppInfo.AppName}.log";
        private static readonly string FolderPath = AppInfo.AppDirectory;
        private static readonly string DataPath = Path.Combine(FolderPath, FileName);
        private static readonly LogProxy Proxy;

        static Log()
        {
            var logger = new TimedLogger(5, 300, 3, true);

            try
            {
                Directory.CreateDirectory(FolderPath);
                StreamWriter fileLog = new StreamWriter(DataPath, true);
                Proxy = new LogProxy(logger, fileLog);
            }
            catch (IOException)
            {
                Proxy = new LogProxy(logger, null);
                Proxy.DirectWriteLine($"Failed to open file log, it may already be open: \"{DataPath}\"");
            }
            catch (Exception ex)
            {
                Proxy = new LogProxy(logger, null);
                Proxy.DirectWriteLine($"Failed opening file log from path \"{DataPath}\": {ex}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(string value)
            => Proxy.Write(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLine(string value)
            => Proxy.WriteLine(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLine()
            => Proxy.WriteLine();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DirectWrite(string value)
            => Proxy.DirectWrite(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DirectWriteLine(string value)
            => Proxy.DirectWriteLine(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DirectWriteLine()
            => Proxy.DirectWriteLine();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Flush()
            => Proxy.Flush();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dispose()
            => Proxy.Dispose();
    }
}
