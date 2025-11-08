using EC.Library.Convertors;
using EC.Library.Core;
using System.CommandLine;
using System.CommandLine.Parsing;
namespace EC.CLI;

internal class Program
{
    static readonly string[] WordExtensions = [".doc", ".docx"];
    static readonly string[] ExcelExtensions = [".xls", ".xlsx"];
    static readonly string[] SupportedExtensions = [.. WordExtensions, .. ExcelExtensions];
    static int Main(string[] args)
    {
        var fileOption = new Option<FileInfo?>("--file")
        {
            Description = "Path to the file to convert"
        };
        var directoryOption = new Option<DirectoryInfo?>("--directory")
        {
            Description = "Path to the directory to convert all supported files within"
        };

        var typeOption = new Option<EncodingType>("--type")
        {
            Description = "Encoding conversion type (ANSIToUnicode = 0 or UnicodeToANSI = 1)",
            Required = true
        };

        var rootCommand = new RootCommand("Encoding Converter CLI")
        {
            fileOption,
            directoryOption,
            typeOption
        };
        rootCommand.Validators.Add(commandResult =>
        {
            var file = commandResult.GetResult(fileOption);
            var directory = commandResult.GetResult(directoryOption);
            if (file is null && directory is null)
            {
                commandResult.AddError("You must specify either --file or --directory.");
            }
            else if (file is not null && directory is not null)
            {
                commandResult.AddError("You cannot specify both --file and --directory.");
            }
        });

        var parseResult = rootCommand.Parse(args);
        if (parseResult.Errors.Count > 0)
        {
            foreach (ParseError parseError in parseResult.Errors)
                Console.Error.WriteLine(parseError.Message);
            return 1;
        }

        var encodingMapper = EncodingMapper.FromFile("EncodingMappers\\armenian.map");
        var textConvertor = new TextConvertor(encodingMapper);
        using var wordConvertor = new WordConvertor(textConvertor, true);
        using var excelConvertor = new ExcelConvertor(textConvertor, true);


        var fileInfo = parseResult.GetValue(fileOption);
        var directoryInfo = parseResult.GetValue(directoryOption);
        var encodingType = parseResult.GetValue(typeOption);

        var fontName = encodingType == EncodingType.ANSIToUnicode ? "Sylfaen" : "Arial";

        if (fileInfo is not null)
        {
            if (WordExtensions.Contains(Path.GetExtension(fileInfo.FullName), StringComparer.OrdinalIgnoreCase))
            {
                wordConvertor.Convert(fileInfo.FullName, encodingType, fontName);
                return 0;
            }

            if (ExcelExtensions.Contains(Path.GetExtension(fileInfo.FullName), StringComparer.OrdinalIgnoreCase))
            {
                excelConvertor.Convert(fileInfo.FullName, encodingType, fontName);
                return 0;
            }

        }

        if (directoryInfo is not null)
        {

            var files = Directory.GetFiles(directoryInfo.FullName, "*.*", SearchOption.AllDirectories)
                .Where(f => SupportedExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                .ToArray();
            foreach (var f in files)
            {
                var file = new FileInfo(f).FullName;
                if (WordExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Converting Word file: {file}");
                    wordConvertor.Convert(file, encodingType, fontName);
                }
                else if (ExcelExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Converting Excel file: {file}");
                    excelConvertor.Convert(file, encodingType, fontName);
                }
            }
            return 0;

        }

        return 1;

    }
}
