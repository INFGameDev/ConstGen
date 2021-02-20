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
    public class ScenesGen : GeneratorBase<ScenesGen, string>
    {
        private const string FILENAME = "_SCENES";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            CreateGeneratorInsance();

            if ( !RetrieveSettings( ()=> instance.oldProperties = ConstantGenerator.GetSettingsFile()._SCENES ) )
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
            ConstantGenerator.GetSettingsFile()._SCENES.Clear();
            ConstantGenerator.GetSettingsFile()._SCENES = instance.newProperties;

            // set SO to be dirty to be saved
            EditorUtility.SetDirty( ConstantGenerator.GetSettingsFile() );

            instance.GenerateCode(
                content =>
                {
                    WrappedInt indentCount = 2; 

                    int z = 0;
                    foreach (string property in instance.newProperties)
                    {
                        // ex) assets/scenes/menu.unity -> menu 
                        // var tail = property.Substring(property.LastIndexOf('/') + 1);
                        // var result = tail.Substring(0, tail.LastIndexOf('.'));

                        content.WriteConstant( indentCount, DT.Int, _ConstGen.MakeIdentifier(property), z.ToString() );
                        z++;
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

        protected override string GetOutputFileName()
        {
            return FILENAME;
        }

        protected override List<string> RetriveValues()
        {
            return EditorBuildSettings.scenes.Select(x => x.path).ToList();
        }
    }
}