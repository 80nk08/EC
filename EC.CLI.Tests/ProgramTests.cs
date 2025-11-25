//using System;
//using System.CommandLine;
//using System.CommandLine.Parsing;
//using System.IO;
//using EC.CLI;
//using EC.Library.Converters;
//using EC.Library.Core;
//using Xunit;

//namespace EC.CLI.Tests;

//public class ProgramTests
//{
//    [Fact]
//    public void ParseCommandLine_ReturnsError_WhenMapFileMissing()
//    {
//        var args = new[] { "--type", "0", "--file", "nofile.txt" };
//        int result = Program.ParseCommandLine(args);
//        Assert.Equal(1, result);
//    }

//    [Fact]
//    public void ParseCommandLine_ReturnsError_WhenFileAndDirectoryMissing()
//    {
//        var tempMap = Path.GetTempFileName();
//        var args = new[] { "--type", "0", "--map", tempMap };
//        int result = Program.ParseCommandLine(args);
//        Assert.Equal(1, result);
//        File.Delete(tempMap);
//    }

//    [Fact]
//    public void ParseCommandLine_ReturnsError_WhenRequiredOptionsMissing()
//    {
//        var args = new[] { "--type", "0" };
//        int result = Program.ParseCommandLine(args);
//        Assert.Equal(1, result);
//    }

//    [Fact]
//    public void ParseCommandLine_ReturnsError_WhenInvalidEncodingType()
//    {
//        var tempMap = Path.GetTempFileName();
//        var args = new[] { "--type", "99", "--map", tempMap, "--file", tempMap };
//        int result = Program.ParseCommandLine(args);
//        Assert.Equal(1, result);
//        File.Delete(tempMap);
//    }

//    [Fact]
//    public void ParseCommandLine_ReturnsError_WhenFileAndDirectoryDoNotExist()
//    {
//        var tempMap = Path.GetTempFileName();
//        var args = new[] { "--type", "0", "--map", tempMap, "--file", "nofile.txt" };
//        int result = Program.ParseCommandLine(args);
//        Assert.Equal(1, result);
//        File.Delete(tempMap);
//    }

//    [Fact]
//    public void ParseCommandLine_ReturnsError_WhenMapFileDoesNotExist()
//    {
//        var args = new[] { "--type", "0", "--map", "nofile.map", "--file", "nofile.txt" };
//        int result = Program.ParseCommandLine(args);
//        Assert.Equal(1, result);
//    }

//    [Fact]
//    public void ParseCommandLine_ReturnsSuccess_ForValidFileConversion()
//    {
//        var tempMap = Path.GetTempFileName();
//        File.WriteAllText(tempMap, "A=B");
//        var tempFile = Path.GetTempFileName() + ".txt";
//        File.WriteAllText(tempFile, "A");
//        var args = new[] { "--type", "0", "--map", tempMap, "--file", tempFile };
//        int result = Program.ParseCommandLine(args);
//        Assert.Equal(0, result);
//        File.Delete(tempMap);
//        File.Delete(tempFile);
//    }

//    [Fact]
//    public void ParseCommandLine_ReturnsSuccess_ForValidDirectoryConversion()
//    {
//        var tempMap = Path.GetTempFileName();
//        File.WriteAllText(tempMap, "A=B");
//        var tempDir = Directory.CreateTempSubdirectory();
//        var tempFile = Path.Combine(tempDir.FullName, "file.txt");
//        File.WriteAllText(tempFile, "A");
//        var args = new[] { "--type", "0", "--map", tempMap, "--directory", tempDir.FullName };
//        int result = Program.ParseCommandLine(args);
//        Assert.Equal(0, result);
//        File.Delete(tempMap);
//        tempDir.Delete(true);
//    }

//    [Fact]
//    public void ParseCommandLine_ReturnsError_WhenNoArgumentsProvided()
//    {
//        var args = Array.Empty<string>();
//        int result = Program.ParseCommandLine(args);
//        Assert.Equal(1, result);
//    }
//}