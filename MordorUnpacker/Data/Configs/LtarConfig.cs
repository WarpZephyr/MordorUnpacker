using MordorUnpacker.Configuration;
using OodleCoreSharp;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace MordorUnpacker.Data.Configs
{
    /// <inheritdoc/>
    [JsonSourceGenerationOptions(WriteIndented = true,
        GenerationMode = JsonSourceGenerationMode.Metadata,
        IncludeFields = true,
        UseStringEnumConverter = true)]
    [JsonSerializable(typeof(LtarConfig))]
    internal partial class LtarConfigSerializerContext : JsonSerializerContext
    {
    }

    /// <summary>
    /// A config json for repacking lithtech archives.
    /// </summary>
    internal class LtarConfig
    {
        /// <summary>
        /// The name of the config file.<br/>
        /// Constant and not included in the config itself.
        /// </summary>
        [JsonIgnore]
        public const string ConfigFileName = "_mordor_ltar.json";

        /// <summary>
        /// The name of the lithtech archive file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether or not the lithtech archive was in big endian.
        /// </summary>
        public bool BigEndian { get; set; }

        /// <summary>
        /// Whether or not to prefer smallest compression for zlib when repacking, will make compression much slower.
        /// </summary>
        public bool UseMaxZlibCompressionLevel { get; set; }

        /// <summary>
        /// The oodle compressor to prefer when repacking, certain compressors may be unstable.
        /// </summary>
        public OodleLZ_Compressor OodleCompressor { get; set; }

        /// <summary>
        /// The oodle compression level to prefer when repacking, certain compression levels may be unstable.
        /// </summary>
        public OodleLZ_CompressionLevel OodleCompressionLevel { get; set; }

        /// <summary>
        /// The version of the lithtech archive.<br/>
        /// Notes for 3:<br/>
        /// - Used in Middle Earth: Shadow of Mordor<br/>
        /// - File extension: .arch05<br/>
        /// - Compression is max zlib deflate.<br/>
        /// - Paths are aligned to 4 bytes.<br/>
        /// - "Flags" in file entries are a 4 byte int.<br/>
        /// <br/>
        /// Notes for 4:<br/>
        /// - Used in Middle Earth: Shadow of War<br/>
        /// - File extension: .arch06<br/>
        /// - Compression uses oodle 5 of unknown settings.<br/>
        /// - Paths are not aligned.<br/>
        /// - There is an unknown byte of value 1 in file entries just above "Flags".<br/>
        /// - "Flags" in file entries are 1 byte.<br/>
        /// - There is always (6 * fileCount) padding bytes before data.<br/>
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Unknown; Always 1.
        /// </summary>
        public int Unk14 { get; set; }

        /// <summary>
        /// A list of relative file paths contained within the lithtech archive.
        /// </summary>
        public List<string> Files { get; set; }

        /// <summary>
        /// Creates a new <see cref="LtarConfig"/> with default values.
        /// </summary>
        public LtarConfig()
        {
            Name = string.Empty;
            BigEndian = false;
            UseMaxZlibCompressionLevel = false;
            OodleCompressor = OodleLZ_Compressor.Mermaid;
            OodleCompressionLevel = OodleLZ_CompressionLevel.HyperFast4;
            Version = 4;
            Unk14 = 1;
            Files = [];
        }

        /// <summary>
        /// Loads an <see cref="LtarConfig"/> from the specified folder.
        /// </summary>
        /// <param name="folder">The folder path to load an <see cref="LtarConfig"/> from.</param>
        /// <returns>A newly read <see cref="LtarConfig"/>.</returns>
        public static LtarConfig Load(string folder)
        {
            string inPath = Path.Combine(folder, ConfigFileName);
            return DataConfigLoader.Load(inPath, LtarConfigSerializerContext.Default.LtarConfig);
        }

        /// <summary>
        /// Saves this <see cref="LtarConfig"/> to the specified folder.
        /// </summary>
        /// <param name="folder">The folder path to save this <see cref="LtarConfig"/> to.</param>
        public void Save(string folder)
        {
            Directory.CreateDirectory(folder);
            string outPath = Path.Combine(folder, ConfigFileName);
            DataConfigLoader.Save(this, outPath, LtarConfigSerializerContext.Default.LtarConfig);
        }
    }
}
