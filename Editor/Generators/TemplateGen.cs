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
    [InitializeOnLoad] 
    public class TemplateGen 
    {

        #region Variables ==================================================================================================================
        /// <summary>
        /// An instance of the generator class itself
        /// it's main purpose is to instantiate and cache a ConstantGenerator as it's property
        /// to be used for generating the code
        /// </summary>
        private static TemplateGen instance;

        private ConstantGenerator generator_ = new ConstantGenerator();
        private static ConstantGenerator generator { get { return instance.generator_; } }

         private static string TemplatePath { get { return string.Format(ConstantGenerator.TemplatePathFormat, generator.GetTemplatesPath(), "GeneratorTemplate"); } }
        #endregion Variables ===============================================================================================================

        private static string GetFilePath( string fileName )
        {
            return string.Format(ConstantGenerator.FilePathFormat, generator.GetTemplateOutputPath(), fileName);
        }

        // static constructor
        // get's called with the class when [InitializeOnLoad] happens
        static TemplateGen()
        {
            if (instance != null) return;

            instance = new TemplateGen();
            instance.generator_ = new ConstantGenerator();

        }

        public static void GenerateTemplate( string generatorName, string outputFileName )
        {
            generator.GenerateCodeFile(GetFilePath(generatorName), generator.GetTemplateOutputPath() ,content =>
            {
                string templateFile = File.ReadAllText( TemplatePath );
                templateFile = templateFile
                    .Replace( "~NameSpace~", "ConstGen" )
                    .Replace( "~ClassName~", generatorName )
                    .Replace( "~OutputFileName~", outputFileName );

                content.Append( templateFile );
            });
        }
    }
}


