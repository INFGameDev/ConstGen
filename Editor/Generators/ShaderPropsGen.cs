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
    public class ShaderPropsGen : AssetPostprocessor
    {
        #region Variables =================================================================================================================
        private const string FileName = "_SHADERPROPS";
        
        /// <summary>
        /// An instance of the generator class itself
        /// it's main purpose is to instantiate and cache a ConstantGenerator as it's property
        /// to be used for generating the code
        /// </summary>
        private static ShaderPropsGen instance;
        private ConstantGenerator generator_ = new ConstantGenerator();
        private static ConstantGenerator generator { get { return instance.generator_; } }
        private static string FilePath { get { return string.Format(ConstantGenerator.FilePathFormat, generator.GetOutputPath(), FileName); } }
        
        private bool regenerateOnMissing;
        private bool updateOnReload;
        private List<ConstGenSettings.Shader_> newProperties;
        private List<ConstGenSettings.Shader_> oldProperties;

        private bool updateGeneration;
        #endregion Variables ===============================================================================================================

        // executed when an asset is imported, deleted or changed path
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            InitializeGen();
        }

        private static void InitializeGen()
        {
            CreateGeneratorInsance();
            
            if ( !RetrieveSettingsData() )
                return;

            if ( !File.Exists(FilePath) ) // check if file exist
            {
                if ( instance.regenerateOnMissing )
                { // if allowed, generate a new since none is present
                    Debug.LogWarning( "[ " + FileName + " ] is not found, Generating a new one...");
                    Generate( false );
                } 
                else if ( instance.updateOnReload )
                { // log that generator tried to update a non existent file
                    Debug.LogWarning( "[ " + FileName + 
                        " ] Update generation failed because file is non-existent" );
                }
            } 
            else 
            {
                // file exist and check  if we can updateOnReload
                if ( instance.updateOnReload ) {
                    UpdateFile();
                }
            }
        }


        private static void CreateGeneratorInsance()
        {
            if (instance != null) return;

            // create self instance of the class
            // then create and cache the ConstantGenerator instance to it's property
            instance = new ShaderPropsGen();
            instance.generator_ = new ConstantGenerator();
        }


        private static bool RetrieveSettingsData()
        {
            bool successful = false;

            try
            {
                ConstGenSettings cgs = ConstantGenerator.GetSettingsFile();

                instance.regenerateOnMissing = cgs.regenerateOnMissing;
                instance.updateOnReload = cgs.updateOnReload;
                instance.oldProperties = cgs._SHADERPROPS;

                successful = true;
            }
            catch (System.Exception)
            {
                successful = false;
            }

            return successful;
        }

        /// <summary>
        /// Generates the file by writing new updated contents or generates the file is none is present
        /// </summary>
        /// <param name="updateGen">is this generation from a property update?</param>
        public static void Generate( bool updateGen )
        {
            CreateGeneratorInsance();
            instance.newProperties = RetriveValues();
    
            instance.updateGeneration = updateGen;

            // // store the new properties to SO
            ConstantGenerator.GetSettingsFile()._SHADERPROPS.Clear();
            ConstantGenerator.GetSettingsFile()._SHADERPROPS = instance.newProperties;

            // // set SO to be dirty to be saved
            EditorUtility.SetDirty( ConstantGenerator.GetSettingsFile() );

            GenerateCode();
        }

        /// <summary>
        /// Generators automatically generate their out files when not present
        /// so we can force them to generate it by deleting the file
        /// </summary>
        public static void ForceGenerate()
        {
            CreateGeneratorInsance();

            if ( File.Exists(FilePath) ) {
                AssetDatabase.DeleteAsset( FilePath );
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning( "[ " + FileName + " ] Force Generate Failed, trying to delete an non existent file" );                
            }
        }

        /// <summary>
        /// checks if there is any changes on the constants 
        /// </summary>
        private static void UpdateFile()
        {
            if (Application.isPlaying) return;

            bool generate = false;
            instance.newProperties = RetriveValues();
            
            // check if the number of shaders in the assets has changed
            if ( instance.oldProperties.Count != instance.newProperties.Count ) {
                generate = true;
            } 
            else // else check for changes in the properties of the shaders
            { 
                // loop through all shaders
                for (int i = 0; i < instance.oldProperties.Count; i++) 
                {
                    ConstGenSettings.Shader_ oldShader = instance.oldProperties[i];
                    ConstGenSettings.Shader_ newShader = instance.newProperties[i];

                    // check if any properties is added or removed
                    if ( oldShader.name != newShader.name || 
                        oldShader.properties.Count != newShader.properties.Count ) {

                        generate = true;
                        break;
                    }
                    else // else check if any of the name of properties changed
                    {
                        // loop through properties
                        for (int i2 = 0; i2 < oldShader.properties.Count; i2++)
                        {
                            string oldName = oldShader.properties[i2];
                            string newName = newShader.properties[i2];

                            // compare property names
                            if ( oldName != newName ) {
                                generate = true;
                                break;
                            }
                        }
                    }

                    if ( generate ) // break out of shaders loop
                        break;
                }
            }

            if ( generate ) {  
                Generate( true );
            }
                
        }

        private string GetScriptName()
        {
            StringBuilder name = new StringBuilder( this.ToString() );
            int dotIndex = name.ToString().IndexOf('.');
            name.Remove( 0, dotIndex+1 );

            return name.ToString();
        }

        static List<ConstGenSettings.Shader_> RetriveValues()
        {
            // to store the duplicated shader names and how many duplicates are there
            Dictionary<string,int> duplicates = new Dictionary<string, int>();
            List<ConstGenSettings.Shader_> shaderList = new List<ConstGenSettings.Shader_>();

            // find shaders
            string[] shadersGUIDPath = AssetDatabase.FindAssets("t:shader");
            
            foreach (string path in shadersGUIDPath)
            {
                // ====================================================================
                // get shader and it's path
                string shaderPath = AssetDatabase.GUIDToAssetPath(path);
                Shader shaderFile = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);

                if (shaderFile == null) continue;
                // ====================================================================
                ConstGenSettings.Shader_ shader_ = new ConstGenSettings.Shader_();
                List<string> propertyNames = new List<string>();

                // remove shader path from shader name
                int index = shaderFile.name.LastIndexOf( '/' );
                StringBuilder shaderName = new StringBuilder(); 
                shaderName.Append(shaderFile.name.Remove( 0, index+1 ));

                // remove any characters not okay for variable naming
                shader_.name = generator.CreateIdentifier( shaderName.ToString() );
                // ====================================================================

                int propertiesCount = ShaderUtil.GetPropertyCount(shaderFile);
                if ( propertiesCount == 0 ) 
                    continue;

                // loop through shader properties and cache it to propertyNames
                for (int i = 0; i < propertiesCount; i++)
                {
                    string propertyName = ShaderUtil.GetPropertyName(shaderFile, i);
                    propertyNames.Add( propertyName );
                }

                // before adding the new shader to the list check first if it has conflicting names
                foreach (ConstGenSettings.Shader_ s in shaderList )
                {
                    // if it has
                    if ( s.name == shader_.name ) 
                    {
                        // check the list  of duplicates if it's already added in there
                        if ( duplicates.ContainsKey(shader_.name) )
                        {
                            // if it is increment the duplicates count and add the duplicate count
                            // to the end of the shader name
                            duplicates[shader_.name]++;
                            shader_.name += "_D" + duplicates[shader_.name];
                        } 
                        else
                        {
                            duplicates.Add( shader_.name, 1 );
                            shader_.name += "_D1";
                        }

                        break;
                    }
                }

                // remove any duplicate shader properties and store it in shaderData.properties
                shader_.properties = propertyNames.Distinct().ToList();
                shaderList.Add( shader_ );
            }

            return shaderList;
        }

        private static void GenerateCode()
        {   
            generator.GenerateCodeFile(FilePath, generator.GetOutputPath() ,content =>
            {
                WrappedInt indentCount = 0;

                // NOTE: indentCount is automatically increamented or decreamented by
                // ContentWriter methods 

                // An indent is added when a new CurlyBrackets IDisposable instance is created
                // and removed everytime that IDisposable instance is disposed of
                // (when the control has excited from the using statement)

                content.WriteIndentedLine( indentCount, generator.GetHeaderText( instance.GetScriptName() ) ); 
                content.WriteImports( "UnityEngine" );

                using (new CurlyBrackets(content, ConstantGenerator.OutputFileNamespace, indentCount))
                {
                    using (new CurlyBrackets(content, "public static class " + FileName, indentCount))
                    {
                        foreach (ConstGenSettings.Shader_ sd in instance.newProperties)
                        {
                            // write shader header name group
                            content.WriteIndentedFormatLine(indentCount, "public static class {0}", generator.CreateIdentifier(sd.name));
                            using (new CurlyBrackets(content, indentCount))
                            {
                                foreach (string propertyName in sd.properties)
                                {
                                    // write properties
                                    content.WriteIndentedFormatLine(indentCount, 
                                        "public const string {0} = \"{1}\";", 
                                            generator.CreateIdentifier(propertyName).Remove(0,1), 
                                                generator.EscapeDoubleQuote(propertyName));
                                }
                            }
                            content.WriteNewLine();
                        }
                    }                    
                }
            });

            // imports the output file when the generation method is through an update
            // so we can force recompile the editor

            // NOTE: for some reason AssetDatabase.Refresh()
            // doesn't immidiately trigger editor recompiling so this method is used instead
            if ( instance.updateGeneration )
                AssetDatabase.ImportAsset( FilePath  );
        }
    }
}