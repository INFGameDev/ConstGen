using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.AI;

namespace ConstGen
{
    public class NavAreasGen : GeneratorBase<NavAreasGen, string>
    {
        private const string FILENAME = "_NAVAREAS"; 

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            CreateGeneratorInsance();

            if ( !RetrieveSettings( ()=> instance.oldProperties = ConstantGenerator.GetSettingsFile()._NAVAREAS ) )
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
            ConstantGenerator.GetSettingsFile()._NAVAREAS.Clear();
            ConstantGenerator.GetSettingsFile()._NAVAREAS = instance.newProperties;

            // set SO to be dirty to be saved
            EditorUtility.SetDirty( ConstantGenerator.GetSettingsFile() );

            instance.GenerateCode(
                content =>
                {
                    WrappedInt indentCount = 2; 

                    foreach (string property in instance.newProperties)
                    {
                        content.WriteConstant( indentCount, 
                            DT.Int, _ConstGen.MakeIdentifier(property), 
                            NavMesh.GetAreaFromName( _ConstGen.EscapeDoubleQuote(property) ).ToString() );                                
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
            return GameObjectUtility.GetNavMeshAreaNames().ToList();
        }
    }
}


