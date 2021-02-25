using System.IO;
using System.Text;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;
#endif

namespace ConstGen
{
    // constrain our type parameter to be only for the GeneratorBase class and also add a new type 
    // parameter where we can declare what kind of data the generate is using to store it's contants.
    // also create an instance creation constraint it will instantiate this type of class object
    public abstract class GeneratorBase<GenType, PropType> where GenType : GeneratorBase<GenType, PropType>, new()
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
        protected static string FilePath { get { return string.Format(ConstantGenerator.FilePathFormat, 
            _ConstGen.GetOutputPath(), instance.GetOutputFileName()); } }
        
        protected bool regenerateOnMissing;
        protected bool updateOnReload;
        protected ConstGenSettings.IndentifierFormat indentifierFormat;
        protected List<PropType> oldProperties;
        protected List<PropType> newProperties;
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

        protected static void CheckForRegenOrUpdate( System.Action onMissingFile )
        {
            if ( !File.Exists(FilePath) ) // check if file exist
            {
                if ( instance.regenerateOnMissing )
                { // if allowed, generate a new since none is present
                    Debug.LogWarning( "[ " + instance.GetOutputFileName() + " ] is not found, Generating a new one...");
                    onMissingFile();
                } 
                else if ( instance.updateOnReload )
                { // log that generator tried to update a non existent file
                    Debug.LogWarning( "[ " + instance.GetOutputFileName() + " ] Update generation failed because file is non-existent" );
                }
            } 
            else 
            {
                // file exist and check  if we can updateOnReload
                if ( instance.updateOnReload )
                    instance.UpdateFile();
            }
        }

        /// <summary>
        /// Generators automatically generate their out files when not present
        /// so we can force them to generate it by deleting the file
        /// </summary>
        public static void ForceGenerate()
        {
            CreateGeneratorInstance();

            if ( File.Exists(FilePath) ) {
#if UNITY_EDITOR
                AssetDatabase.DeleteAsset( FilePath );
                AssetDatabase.Refresh();
#endif
            }
            else {
                Debug.LogWarning( "[ " + instance.GetOutputFileName() + " ] Force Generate Failed, trying to delete an non existent file" );                
            }
        }

        /// <summary>
        /// Retrieves Settings SO file
        /// </summary>
        /// <param name="propertyCaching">callback for caching the list type data of the generator stored on the SO to it's instance</param>
        /// <returns>if the settings and the list data is successfully retrieve</returns>
        protected static bool RetrieveSettings( System.Action propertyCaching )
        {
            bool successful = false; 

            try {
                ConstGenSettings cgs = ConstantGenerator.GetSettingsFile();
                instance.regenerateOnMissing = cgs.regenerateOnMissing;
                instance.updateOnReload = cgs.updateOnReload;
                instance.indentifierFormat = cgs.indentifierFormat;

                propertyCaching();
                successful = true;
            }
            catch (System.Exception) {
                successful = false;
            }

            return successful;
        }

        /// <summary>
        /// checks if there is any changes on the constants 
        /// </summary>
        protected abstract void UpdateFile();

        /// <summary>
        /// Retrieves the constants type the generator is designated to create
        /// with list return type specified by it
        /// </summary>
        /// <returns></returns>
        protected abstract List<PropType> RetriveValues();
        protected abstract string GetOutputFileName();

        protected virtual void GenerateCode( System.Action<StringBuilder> contentWriteCallback )
        {
            _ConstGen.GenerateCodeFile(FilePath, _ConstGen.GetOutputPath() , content =>
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
                    using (new CurlyBrackets(content, "public static class " + GetOutputFileName(), indentCount))
                    {
                        // write file contents
                        contentWriteCallback(content);
                    }                    
                }
            });
        }
    }
}


