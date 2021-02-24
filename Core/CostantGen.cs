using System.IO;
using System.Text;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConstGen
{
    public class EnumConstant
    {
        public string name;
        public Dictionary<string,Nullable<int>> values;

        public EnumConstant( string name_ )
        {
            name = name_;
            values = new Dictionary<string, int?>();
        }
    }

    public static class EnumConstAdder
    {
        public static void Add( this EnumConstant ec, string valueName, int? value )
        {
            ec.values.Add( valueName, value );
        }
    }

    public class CostantGen
    {
#region Variables =====================================================================================  
        public class GenerationPaths
        {
            public string filePath;
            public string outputDirectory;
            public string fileClassName;

            public GenerationPaths( string filepath_ )
            {
                filePath = filepath_;
                int index = filePath.LastIndexOf("/");
                outputDirectory = filePath.Remove( index, filePath.Length - index );
                fileClassName = filePath.Substring(index+1);
                filePath += ".cs";
            }     

            public GenerationPaths( string outputDirectory_, string fileName )
            {
                outputDirectory = outputDirectory_;
                fileClassName = fileName;
                filePath = outputDirectory_ + fileName + ".cs";
            } 
        }

        /// <summary>
        /// An instance of the generator class itself
        /// it's main purpose is to instantiate and cache a ConstantGenerator as it's property
        /// to be used for generating the code
        /// </summary>
        protected static CostantGen instance;
        protected ConstantGenerator ConstGen_ = new ConstantGenerator();
        protected static ConstantGenerator _ConstGen { get { return instance.ConstGen_; } }

        private const string DefaultOutputPath = "Assets/Scripts/ConstGen Files/Generated Enums/";
        
#endregion Variables ==================================================================================

        private string GetScriptName()
        {
            // "this" will pertain to the derived class's name inheriting from this base class
            // also it will return also the name of the namespace the class is it
            // so we gotta filter that out
            StringBuilder name = new StringBuilder( this.ToString() );
            int dotIndex = name.ToString().IndexOf('.');
            name.Remove( 0, dotIndex + 1 );

            return name.ToString();
        }


        private static void CreateGeneratorInsance()
        {
            if (instance != null) return;

            // create self instance of the class
            // then create and cache the ConstantGenerator instance to it's property
            instance = new CostantGen();
            instance.ConstGen_ = new ConstantGenerator();
        }


        private void GenerateEnumContent( StringBuilder content, List<EnumConstant> enums, WrappedInt indentCount )
        {
            // loop through the enums
            foreach (EnumConstant enumConstant in enums)
            {
                // store enum name and constant name and value
                string enumName = enumConstant.name;
                Dictionary<string,int?> constantsDict = enumConstant.values;
                // "int?" is the shortcut for "Nullable<int>"

                // write enum header
                using (new CurlyBrackets(content, "public enum " + enumName, indentCount))
                {
                    int dictLength = constantsDict.Count;
                    int loopIndex = 0;
                    
                   foreach ( var constant_ in constantsDict )
                    {
                        string format = "{0}";
                        Nullable<int> value = constant_.Value;

                        if ( value == null ) {
                            format = string.Format( format, constant_.Key );
                        } else { 
                            format += " = {1}";
                            format = string.Format( format, constant_.Key, value );
                        }
                            
                        // exclude out the "," at the last enum constant line 
                        if ( loopIndex < dictLength-1 ) {
                            content.WriteIndentedLine( indentCount, format + "," );
                        } else {
                            content.WriteIndentedLine( indentCount, format );
                        }

                        loopIndex++;
                    }
                }

                content.WriteNewLine();
            }
        }
#region GenerateEnums Overloads ===========================================================================
        
        /// <summary>
        /// Generates the enums into the target file in the directory with the given namepsace name
        /// </summary>
        /// <param name="outputFilePath">file path of the generated enum (if path is non-existent, one shall be created)</param>
        /// <param name="namespaceName">namespace name</param>
        /// <param name="enums">dictionaries containing enums to be written into the file (TKey:string is the enums name and TValue:string[] is the array of values it'll have</param>
        public static void GenerateEnums( string fileName, string namespaceName, List<EnumConstant> enums, string outputDirectory = DefaultOutputPath )
        {
            CreateGeneratorInsance();
            GenerationPaths gp = new GenerationPaths( outputDirectory, fileName );

            _ConstGen.GenerateCodeFile(gp.filePath, gp.outputDirectory, content =>
            {
                WrappedInt indentCount = 0;

                using (new CurlyBrackets(content, "namespace " + namespaceName, indentCount))
                {
                    instance.GenerateEnumContent( content, enums, indentCount );
                }
            });
        }    

        /// <summary>
        /// Generates the enums into the target file in the directory with a given class name
        /// </summary>
        /// <param name="outputFilePath">file path of the generated enum, the name of the file will be the name of the class (if path is non-existent, one shall be created)</param>
        /// <param name="isClassStatic">write the class as static?</param>
        /// <param name="enums">dictionaries containing enums to be written into the file (TKey:string is the enums name and TValue:string[] is the array of values it'll have</param>
        public static void GenerateEnums( string fileName, bool isClassStatic, List<EnumConstant> enums, string outputDirectory = DefaultOutputPath )
        {
            CreateGeneratorInsance();            
            GenerationPaths gp = new GenerationPaths( outputDirectory, fileName );

            _ConstGen.GenerateCodeFile(gp.filePath, gp.outputDirectory, content =>
            {
                WrappedInt indentCount = 0;

                string className = "public{0} class {1}";

                if ( isClassStatic ) {
                    className = string.Format( className, " static", gp.fileClassName );
                } else {
                    className = string.Format( className, "", gp.fileClassName );
                }

                using (new CurlyBrackets(content, className, indentCount))
                {
                    instance.GenerateEnumContent( content, enums, indentCount );
                }
            });
        }

        /// <summary>
        /// Generates the enums into the target file in the directory without a class or namespace
        /// </summary>
        /// <param name="outputFilePath">file path of the generated enum (if path is non-existent, one shall be created)</param>
        /// <param name="enums">dictionaries containing enums to be written into the file (TKey:string is the enums name and TValue:string[] is the array of values it'll have</param>
        public static void GenerateEnums( string fileName, List<EnumConstant> enums, string outputDirectory = DefaultOutputPath )
        {
            CreateGeneratorInsance();
            GenerationPaths gp = new GenerationPaths( outputDirectory, fileName );

            _ConstGen.GenerateCodeFile(gp.filePath, gp.outputDirectory, content =>
            {
                WrappedInt indentCount = 0;
                instance.GenerateEnumContent( content, enums, indentCount );
            });
        }
#endregion GenerateEnums Overloads ===========================================================================
    }    
}


