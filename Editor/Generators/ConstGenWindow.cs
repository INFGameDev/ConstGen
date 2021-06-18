using UnityEngine;
using UnityEditor;

// use alias directives so we can access easier through shorther names
using EGL = UnityEditor.EditorGUILayout;
using GL = UnityEngine.GUILayout;

namespace ConstGen
{
    public class ConstGenWindow : EditorWindow {

        private static ConstGenSettings settings;
        private static ConstGenWindow window;
        private string generatorName;
        private string outputFileName;
        private DT outputType;


        private string outputFilePath_;
        private string outputFilePath { 
            get { 

                string s = string.Empty;

                if ( !string.IsNullOrEmpty(outputFilePath_) )
                    s = outputFilePath_.Replace( Application.dataPath, string.Empty );
                
                if ( !string.IsNullOrEmpty(s) )
                    s = s.Remove(0,1);

                return s;
            } 
            
            set { outputFilePath_ = value; } 
        }

        private bool showFoldOut = true;
        private Texture logo;
        private Texture border;

        private Color buttonColor = Color.white * 2.5f;
        private Color ButtonLabelColor = Color.black * 5;
        private Color buttonLightModeBackgroundColor = Color.black * 0.75f;
        private Color buttonLightModeHoverColor = Color.cyan;


        private Color buttonBackgroundColor = Color.black * 2;
        private Color buttonBackgroundLabelColor = Color.white * 2;



        [MenuItem("Const Gen/Generator")]
        private static void ShowWindow() {
            window = GetWindow<ConstGenWindow>();
            window.titleContent = new GUIContent("Const Generator");
            window.minSize = new Vector2( 300, 555 );
            // window.maxSize = new Vector2( 300, 400 ); 
            window.Show();
        }

        private void CacheGlobalVars()
        {
            if ( settings == null )
                settings = ConstantGenerator.GetSettingsFile();

            if ( logo == null )
                logo = ConstantGenerator.GetLogo();

            if ( border == null )
                border = ConstantGenerator.GetBorder();

            if ( window == null )
                window = GetWindow<ConstGenWindow>(); 
        }

        private void OnGUI() 
        {

            CacheGlobalVars();

            EditorGUI.BeginChangeCheck();

            DrawGenerationButtons();
            DrawSettings();

            if ( EditorGUIUtility.isProSkin ) {
                DrawLine( Color.white, 2, 5 );
            } else {
                DrawLine( Color.gray, 2, 5 );
            }

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white * 5;

// check for unity versions using conditional directives
// NOTE: there is no "BeginFoldoutHeaderGroup" in below 2019.1
 #if UNITY_2019_OR_NEWER
    showFoldOut = EGL.BeginFoldoutHeaderGroup( showFoldOut, "Create Generator Script" );
#else
    showFoldOut = EGL.Foldout( showFoldOut, "Create Generator Script" );
 #endif
                if ( showFoldOut )
                {
                    GL.Space(5);

                    if ( EditorGUIUtility.isProSkin ) {
                        GUI.contentColor = Color.white * 10;
                    } else {
                        GUI.contentColor = Color.black;
                    }

                    generatorName = EGL.TextField( "Generator Name" , generatorName );
                    outputFileName = EGL.TextField( "Output File Name" , outputFileName );
                    outputType = (DT)EGL.EnumPopup( "Output Type", outputType );

                    EGL.BeginHorizontal();
                    EGL.LabelField( "Output Path" );
                        if ( GL.Button( "Choose Path" ) )
                        {
                            outputFilePath = EditorUtility.OpenFolderPanel( "Choose Generator Ouput File Path", "Assets", "" );
                        }
                    EGL.EndHorizontal();

                    Rect pathBox = EGL.BeginHorizontal();
                        GUI.Box( pathBox, GUIContent.none );
                        EGL.LabelField( "[ " + outputFilePath + " ]" );
                    EGL.EndHorizontal();

                    GL.Space(5);
                    EGL.BeginHorizontal();

                    if ( !settings.regenerateOnMissing )
                    {
                        EGL.BeginVertical();
                            GL.FlexibleSpace();
                            
                            GUI.backgroundColor = Color.white;
                            GUI.contentColor = Color.white;
                            EGL.HelpBox("NOTE: Force Generate will only delete the file but will NOT generate a new one if the [ReGen On Missing] is turned off", 
                                MessageType.Warning);
                                
                        EGL.EndVertical();
                    } 
                    else 
                    {   // ============================================================================
                        // Draw Ma Awesome Logo
                        EGL.BeginVertical();
                            GL.FlexibleSpace();
                            Rect horiRect = EGL.BeginHorizontal();
                            
                                Rect boxRect = new Rect( horiRect.x+3, horiRect.y-54, 125, 52 );

                                Rect backgroundRect = boxRect;
                                backgroundRect.width = border.width;
                                backgroundRect.height = border.height;
                                GUI.DrawTexture( backgroundRect, border );
                                // GUI.Box( boxRect, iconBackground, );

                                // GUI.Label( new Rect( boxRect.x+3, boxRect.y+30, 100, 20 ), "    (╯°□°）╯" );
                                // GUI.Label( new Rect( boxRect.x+3, boxRect.y+18, 100, 20 ), "_____________" );
                                // GUI.Label( new Rect( boxRect.x+3, boxRect.y+16, 100, 20 ), "Created BY: " );

                                GUI.Label( new Rect( boxRect.x+3, boxRect.y+16, 100, 20 ), "Created BY:" );
                                GUI.Label( new Rect( boxRect.x+3, boxRect.y+30, 100, 20 ), "     (╯°□°）╯︵" );

                                // GUI.Label( new Rect( boxRect.x+3, boxRect.y, 100, 20 ), "Created BY: " );
                                // GUI.Label( new Rect( boxRect.x+3, boxRect.y+30, 100, 20 ), "    (╯°□°）╯︵" );

                                Rect logoRect = new Rect( boxRect.x+76, boxRect.y+2, logo.width, logo.height );
                                GUI.DrawTexture( logoRect, logo);

                            EGL.EndHorizontal();
                        EGL.EndVertical();
                        // ============================================================================
                    }

                    GL.FlexibleSpace();

                    GUI.contentColor = Color.white * 5;
                        EGL.BeginVertical();
                            GL.FlexibleSpace();

                            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
                            buttonStyle.fontStyle = FontStyle.Bold;

                            if ( EditorGUIUtility.isProSkin ) {
                                GUI.backgroundColor = Color.white * 2.5f;
                                GUI.contentColor = Color.black * 5;
                            } else {
                                GUI.backgroundColor = Color.black * 0.75f;
                                GUI.contentColor = Color.white * 2.5f;
                                
                                buttonStyle.hover.textColor = Color.cyan;
                                buttonStyle.normal.textColor = Color.white * 2.5f;
                            }

                            if ( GL.Button( "Create", buttonStyle, GL.Width(75), GL.Height(30) ) )
                            {
                                if ( string.IsNullOrEmpty( generatorName ) || string.IsNullOrEmpty( outputFileName )  )
                                {
                                    Debug.LogWarning( "Fill out all the fields" );
                                } 
                                else
                                {
                                    TriggerCloseWindow(); 
                                    CustomGeneratorGen.CreateGenerator( generatorName, outputFileName, outputType, outputFilePath );
                                }
                            }
                        EGL.EndVertical();
                    GL.Space(1);
                    EGL.EndHorizontal();
                }

 #if UNITY_2019_OR_NEWER
    EGL.EndFoldoutHeaderGroup();
 #endif
 
            if ( EditorGUI.EndChangeCheck() )
            {
                EditorUtility.SetDirty( settings );
            }
        }

        private void DrawGenerationButtons()
        {
            DrawButtonBackground( "Layers" );
                if ( DrawGenButton() )
                {
                    TriggerCloseWindow();
                    LayersGen.Generate();
                }

                if ( DrawForceGenButton() )
                {
                    TriggerCloseWindow();   
                    LayersGen.ForceGenerate();           
                }
            EndDrawButtonBackground();
            // -------------------------------------------------------------------------------------
            DrawButtonBackground( "Tags" );
                if ( DrawGenButton() )
                {
                    TriggerCloseWindow();
                    TagsGen.Generate();               
                }

                if ( DrawForceGenButton() )
                {
                    TriggerCloseWindow(); 
                    TagsGen.ForceGenerate();                    
                }               
            EndDrawButtonBackground();
            // -------------------------------------------------------------------------------------
            DrawButtonBackground( "Sort Layers" );
                if ( DrawGenButton() )
                {
                    TriggerCloseWindow();
                    SortingLayersGen.Generate();
                }

                if ( DrawForceGenButton() )
                {
                    TriggerCloseWindow();   
                    SortingLayersGen.ForceGenerate();                  
                }                
            EndDrawButtonBackground();
            // -------------------------------------------------------------------------------------
            DrawButtonBackground( "Scenes" );
                if ( DrawGenButton() )
                {
                    TriggerCloseWindow();
                    ScenesGen.Generate();                  
                }

                if ( DrawForceGenButton() )
                {
                    TriggerCloseWindow(); 
                    ScenesGen.ForceGenerate();               
                }               
            EndDrawButtonBackground();
            // -------------------------------------------------------------------------------------
            DrawButtonBackground( "Nav Areas" );
                if ( DrawGenButton() )
                {
                    TriggerCloseWindow();
                    NavAreasGen.Generate();
                }

                if ( DrawForceGenButton() )
                {
                    TriggerCloseWindow();
                    NavAreasGen.ForceGenerate(); 
                }                
            EndDrawButtonBackground();
            // -------------------------------------------------------------------------------------
            DrawButtonBackground( "Input Axes" );
                if ( DrawGenButton() )
                {
                    TriggerCloseWindow();
                    InputAxesGen.Generate();
                }

                if ( DrawForceGenButton() )
                {
                    TriggerCloseWindow();
                    InputAxesGen.ForceGenerate();
                }                
            EndDrawButtonBackground();
            // -------------------------------------------------------------------------------------
            DrawButtonBackground( "Shader Props" );
                if ( DrawGenButton() )
                {
                    TriggerCloseWindow();
                    ShaderPropsGen.Generate(false);         
                }

                if ( DrawForceGenButton() )
                {
                    TriggerCloseWindow();
                    ShaderPropsGen.ForceGenerate();                       
                }               
            EndDrawButtonBackground();
            // -------------------------------------------------------------------------------------
            DrawButtonBackground( "Anim Params" );
                if ( DrawGenButton() )
                {
                    TriggerCloseWindow();
                    AnimParamsGen.Generate();              
                }

                if ( DrawForceGenButton() )
                {
                    TriggerCloseWindow();
                    AnimParamsGen.ForceGenerate();             
                }               
            EndDrawButtonBackground();
            // -------------------------------------------------------------------------------------
            DrawButtonBackground( "Anim Layers" );
                if ( DrawGenButton() )
                {
                    TriggerCloseWindow();
                    AnimLayersGen.Generate();
                    
                }

                if ( DrawForceGenButton() )
                {
                    TriggerCloseWindow();
                    AnimLayersGen.ForceGenerate();
                }               
            EndDrawButtonBackground();
            // -------------------------------------------------------------------------------------
            DrawButtonBackground( "Anim States" );
                if ( DrawGenButton() )
                {
                    TriggerCloseWindow();
                    AnimStatesGen.Generate();
                }

                if ( DrawForceGenButton() )
                {
                    TriggerCloseWindow();
                    AnimStatesGen.ForceGenerate(); 
                }                
            EndDrawButtonBackground();
            // -------------------------------------------------------------------------------------
            DrawButtonBackground( "ALL", true );
                if ( DrawGenButton(true) )
                {
                    TriggerCloseWindow();
                    LayersGen.Generate();
                    TagsGen.Generate();
                    SortingLayersGen.Generate();
                    ScenesGen.Generate();
                    ShaderPropsGen.Generate( false );
                    AnimParamsGen.Generate();
                    AnimLayersGen.Generate();
                    AnimStatesGen.Generate();
                    NavAreasGen.Generate();
                    InputAxesGen.Generate();
                }

                if ( DrawForceGenButton(true) )
                {
                    TriggerCloseWindow();
                    ConstantGenerator.ForceGenerateALL();
                    // LayersGen.ForceGenerate();
                    // TagsGen.ForceGenerate();
                    // SortingLayersGen.ForceGenerate();
                    // ScenesGen.ForceGenerate();
                    // ShaderPropsGen.ForceGenerate();
                    // AnimParamsGen.ForceGenerate();
                    // AnimLayersGen.ForceGenerate();
                    // AnimStatesGen.ForceGenerate();
                    // NavAreasGen.ForceGenerate();
                    // InputAxesGen.ForceGenerate();
                }                
            EndDrawButtonBackground();
        }

        private void DrawSettings()
        {
            GUI.contentColor = Color.white;

            GUIStyle enumLabel = new GUIStyle( GUI.skin.label );
            enumLabel.alignment = TextAnchor.MiddleLeft;
            enumLabel.fontStyle = FontStyle.Bold;
            
            GUIStyle enumField = new GUIStyle( GUI.skin.box );
            enumField.alignment = TextAnchor.MiddleCenter;
            enumField.fontStyle = FontStyle.Bold;

            enumField.normal.textColor = ButtonLabelColor;
            enumField.hover.textColor = ButtonLabelColor;
            enumField.active.textColor = ButtonLabelColor;
            enumField.focused.textColor = ButtonLabelColor;

            if ( EditorGUIUtility.isProSkin ) {
                DrawLine( Color.white, 2, 5 );
                enumLabel.normal.textColor = buttonColor;
                GUI.backgroundColor = Color.white * 5;
            } else {
                DrawLine( Color.gray, 2, 5 );
                enumLabel.normal.textColor = ButtonLabelColor;
                GUI.backgroundColor = Color.gray*1.5f;
            }

            EGL.BeginHorizontal();
                EGL.LabelField("Close On Generate:", enumLabel, GL.Width( 145 ));
                settings.closeOnGenerate = EGL.Toggle( settings.closeOnGenerate );
            EGL.EndHorizontal();

            EGL.BeginHorizontal();
                EGL.LabelField("Identifier Format:", enumLabel, GL.Width( 145 ));

                settings.identifierFormat = 
                    (ConstGenSettings.IdentifierFormat)EGL.EnumPopup( settings.identifierFormat, enumField );
            EGL.EndHorizontal();

            GUIStyle style = new GUIStyle( GUI.skin.button );
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;

            GL.Space(3);

            EGL.BeginHorizontal();
                EGL.BeginVertical();
            // ---------------------------------------------------------------------------------------
                        Color genOnReloadColor;
                        Color updateOnReloadColor;

                        style.normal.textColor =  Color.black;

                        if ( EditorGUIUtility.isProSkin ) {

                            GUI.contentColor = ButtonLabelColor;
                            GUI.backgroundColor = buttonColor;

                            style.normal.textColor = ButtonLabelColor;

                            if ( settings.regenerateOnMissing ) {
                                genOnReloadColor =  Color.green * 2;
                            } else {
                                genOnReloadColor =  Color.white;
                            }

                            if ( settings.updateOnReload ) {
                                updateOnReloadColor =  Color.green * 2;
                            } else {
                                updateOnReloadColor =  Color.white;
                            }
                        } 
                        else 
                        {
                            GUI.contentColor = buttonColor;
                            GUI.backgroundColor = buttonLightModeBackgroundColor;

                            style.normal.textColor = ButtonLabelColor;
                            style.hover.textColor = Color.grey;

                            if ( settings.regenerateOnMissing ) {
                                genOnReloadColor =  Color.green;
                            } else {
                                genOnReloadColor =  Color.gray;
                            }

                            if ( settings.updateOnReload ) {
                                updateOnReloadColor =  Color.green;
                            } else {
                                updateOnReloadColor =  Color.gray;
                            }
                        }

                        EGL.BeginHorizontal();

                            GUI.backgroundColor = genOnReloadColor;
                            if ( GL.Button( new GUIContent("ReGen On Missing", "Automatically re-generates the constants file is none is present."), style ) ) {
                                settings.regenerateOnMissing = !settings.regenerateOnMissing;
                                EditorUtility.SetDirty( settings );
                            } 

                            GUI.backgroundColor = updateOnReloadColor;
                            if ( GL.Button( new GUIContent("Update On Reload", "Automatically re-generates the constants on editor recompile if any changes are detected."), style) ) {
                                settings.updateOnReload = !settings.updateOnReload;
                                EditorUtility.SetDirty( settings );
                            } 
                        EGL.EndHorizontal();
                EGL.EndVertical();
            EGL.EndHorizontal();            
        }

        private void DrawButtonBackground( string name, bool generateAllButton = false )
        {
            Rect r =  EGL.BeginVertical( );
            GL.Space(3);
            EGL.BeginHorizontal();

                GUIStyle style = new GUIStyle( GUI.skin.label );
                style.fontStyle = FontStyle.Bold;

                if ( EditorGUIUtility.isProSkin ) {

                    if ( generateAllButton ) {
                        GUI.backgroundColor = Color.black;
                    } else {
                        GUI.backgroundColor = buttonBackgroundColor;
                    }

                    style.normal.textColor  = Color.white * 40;
                    GUI.contentColor = buttonBackgroundLabelColor;
                } else {

                    if ( generateAllButton ) {
                        GUI.backgroundColor = Color.gray;
                    } else {
                        GUI.backgroundColor = Color.white;
                    }

                    style.normal.textColor  = Color.black;
                    GUI.contentColor = buttonBackgroundColor;
                }

                GUI.Box( r, GUIContent.none );
                EGL.LabelField(" " + name, style, GL.Width(90) );
        }

        private bool DrawGenButton( bool isGenerateAll = false )
        {
            GUIStyle style = new GUIStyle( GUI.skin.button );

            if ( EditorGUIUtility.isProSkin ) {
                GUI.contentColor = ButtonLabelColor;
                GUI.backgroundColor = buttonColor;

                style.normal.textColor = ButtonLabelColor;
            } else {
                GUI.contentColor = buttonColor;
                GUI.backgroundColor = buttonLightModeBackgroundColor;

                style.normal.textColor = buttonColor;
                style.hover.textColor = buttonLightModeHoverColor;
            }

            string text = "Generate";

            if ( isGenerateAll ) {
                style.fontStyle = FontStyle.Bold;
                text = text.ToUpper();
            }

            bool clicked = GL.Button( 
                new GUIContent(text), 
                style,
                GL.Height(20) 
            );

            return clicked;
        }

        private bool DrawForceGenButton(  bool isGenerateAll = false )
        {
            string text = "Force Generate";
            GUIStyle style = new GUIStyle( GUI.skin.button );

            if ( EditorGUIUtility.isProSkin ) {
                GUI.contentColor = ButtonLabelColor;
                GUI.backgroundColor = buttonColor;

                style.normal.textColor = ButtonLabelColor;
            } else {
                GUI.contentColor = buttonColor;
                GUI.backgroundColor = buttonLightModeBackgroundColor;

                style.normal.textColor = buttonColor;
                style.hover.textColor = buttonLightModeHoverColor;
            }

            if ( isGenerateAll ) {
                style.fontStyle = FontStyle.Bold;
                text = text.ToUpper();
            }

            bool clicked = GL.Button( 
                new GUIContent(text), 
                style,
                GL.Height(20) 
            );

            return clicked;
        }    

        private void EndDrawButtonBackground()
        {
                GL.Space(5);
            EGL.EndHorizontal();    
            GL.Space(2);
            EGL.EndVertical();
        }   

        private void DrawLine( Color color, int thickness = 2, int padding = 10 )
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            r.height = thickness;
            r.y+=padding/2;
            r.x-=2;
            r.width +=6;
            EditorGUI.DrawRect(r, color);        
        }

        private void TriggerCloseWindow()
        {
            if ( settings.closeOnGenerate )
                window.Close();
        }

        private bool Is2018()
        {
            string v = Application.unityVersion;
            v = v.Remove( v.IndexOf('.') , v.Length - v.IndexOf('.') );

            if ( v == "2018" ) {
                return true;
            }

            return false;
        } 

        private void SetColor( Color content, Color background )
        {
            if ( EditorGUIUtility.isProSkin ) {
                GUI.contentColor = content;
                GUI.backgroundColor = background;
            } else {
                GUI.contentColor = background;
                GUI.backgroundColor = content;
            }
        }

        private void SetColorContent( Color dark, Color light )
        {
            if ( EditorGUIUtility.isProSkin ) {
                GUI.contentColor = dark;
            } else {
                GUI.contentColor = light;
            }
        }

        private void SetColorBackground( Color dark, Color light )
        {
            if ( EditorGUIUtility.isProSkin ) {
                GUI.backgroundColor = dark;
            } else {
                GUI.backgroundColor = light;
            }
        }
    }   
}