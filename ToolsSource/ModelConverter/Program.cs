using ModelConverter.Convert;
using SaberTools.Convert;
using SaberTools.USF;
using SharpGLTF.Schema2;
using System;
using System.IO;
using System.Linq;

// This file is part of Model Converter for Space Marine 2
// Copyright (C) 2025 Neo_Kesha
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// See <https://www.gnu.org/licenses/> for more details.

/*
	Message to Modders of Space Marine 2 from the Author.
	This is the final release of Model Converter, hence i am releasing its source code to the public.
	It got some dirty code, some bad practices, some bugs, but i tried to clean it up as much as possible.
	Thank you for your support during development of the tool, especially thank you for all amazing work you did with it.
	I tried my best to implement all the required features i could, such as PvE customization support and TPL animations.
	
	As source code gets released, i drop the support of it and handle it to the community.
*/

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintHelp();
            return;
        }
        ConvertParams convertParams = new ConvertParams();

        for (var i = 0; i < args.Length; ++i)
        {
            var arg = args[i];
            if (arg.Equals("--fixup"))
            {
                if (args.Length <= i + 1)
                {
                    continue;
                }

                bool value = true;
                if (Boolean.TryParse(args[i + 1], out value))
                {
                    convertParams.fixData = value;
                }
                ++i;
            }
            else if (arg.Equals("--no-skin"))
            {
                convertParams.noSkin = true;
            }
            else if (arg.Equals("--experimental"))
            {
                convertParams.experimental = true;
            }
            else
            {
                if (convertParams.inputFileName == null)
                {
                    convertParams.inputFileName = arg;
                }
                else
                {
                    convertParams.outputFileName = arg;
                }
            }
        }

        if (!File.Exists(convertParams.inputFileName))
        {
            Console.WriteLine("Input file not found.");
            return;
        }

        var ext = Path.GetExtension(convertParams.inputFileName).ToLower();
        var outExt = "";
        var toGltf = false;
        switch (ext)
        {
            case ".usf":
            case ".s3dusf":
                outExt = ".gltf";
                toGltf = true;
                break;
            case ".gltf":
                outExt = ".usf";
                toGltf = false;
                break;
            default:
                Console.WriteLine("Extension not supported");
                return;
        }
        if (convertParams.outputFileName == null)
        {
            var extLen = ext.Length;
            convertParams.outputFileName = convertParams.inputFileName.Substring(0, convertParams.inputFileName.Length - extLen) + outExt;
        }

        var outputDir = convertParams.outputFileName.Substring(0, convertParams.outputFileName.Length - Path.GetFileName(convertParams.outputFileName).Length);
        Directory.CreateDirectory(outputDir);

        if (toGltf)
        {
            Usf2Gltf(convertParams);
        }
        else
        {
            Gltf2Usf(convertParams);
        }

        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
    }

    public static void PrintHelp()
    {
        Console.WriteLine("Usage: ModelConverter <inputFile> [outputFile] [options]");
        Console.WriteLine("Options: ");
        Console.WriteLine("\tInput file *.usf -> Convert USF to GLTF");
        Console.WriteLine("\tInput file *.gltf -> Convert GLTF to USF");
        Console.WriteLine("\t--no-skin -> Convert USF without skin data");
        Console.WriteLine("\t--experimental -> Convert USF without skin data");
    }

    public static void Usf2Gltf(ConvertParams convertParams)
    {
        var usf = new Usf();
        using (var usfFile = new FileStream(convertParams.inputFileName, FileMode.Open, FileAccess.Read))
        {
            var reader = new BinaryReader(usfFile);
            usf.Read(reader);
        }

        var scene = UsfConverter.ConvertUsf(usf, convertParams.noSkin);
        
        try
        {
            var gltf = scene.ToGltf2();
            gltf.SaveGLTF(convertParams.outputFileName);
        }
        catch
        {

        }

    }

    public static void Gltf2Usf(ConvertParams convertParams)
    {
        ModelRoot gltf = null;
        try
        {
            gltf = ModelRoot.Load(convertParams.inputFileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine("SharpGLTF library threw an exception:");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return;
        }
        var a = 0;
        var gltfScene = gltf.DefaultScene;
        gltfScene.Name = Path.GetFileNameWithoutExtension(convertParams.inputFileName);
        var usf = GltfConverter.ConvertGltf(gltfScene, convertParams.experimental);
        using (var usfFile = new FileStream(convertParams.outputFileName, FileMode.Create, FileAccess.Write))
        {
            var writer = new BinaryWriter(usfFile);
            usf.Write(writer);
        }
    }

    public class ConvertParams
    {
        public string inputFileName;
        public string outputFileName;
        public bool fixData;
        public bool noSkin = false;
        public bool experimental = false;
    }

}