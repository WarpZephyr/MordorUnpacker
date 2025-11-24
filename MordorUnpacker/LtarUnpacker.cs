using MordorFormats.LTAR;
using MordorUnpacker.Logging;
using System.IO;

namespace MordorUnpacker
{
    internal static class LtarUnpacker
    {
        public static void Unpack(LtarReader ltar, string outFolder)
        {
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
                    Log.WriteLine(Path.Combine(lFolder.Name, lFile.Name));

                    using var fs = File.OpenWrite(outLPath);
                    ltar.ReadFile(lFile, fs);
                }

                // Increment the total file index
                fileIndexBase += lFolder.FileCount;
            }
        }
    }
}
