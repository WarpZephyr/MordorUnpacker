using MordorFormats.LTAR;
using MordorUnpacker.Logging;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace MordorUnpacker
{
    internal class Program
    {
        private static bool HadWarnings;
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
                if (Directory.Exists(arg))
                {
                    ProcessDirectory(arg);
                }
                else if (File.Exists(arg))
                {
                    ProcessFileErrorHandler(arg);
                }
                else
                {
                    Warn($"Skipping argument as it isn't a file or folder: \"{arg}\"");
                }
            }

            Log.DirectWriteLine("Finished.");
            if (HadErrors || HadWarnings)
            {
                Pause();
            }

            Log.Dispose();
        }

        private static void ProcessDirectory(string folder)
        {
            foreach (string file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
            {
                ProcessFileErrorHandler(file);
            }
        }

        private static void ProcessFileErrorHandler(string file)
        {
#if !DEBUG
            try
            {
#endif
            ProcessFile(file);
#if !DEBUG
            }
            catch (OodleNotFoundException ex)
            {
                Error($"Failed to process file due to a lack of oodle: \"{file}\"\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Error($"Failed to process file: \"{file}\"\nMessage: \"{ex.Message}\"\nStacktrace: \"{ex.StackTrace}\"");
            }
#endif
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
                LtarUnpacker.Unpack(ltar, outFolder);
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
