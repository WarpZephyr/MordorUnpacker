using MordorFormats.LTAR;
using MordorUnpacker.Data;
using MordorUnpacker.Data.Configs;
using MordorUnpacker.Logging;
using OodleCoreSharp.Exceptions;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace MordorUnpacker
{
    internal class Program
    {
        /// <summary>
        /// Whether or not warnings are present.
        /// </summary>
        private static bool HadWarnings;

        /// <summary>
        /// Whether or not errors are present.
        /// </summary>
        private static bool HadErrors;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Log.DirectWriteLine("This tool has no GUI; Please drag and drop files or folders into the tool exe.");
                Pause();
                return;
            }

            foreach (string arg in args)
            {
#if DEBUG
                ProcessArg(arg);
#else
                ProcessArgErrorHandler(arg);
#endif
            }

            Log.DirectWriteLine("Finished.");
            if (HadErrors || HadWarnings)
            {
                Pause();
            }

            Log.Dispose();
        }

        /// <summary>
        /// An error wrapper for release versions so the tool can keep on processing other arguments when errors occur.
        /// </summary>
        /// <param name="arg">The argument to process.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ProcessArgErrorHandler(string arg)
        {
            try
            {
                ProcessArg(arg);
            }
            catch (OodleNotFoundException ex)
            {
                Error($"Failed to process argument due to a lack of oodle: \"{arg}\"\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Error($"Failed to process argument: \"{arg}\"\nMessage: \"{ex.Message}\"\nStacktrace: \"{ex.StackTrace}\"");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ProcessArg(string arg)
        {
            if (Directory.Exists(arg))
            {
                ProcessFolder(arg);
            }
            else if (File.Exists(arg))
            {
                ProcessFile(arg);
            }
            else
            {
                Warn($"Skipping argument as it isn't a file or folder: \"{arg}\"");
            }
        }

        private static void ProcessFolder(string inFolder)
        {
            string inName = new DirectoryInfo(inFolder).Name;
            string? outFolder = Path.GetDirectoryName(inFolder);
            if (string.IsNullOrEmpty(outFolder))
            {
                Error($"Could not get parent folder of: \"{inFolder}\"");
                return;
            }

            if (File.Exists(Path.Combine(inFolder, LtarConfig.ConfigFileName)))
            {
                Log.DirectWriteLine($"Repacking LTAR: {inName}...");
                LtarUnpacker.Repack(inFolder, outFolder);
            }
            else
            {
                Warn($"Couldn't detect what kind of file to repack for folder: \"{inFolder}\"");
            }
        }

        private static void ProcessFile(string file)
        {
            string? folder = Path.GetDirectoryName(file);
            if (folder == null)
            {
                Error($"Could not get folder name for file: \"{file}\"");
                return;
            }

            // Create output folder with same name as file but with dashes instead of dots
            string? fileName = Path.GetFileName(file);
            if (string.IsNullOrEmpty(fileName))
            {
                throw new Exception($"Could not get name of file: \"{file}\"");
            }

            string outFolderName;
            if (fileName.Contains('.'))
                outFolderName = fileName.Replace('.', '-');
            else
                outFolderName = fileName + "-unpack"; // Add if file has no extensions to make it's name different with


            string outFolder = Path.Combine(folder, outFolderName);
            if (File.Exists(outFolder))
            {
                throw new Exception($"Expected output folder path was a file for: \"{file}\"");
            }

            if (LtarReader.IsRead(file, out LtarReader? ltar))
            {
                Log.DirectWriteLine($"Unpacking LTAR at \"{file}\"...");
                LtarUnpacker.Unpack(ltar, fileName, outFolder);
            }
            else
            {
                Warn($"Skipping file because unknown file type: \"{file}\"");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Error(string message)
        {
            Log.DirectWriteLine($"Error: {message}");
            HadErrors = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Warn(string message)
        {
            Log.DirectWriteLine($"Warning: {message}");
            HadWarnings = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Pause()
            => Console.ReadKey(true);
    }
}
