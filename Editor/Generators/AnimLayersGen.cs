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
    public class AnimLayersGen : GeneratorBase<AnimLayersGen, ConstGenSettings.LayersCTRLR>
    {
        private const string FILENAME = "_ANIMLAYERS";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            CreateGeneratorInsance();

            if ( !RetrieveSettings( ()=> instance.oldProperties = ConstantGenerator.GetSettingsFile()._ANIMLAYERS ) )
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
            ConstantGenerator.GetSettingsFile()._ANIMLAYERS.Clear();
            ConstantGenerator.GetSettingsFile()._ANIMLAYERS = instance.newProperties;

            // set SO to be dirty to be saved
            EditorUtility.SetDirty( ConstantGenerator.GetSettingsFile() );

            instance.GenerateCode(
                content =>
                {
                    WrappedInt indentCount = 2; 

                    foreach (ConstGenSettings.LayersCTRLR ctlr in instance.newProperties)
                    {
                        // write layers animator header group
                        string animatorName = string.Format( ConstantGenerator.ClassFormat, _ConstGen.MakeIdentifier(ctlr.name) );
                        using (new CurlyBrackets(content, animatorName ,indentCount))
                        {
                            int layerIndex = 0;
                            foreach (string layer in ctlr.layers )
                            {
                                content.WriteConstant( 
                                    indentCount, DT.Int, 
                                    _ConstGen.MakeIdentifier(layer), 
                                    layerIndex.ToString() 
                                );
                                
                                layerIndex++;
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
            else // else check for changes in the layers of the controllers
            { 
                // loop through animators
                for (int i = 0; i < instance.oldProperties.Count && generate == false; i++) {

                    ConstGenSettings.LayersCTRLR oldCTRLR = instance.oldProperties[i];
                    ConstGenSettings.LayersCTRLR newCTRLR = instance.newProperties[i];

                    // check if the name of the controller has changed or
                    // if any layers is added or removed
                    if ( oldCTRLR.name != newCTRLR.name || 
                        oldCTRLR.layers.Count != newCTRLR.layers.Count ) {

                        generate = true;
                        break;
                    }
                    else // else check if any of the name of layers has changed
                    {
                        // loop through layers
                        for (int i2 = 0; i2 < oldCTRLR.layers.Count; i2++)
                        {
                            string oldName = oldCTRLR.layers[i2];
                            string newName = newCTRLR.layers[i2];

                            // compare layer names
                            if ( oldName != newName ) { 
                                generate = true;
                                break;
                            }
                        }
                    }

                    if ( generate ) // break out animators loop
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

        protected override List<ConstGenSettings.LayersCTRLR> RetriveValues()
        {
            // find controller GUIDs and create LayersCTRLR list
            string[] controllers = AssetDatabase.FindAssets("t:animatorcontroller");
            List<ConstGenSettings.LayersCTRLR> layersControllers = new List<ConstGenSettings.LayersCTRLR>();

            foreach (string CTRLR in controllers)  
            {
                // get controller and it's path
                string path = AssetDatabase.GUIDToAssetPath(CTRLR);
                UnityEditor.Animations.AnimatorController animCTRLR = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(path);
                
                if (animCTRLR.layers.Length == 0) continue;

                ConstGenSettings.LayersCTRLR layerCTRLR = new ConstGenSettings.LayersCTRLR();
                layerCTRLR.name = animCTRLR.name;

                // loop through controller's layers and cache it
                foreach (var layer_ in animCTRLR.layers)
                {
                    layerCTRLR.layers.Add( layer_.name );
                }

                layersControllers.Add( layerCTRLR );
            }

            return layersControllers;
        }
    }
}