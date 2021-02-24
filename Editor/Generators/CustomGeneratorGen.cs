using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConstGen
{
    public class CustomGeneratorGen
    {
        #region Variables ==================================================================================================================
        private static CustomGeneratorGen instance;
        private ConstantGenerator generator_ = new ConstantGenerator();
        private static ConstantGenerator generator { get { return instance.generator_; } }
        private static string TemplatePath { get { return string.Format(ConstantGenerator.TemplatePathFormat, generator.GetTemplatesPath(), "CustomGenTemplate"); } }
        #endregion Variables ===============================================================================================================

        private static string GetFilePath( string fileName )
        {
            return string.Format(ConstantGenerator.FilePathFormat, generator.GetTemplateOutputPath(), fileName);
        }

        // static constructor
        // get's called with the class when [InitializeOnLoad] happens
        static CustomGeneratorGen()
        {
            if (instance != null) 
                return;

            instance = new CustomGeneratorGen();
            instance.generator_ = new ConstantGenerator();
        }

        public static void CreateGenerator( string generatorName, string outputFileName, DT outputType, string outputFilePath )
        {
            generator.GenerateCodeFile(GetFilePath(generatorName), generator.GetTemplateOutputPath(), content =>
            {
                string templateFile = File.ReadAllText( TemplatePath );
                templateFile = templateFile
                    // .Replace( "~NAMESPACE~", "ConstGen" )
                    .Replace( "~CLASSNAME~", generatorName )
                    .Replace( "~OUTPUTFILENAME~", outputFileName )
                    .Replace( "~OUTPUTPATH~", "Assets/" + outputFilePath )
                    .Replace( "~OUTPUTTYPE~", outputType.ToString().ToLower() );

                content.Append( templateFile );
            });
        }
    }   
}
