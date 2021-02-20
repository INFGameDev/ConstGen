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
    public class AnimStatesGen : GeneratorBase<AnimStatesGen, ConstGenSettings.StatesCTRLR>
    {
        private const string FILENAME = "_ANIMSTATES";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            CreateGeneratorInsance();

            if ( !RetrieveSettings( ()=> instance.oldProperties = ConstantGenerator.GetSettingsFile()._ANIMSTATES ) )
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
            ConstantGenerator.GetSettingsFile()._ANIMSTATES.Clear();
            ConstantGenerator.GetSettingsFile()._ANIMSTATES = instance.newProperties;

            // set SO to be dirty to be saved
            EditorUtility.SetDirty( ConstantGenerator.GetSettingsFile() );

            instance.GenerateCode(
                content =>
                {
                    WrappedInt indentCount = 2; 

                    // loop through all animators
                    for (int i = 0; i < instance.newProperties.Count; i++)
                    {
                        // cache current animator 
                        ConstGenSettings.StatesCTRLR sttCTRLR = instance.newProperties[i];
                        string ctrlrName = string.Format( ConstantGenerator.ClassFormat, _ConstGen.MakeIdentifier(sttCTRLR.name) );
                        
                        // write animator name header
                        using ( new CurlyBrackets(content, ctrlrName, indentCount) )
                        {
                            // loop through layers
                            for (int i2 = 0; i2 < sttCTRLR.animLayers.Count; i2++)
                            {
                                // cache current layer
                                ConstGenSettings.AnimLayer animLayer = sttCTRLR.animLayers[i2];
                                string layerName = string.Format( ConstantGenerator.ClassFormat+"_L", _ConstGen.MakeIdentifier(animLayer.name) );

                                // write layer group header name
                                using ( new CurlyBrackets( content, layerName, indentCount ) )
                                {
                                    // loop through states
                                    for (int i3 = 0; i3 < animLayer.animStates.Count; i3++)
                                    {
                                        // cache current state
                                        ConstGenSettings.AnimState state_ = animLayer.animStates[i3];
                                        string stateName = string.Format(  ConstantGenerator.ClassFormat+"_S", _ConstGen.MakeIdentifier(state_.name) );
                                        
                                        // write state name group at header
                                        using ( new CurlyBrackets( content, stateName, indentCount ) )
                                        {
                                            // write state name
                                            content.WriteConstant( indentCount, DT.String, "name", state_.name );

                                            // write state tag if present
                                            if ( state_.tag != string.Empty && state_.tag != null ) {
                                                content.WriteConstant( indentCount, DT.String, "tag", state_.tag );                                                
                                            }
                                        }

                                        content.WriteNewLine();
                                    }
                                }

                                content.WriteNewLine();
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

            // check if the number of controllers has changed
            if ( instance.newProperties.Count != instance.oldProperties.Count )
            {
                generate = true;
            }
            else
            {
                // loop through animator controllers
                for (int i = 0; i < instance.oldProperties.Count; i++)
                {
                    List<ConstGenSettings.AnimLayer> oldAnimLayers = instance.oldProperties[i].animLayers;
                    List<ConstGenSettings.AnimLayer> newAnimLayers = instance.newProperties[i].animLayers;

                    // check if the number of layers has changed
                    if ( oldAnimLayers.Count != newAnimLayers.Count ) {
                        generate = true;
                        break;
                    }

                    if ( !generate ) {
                        // loop through all the animator layers
                        for (int i2 = 0; i2 < oldAnimLayers.Count; i2++)
                        {
                            ConstGenSettings.AnimLayer oldLayer = oldAnimLayers[i2];
                            ConstGenSettings.AnimLayer newLayer = newAnimLayers[i2];

                            // compare layer names if it has changed
                            if ( oldLayer.name != newLayer.name ) {
                                generate = true;
                                break;
                            }

                            if ( !generate )  // check the anim states in the layer
                            {
                                List<ConstGenSettings.AnimState> oldStates = oldLayer.animStates;
                                List<ConstGenSettings.AnimState> newStates = newLayer.animStates;                                

                                // check if the number of states has changed
                                if ( oldStates.Count != newStates.Count ) {
                                    generate = true;
                                    break;
                                }

                                if ( !generate ) // loop through all states in that layer
                                { 
                                    for (int i3 = 0; i3 < oldStates.Count; i3++) 
                                    {
                                        ConstGenSettings.AnimState oldState = oldStates[i3];
                                        ConstGenSettings.AnimState newState = newStates[i3];

                                        // compare names if it has changed
                                        if ( oldState.name != newState.name ) {
                                            generate = true;
                                            break;
                                        }

                                        // compare tags if it has changed
                                        if ( oldState.tag != newState.tag )
                                        {
                                            generate = true;
                                            break;
                                        }
                                    }
                                }
                            }

                        if ( generate ) // break out of layers loop
                            break;
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

        protected override List<ConstGenSettings.StatesCTRLR> RetriveValues()
        {
            // find controller GUIDs and create StatesCTRLR list
            string[] controllers = AssetDatabase.FindAssets("t:animatorcontroller");
            List<ConstGenSettings.StatesCTRLR> statesControllers = new List<ConstGenSettings.StatesCTRLR>();

            foreach (string CTRLR in controllers)  
            {
                // get controller and it's path
                string path = AssetDatabase.GUIDToAssetPath(CTRLR);
                UnityEditor.Animations.AnimatorController animCTRLR = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(path);
                
                ConstGenSettings.StatesCTRLR stateCTRLR = new ConstGenSettings.StatesCTRLR();
                stateCTRLR.name = animCTRLR.name;

                if (animCTRLR.layers.Length == 0) continue;

                // loop throug layers
                for (int i = 0; i < animCTRLR.layers.Length; i++)
                {
                    var layer_ = animCTRLR.layers[i]; // get layer
      
                    ConstGenSettings.AnimLayer animLayer = new ConstGenSettings.AnimLayer();
                    animLayer.name = layer_.name; // store layer name

                    if ( layer_.stateMachine.states.Length == 0 )
                        continue;

                    // loop through layer states
                    for (int i2 = 0; i2 < layer_.stateMachine.states.Length; i2++)
                    {
                        var state_ = layer_.stateMachine.states[i2]; // get state
                        
                        ConstGenSettings.AnimState animState = new ConstGenSettings.AnimState();
                        animState.name = state_.state.name; // store state name
                        animState.tag = state_.state.tag; // store tag name

                        // add state to layers
                        animLayer.animStates.Add( animState ); 
                    }     

                    // add layers to controller
                    stateCTRLR.animLayers.Add( animLayer );      
                }

                // add controllers to list of controllers
                statesControllers.Add( stateCTRLR );
            }

            return statesControllers;
        }
    }
}