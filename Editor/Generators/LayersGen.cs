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
    public class LayersGen : GeneratorBase<LayersGen, string>
    {
        private const string FILENAME = "_LAYERS";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            CreateGeneratorInsance();

            if ( !RetrieveSettings( ()=> instance.oldProperties = ConstantGenerator.GetSettingsFile()._LAYERS ) )
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
            ConstantGenerator.GetSettingsFile()._LAYERS.Clear();
            ConstantGenerator.GetSettingsFile()._LAYERS = instance.newProperties;

            // set SO to be dirty to be saved
            EditorUtility.SetDirty( ConstantGenerator.GetSettingsFile() );

            instance.GenerateCode(
                content =>
                {
                    WrappedInt indentCount = 2; 

                    foreach (string property in instance.newProperties)
                    {
                        int layerIndex = LayerMask.NameToLayer(_ConstGen.EscapeDoubleQuote(property));
                        content.WriteConstant( indentCount, DT.Int,  _ConstGen.MakeIdentifier(property), layerIndex.ToString() );
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
            return Enumerable.Range(0, 32).Select(x => LayerMask.LayerToName(x)).Where(y => y.Length > 0).ToList();
        }
    }
}
