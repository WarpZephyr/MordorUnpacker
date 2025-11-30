using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MordorUnpacker
{
    /// <summary>
    /// Information about this program gathered into an easy to access and static place.
    /// </summary>
    internal static class AppInfo
    {
        /// <summary>
        /// A representation of the platform the program is on.
        /// </summary>
        public static readonly string Platform;

        /// <summary>
        /// The current version of the program.
        /// </summary>
        public static readonly string Version;

        /// <summary>
        /// Whether or not the program is built as debug.
        /// </summary>
#if DEBUG
        public const bool IsDebug = true;
#else
        public const bool IsDebug = false;
#endif

        /// <summary>
        /// The name of the program.
        /// </summary>
        public const string AppName = "MordorUnpacker";

        /// <summary>
        /// The file path of the program.
        /// </summary>
        public static readonly string AppFilePath;

        /// <summary>
        /// The folder path of the program.
        /// </summary>
        public static readonly string AppDirectory;

        /// <summary>
        /// Initializes <see cref="AppInfo"/>.
        /// </summary>
        static AppInfo()
        {
            Platform = GetPlatform();
            Version = GetVersion();
            AppFilePath = Environment.ProcessPath ?? "Unknown";
            AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Find the app version or default.
        /// </summary>
        /// <returns>The app version.</returns>
        private static string GetVersion()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            var version = executingAssembly.GetName().Version;
            if (version != null)
            {
                return version.ToString();
            }
            else
            {
                return "0.0.0.0";
            }
        }

        /// <summary>
        /// Find the app platform.
        /// </summary>
        /// <returns>The app platform.</returns>
        private static string GetPlatform()
        {
            string platform;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                platform = "Linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                platform = "FreeBSD";
            }
            else
            {
                return Environment.OSVersion.ToString();
            }

            var osVersion = Environment.OSVersion;
            string servicePack = osVersion.ServicePack;
            return string.IsNullOrEmpty(servicePack) ?
               $"{platform} {osVersion.Version}" :
               $"{platform} {osVersion.Version.ToString(3)} {servicePack}";
        }
    }
}
