using MordorFormats.LTAR;
using MordorUnpacker.Data.Configs;
using MordorUnpacker.Logging;
using MordorUnpacker.Utilities;
using System.IO;

namespace MordorUnpacker.Data
{
    /// <summary>
    /// An unpacker and repacker for lithtech archives.
    /// </summary>
    internal static class LtarUnpacker
    {
        /// <summary>
        /// Unpacks a lithtech archive.
        /// </summary>
        /// <param name="ltar">The archive reader.</param>
        /// <param name="fileName">The file name of the archive.</param>
        /// <param name="outFolder">The folder to unpack to.</param>
        public static void Unpack(LtarReader ltar, string fileName, string outFolder)
        {
            var cfg = new LtarConfig
            {
                Name = fileName,
                BigEndian = ltar.BigEndian,
                Version = ltar.Version,
                Unk14 = ltar.Unk14
            };

            int fileIndexBase = 0;
            for (int folderIndex = 0; folderIndex < ltar.Folders.Count; folderIndex++)
            {
                // Create folder path
                var lFolder = ltar.Folders[folderIndex];
                string outLFolder;
                if (lFolder.Name == string.Empty)
                    outLFolder = outFolder; // handle root folder
                else
                    outLFolder = Path.Combine(outFolder, lFolder.Name);

                // Create folder
                Directory.CreateDirectory(outLFolder);

                // Unpack all files in each folder
                for (int folderFileIndex = 0; folderFileIndex < lFolder.FileCount; folderFileIndex++)
                {
                    int fileIndex = fileIndexBase + folderFileIndex;
                    var lFile = ltar.Files[fileIndex];
                    string outLPath = Path.Combine(outLFolder, lFile.Name);
                    string localPath = Path.Combine(lFolder.Name, lFile.Name);
                    Log.WriteLine(localPath);

                    cfg.Files.Add(localPath);
                    using var fs = File.OpenWrite(outLPath);
                    ltar.ReadFile(lFile, fs);
                }

                // Increment the total file index
                fileIndexBase += lFolder.FileCount;
            }

            cfg.Save(outFolder);
        }

        /// <summary>
        /// Repacks a lithtech archive.
        /// </summary>
        /// <param name="inFolder">The input folder containing the files to repack.</param>
        /// <param name="outFolder">The output folder containing the lithtech archive.</param>
        /// <exception cref="FileNotFoundException">Could not find one of the files to repack.</exception>
        public static void Repack(string inFolder, string outFolder)
        {
            var cfg = LtarConfig.Load(inFolder);
            var ltar = new LtarWriter
            {
                BigEndian = cfg.BigEndian,
                UseMaxZlibCompressionLevel = cfg.UseMaxZlibCompressionLevel,
                OodleCompressor = cfg.OodleCompressor,
                OodleCompressionLevel = cfg.OodleCompressionLevel,
                Version = cfg.Version,
                Unk14 = cfg.Unk14
            };

            foreach (string file in cfg.Files)
            {
                string filePath = Path.Combine(inFolder, file);
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Could not find file to repack into LTAR: \"{filePath}\"", filePath);
                }

                ltar.RootNode.MountFile(inFolder, filePath, 9);
            }

            string outPath = Path.Combine(outFolder, cfg.Name);
            FileEx.BackupFile(outPath);
            ltar.Write(outPath);
        }
    }
}
