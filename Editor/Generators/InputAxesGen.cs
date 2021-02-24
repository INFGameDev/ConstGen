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
    public class InputAxesGen : GeneratorBase<InputAxesGen, string>
    {
        private const string FILENAME = "_INPUTAXES";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            CreateGeneratorInstance();

            if ( !RetrieveSettings( ()=> instance.oldProperties = ConstantGenerator.GetSettingsFile()._INPUTAXES ) )
                return; 

            CheckForRegenOrUpdate( ()=> Generate() );
        }

        /// <summary>
        /// Generates the file by writing new updated contents or generates the file is none is present
        /// </summary>
        public static void Generate()
        {
            CreateGeneratorInstance();
            instance.newProperties = instance.RetriveValues();

            // store the new properties to SO
            ConstantGenerator.GetSettingsFile()._INPUTAXES.Clear();
            ConstantGenerator.GetSettingsFile()._INPUTAXES = instance.newProperties;

            // set SO to be dirty to be saved
            EditorUtility.SetDirty( ConstantGenerator.GetSettingsFile() );

            instance.GenerateCode(
                content =>
                {
                    WrappedInt indentCount = 2; 

                    foreach (string property in instance.newProperties)
                    {
                        content.WriteConstant( 
                            indentCount, 
                            DT.String, _ConstGen.CreateIdentifier( property ), 
                            _ConstGen.EscapeDoubleQuote( property ) 
                        );
                    }
                }
            );
        }

        protected override void UpdateFile()
        {
            if (Application.isPlaying) return;

            instance.newProperties = RetriveValues();

            List<string> differences = instance.newProperties.Except( instance.oldProperties ).ToList();

            if (differences.Count > 0 || instance.newProperties.Count != instance.oldProperties.Count)
                Generate();
        }

        protected override string GetOutputFileName() {
            return FILENAME;
        }

        protected override List<string> RetriveValues()
        {
			List<string> AxisInputs = new List<string>();
			SerializedObject InputManagerSO = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
			SerializedProperty axesProperty = InputManagerSO.FindProperty("m_Axes");

			axesProperty.Next(true);
			axesProperty.Next(true);

			while ( axesProperty.Next(false) )
            {
				SerializedProperty axis = axesProperty.Copy();
				axis.Next(true);

				if ( !AxisInputs.Contains(axis.stringValue) )
					AxisInputs.Add(axis.stringValue);
			}

			return AxisInputs;
        }
    }    
}


