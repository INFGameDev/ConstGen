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
    public class TagsGen : GeneratorBase<TagsGen, string>
    {
        private const string FILENAME = "_TAGS";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            CreateGeneratorInstance();

            if ( !RetrieveSettings( ()=> instance.oldProperties = ConstantGenerator.GetSettingsFile()._TAGS ) )
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
            ConstantGenerator.GetSettingsFile()._TAGS.Clear();
            ConstantGenerator.GetSettingsFile()._TAGS = instance.newProperties;

            // set SO to be dirty to be saved
            EditorUtility.SetDirty( ConstantGenerator.GetSettingsFile() );

            instance.GenerateCode(
                content =>
                {
                    WrappedInt indentCount = 2; 

                    foreach (string property in instance.newProperties)
                    {
                        content.WriteConstant( indentCount, 
                            DT.String, _ConstGen.CreateIdentifier(property), 
                            _ConstGen.EscapeDoubleQuote(property) 
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
                Generate(); ;
        }

        protected override string GetOutputFileName()
        {
           return FILENAME;
        }

        protected override List<string> RetriveValues()
        {
            return InternalEditorUtility.tags.ToList();
        }
    }
}