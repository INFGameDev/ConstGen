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
    public class AnimParamsGen : GeneratorBase<AnimParamsGen, ConstGenSettings.ParamsCTRLR>
    {
        private const string FILENAME = "_ANIMPARAMS";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            CreateGeneratorInsance();

            if ( !RetrieveSettings( ()=> instance.oldProperties = ConstantGenerator.GetSettingsFile()._ANIMPARAMS ) )
                return; 

            CheckForRegenOrUpdate( ()=> Generate() );
        }

        /// <summary>
        /// Generates the file by writing new updated contents or generates the file is none is present
        /// </summary>
        public static void Generate()
        {
            CreateGeneratorInsance();
            instance.newProperties = instance.RetriveValues();

            // store the new properties to SO
            ConstantGenerator.GetSettingsFile()._ANIMPARAMS.Clear();
            ConstantGenerator.GetSettingsFile()._ANIMPARAMS = instance.newProperties;

            // set SO to be dirty to be saved
            EditorUtility.SetDirty( ConstantGenerator.GetSettingsFile() );

            instance.GenerateCode(
                content =>
                {
                    WrappedInt indentCount = 2; 

                    foreach (ConstGenSettings.ParamsCTRLR ctrlr in instance.newProperties)
                    {
                        // write animator owner header group of the parameters
                        string animatorName = string.Format( ConstantGenerator.ClassFormat, _ConstGen.MakeIdentifier(ctrlr.name) );
                        using (new CurlyBrackets(content, animatorName, indentCount))
                        {
                            // write parameters
                            foreach (string parameter in ctrlr.parameters )
                            {
                                content.WriteConstant( 
                                    indentCount, 
                                    DT.String, _ConstGen.MakeIdentifier(parameter), 
                                    _ConstGen.EscapeDoubleQuote(parameter) 
                                );
                            }
                        }
                        content.WriteNewLine();
                    }
                }
            );
        }

        protected override void UpdateFile()
        {
            if (Application.isPlaying) return;

            bool generate = false;
            instance.newProperties = RetriveValues();

            // check if the number of animation controllers in the assets has changed
            if ( instance.oldProperties.Count != instance.newProperties.Count ) {
                generate = true;
            } 
            else // else check for changes in the parameters of the controllers
            { 
                // loop through animators
                for (int i = 0; i < instance.oldProperties.Count; i++) 
                {
                    ConstGenSettings.ParamsCTRLR oldCTRLR = instance.oldProperties[i];
                    ConstGenSettings.ParamsCTRLR newCTRLR = instance.newProperties[i];

                    // check if the name of the controller has changed or
                    // if any parameters is added or removed
                    if ( oldCTRLR.name != newCTRLR.name || 
                        oldCTRLR.parameters.Count != newCTRLR.parameters.Count ) {

                        generate = true;
                        break;
                    }
                    else // else check if any of the name of paramters has changed
                    {
                        // loop through parameters
                        for (int i2 = 0; i2 < oldCTRLR.parameters.Count; i2++)
                        {
                            string oldName = oldCTRLR.parameters[i2];
                            string newName = newCTRLR.parameters[i2];

                            // compare parameter names
                            if ( oldName != newName ) 
                            {
                                generate = true;
                                break;
                            }
                        }
                    }

                    if ( generate ) // break out of animators loop
                        break;
                }
            }

            if ( generate ) {  
                Generate();
            }
        }

        protected override string GetOutputFileName() {
            return FILENAME;
        }

        protected override List<ConstGenSettings.ParamsCTRLR> RetriveValues()
        {
            // find controller GUIDs and create LayersCTRLR list
            string[] controllers = AssetDatabase.FindAssets("t:animatorcontroller");
            List<ConstGenSettings.ParamsCTRLR> animCTRLRS = new List<ConstGenSettings.ParamsCTRLR>();

            foreach (string CTRLR in controllers)
            {
                // get controller and it's path
                string path = AssetDatabase.GUIDToAssetPath(CTRLR);
                UnityEditor.Animations.AnimatorController item = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(path);
                
                if (item.parameters.Length == 0) continue;

                ConstGenSettings.ParamsCTRLR controller = new ConstGenSettings.ParamsCTRLR();
                controller.name = item.name;

                // loop through controller's parameters and cache it
                foreach (var parameter in item.parameters)
                {
                    controller.parameters.Add( parameter.name );
                }

                animCTRLRS.Add( controller );
            }

            return animCTRLRS;
        }
    }
}