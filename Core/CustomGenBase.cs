using System.IO;
using System.Text;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConstGen
{
    // constrain our type parameter to be only for the GeneratorBase class and also add a new type 
    // parameter where we can declare what kind of data the generate is using to store it's contants.
    // also create an instance creation constraint it will instantiate this type of class object
    public abstract class CustomGenBase<GenType, PropType> where GenType : CustomGenBase<GenType, PropType>, new()
    {
#region Variables =====================================================================================                
        /// <summary>
        /// An instance of the generator class itself
        /// it's main purpose is to instantiate and cache a ConstantGenerator as it's property
        /// to be used for generating the code
        /// </summary>
        protected static GenType instance;
        protected ConstantGenerator ConstGen_ = new ConstantGenerator();
        protected static ConstantGenerator _ConstGen { get { return instance.ConstGen_; } }

        public string outputPath;
        public string outputFileName;

        protected static string FilePath { get { return string.Format(ConstantGenerator.FilePathFormat, 
           instance.outputPath, instance.outputFileName); } }
        
        protected ConstGenSettings.IndentifierFormat indentifierFormat;
        protected List<PropType> properties;
#endregion Variables ==================================================================================

        protected string GetScriptName()
        {
            // "this" will pertain to the derived class's name inheriting from this base class
            // also it will return also the name of the namespace the class is it
            // so we gotta filter that out
            StringBuilder name = new StringBuilder( this.ToString() );
            int dotIndex = name.ToString().IndexOf('.');
            name.Remove( 0, dotIndex + 1 );

            return name.ToString();
        }

        protected static void CreateGeneratorInstance()
        {
            if (instance != null) return;

            // create self instance of the class
            // then create and cache the ConstantGenerator instance to it's property
            instance = new GenType();
            instance.ConstGen_ = new ConstantGenerator();
        }

        /// <summary>
        /// Retrieves the constants type the generator is designated to create
        /// with list return type specified by it
        /// </summary>
        /// <returns></returns>
        protected abstract List<PropType> RetriveValues();
        public abstract void Generate();
        protected abstract void SetPaths();

#region GenerateCode Overload Methods ====================================================================
        
        // Default
        protected void GenerateCode( System.Action<StringBuilder> contentWriteCallback )
        {
            _ConstGen.GenerateCodeFile(FilePath, outputPath, content =>
            {
                WrappedInt indentCount = 0;

                // NOTE: indentCount is automatically incremented or decremented by
                // ContentWriter methods 

                // An indent is added when a new CurlyBrackets IDisposable instance is created
                // and removed everytime that IDisposable instance is disposed of
                // (when the control has exited from the using statement)

                content.WriteIndentedLine( indentCount, _ConstGen.GetHeaderText( instance.GetScriptName() ) ); 
                content.WriteImports( "UnityEngine" );

                // write namespace name
                using (new CurlyBrackets(content, ConstantGenerator.OutputFileNamespace, indentCount))
                {
                    // write root class name
                    using (new CurlyBrackets(content, "public static class " + outputFileName, indentCount))
                    {
                        // write file contents
                        contentWriteCallback(content);
                    }                    
                }
            });
        }

        // namespace override
        protected void GenerateCode( System.Action<StringBuilder> contentWriteCallback, string namespaceName )
        {
            _ConstGen.GenerateCodeFile(FilePath, outputPath, content =>
            {
                WrappedInt indentCount = 0;

                // NOTE: indentCount is automatically incremented or decremented by
                // ContentWriter methods 

                // An indent is added when a new CurlyBrackets IDisposable instance is created
                // and removed everytime that IDisposable instance is disposed of
                // (when the control has exited from the using statement)

                content.WriteIndentedLine( indentCount, _ConstGen.GetHeaderText( instance.GetScriptName() ) ); 
                content.WriteImports( "UnityEngine" );

                // write namespace name
                using (new CurlyBrackets(content, "namespace " + namespaceName, indentCount))
                {
                    // write root class name
                    using (new CurlyBrackets(content, "public static class " + outputFileName, indentCount))
                    {
                        // write file contents
                        contentWriteCallback(content);
                    }                    
                }
            });
        }

        // namespace and imports override
       protected void GenerateCode( System.Action<StringBuilder> contentWriteCallback, string namespaceName ,string[] imports, 
        bool isRootClassStatic )
        {
            _ConstGen.GenerateCodeFile(FilePath, outputPath, content =>
            {
                WrappedInt indentCount = 0;

                // NOTE: indentCount is automatically incremented or decremented by
                // ContentWriter methods 

                // An indent is added when a new CurlyBrackets IDisposable instance is created
                // and removed everytime that IDisposable instance is disposed of
                // (when the control has exited from the using statement)

                content.WriteIndentedLine( indentCount, _ConstGen.GetHeaderText( instance.GetScriptName() ) ); 
                content.WriteImports( imports );

                // write namespace name
                using (new CurlyBrackets(content, "namespace " + namespaceName, indentCount))
                {

                    string className = "public {0} class {1}";

                    if ( isRootClassStatic ) {
                        className = string.Format( className, "static", outputFileName );
                    } else {
                        className = string.Format( className, "", outputFileName );
                    }

                    // write root class name
                    using (new CurlyBrackets(content, className, indentCount))
                    {
                        // write file contents
                        contentWriteCallback(content);
                    }                    
                }
            });
        }
#endregion GenerateCode Overload Methods ====================================================================
    }
}
