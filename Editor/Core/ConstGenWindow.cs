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
        private bool showFoldOut = true;
        private Texture logo;
        private Texture border;

        private Color contentColor;
        private Color backgroundColor;

        [MenuItem("Const Gen/Generator")]
        private static void ShowWindow() {
            window = GetWindow<ConstGenWindow>();
            window.titleContent = new GUIContent("Const Generator");
            window.minSize = new Vector2( 300, 425 );
            // window.maxSize = new Vector2( 300, 400 );
            window.Show();
        }

        private void CacheExternalAssets()
        {
            if ( settings == null )
                settings = ConstantGenerator.GetSettingsFile();

            if ( logo == null )
                logo = ConstantGenerator.GetLogo();

            if ( border == null )
                border = ConstantGenerator.GetBorder();
        }

        private void OnGUI() {

            CacheExternalAssets();

            contentColor = Color.black * 5;
            backgroundColor = Color.white * 2.5f;

            EditorGUI.BeginChangeCheck();

            StartGUI( "Layers" );
                if ( DrawGenButton() )
                {
                    window.Close();
                    LayersGen.Generate();
                }

                if ( DrawForceGenButton() )
                {
                    window.Close();   
                    LayersGen.ForceGenerate();           
                }
            EndGUI();
            // -------------------------------------------------------------------------------------
            StartGUI( "Tags" );
                if ( DrawGenButton() )
                {
                    window.Close();
                    TagsGen.Generate();               
                }

                if ( DrawForceGenButton() )
                {
                    window.Close(); 
                    TagsGen.ForceGenerate();                    
                }               
            EndGUI();
            // -------------------------------------------------------------------------------------
            StartGUI( "Sort Layers" );
                if ( DrawGenButton() )
                {
                    window.Close();
                    SortingLayersGen.Generate();      
                }

                if ( DrawForceGenButton() )
                {
                    window.Close();   
                    SortingLayersGen.ForceGenerate();                  
                }                
            EndGUI();
            // -------------------------------------------------------------------------------------
            StartGUI( "Scenes" );
                if ( DrawGenButton() )
                {
                    window.Close();
                    ScenesGen.Generate();                  
                }

                if ( DrawForceGenButton() )
                {
                    window.Close(); 
                    ScenesGen.ForceGenerate();               
                }               
            EndGUI();
            // -------------------------------------------------------------------------------------
            StartGUI( "Shader Props" );
                if ( DrawGenButton() )
                {
                    window.Close();
                    ShaderPropsGen.Generate(false);         
                }

                if ( DrawForceGenButton() )
                {
                    window.Close();
                    ShaderPropsGen.ForceGenerate();                       
                }               
            EndGUI();
            // -------------------------------------------------------------------------------------
            StartGUI( "Anim Params" );
                if ( DrawGenButton() )
                {
                    window.Close();
                    AnimParamsGen.Generate();              
                }

                if ( DrawForceGenButton() )
                {
                    window.Close();
                    AnimParamsGen.ForceGenerate();             
                }               
            EndGUI();
            // -------------------------------------------------------------------------------------
            StartGUI( "Anim Layers" );
                if ( DrawGenButton() )
                {
                    window.Close();
                    AnimLayersGen.Generate();
                    
                }

                if ( DrawForceGenButton() )
                {
                    window.Close();
                    AnimLayersGen.ForceGenerate();
                }               
            EndGUI();
            // -------------------------------------------------------------------------------------
            StartGUI( "Anim States" );
                if ( DrawGenButton() )
                {
                    window.Close();
                    AnimStatesGen.Generate();
                }

                if ( DrawForceGenButton() )
                {
                    window.Close();
                    AnimStatesGen.ForceGenerate(); 
                }                
            EndGUI();
            // -------------------------------------------------------------------------------------
            StartGUI( "Nav Areas" );
                if ( DrawGenButton() )
                {
                    window.Close();
                    NavAreasGen.Generate();
                }

                if ( DrawForceGenButton() )
                {
                    window.Close();
                    NavAreasGen.ForceGenerate(); 
                }                
            EndGUI();
            // =========================================================================================
            if ( EditorGUIUtility.isProSkin ) {
                DrawLine( Color.white, 2, 5 );
            } else {
                DrawLine( Color.gray, 2, 5 );
            }


            GUIStyle style = new GUIStyle( GUI.skin.button );
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;

            if ( EditorGUIUtility.isProSkin ) {
                GUI.contentColor = contentColor;
                GUI.backgroundColor = backgroundColor;

                style.normal.textColor = contentColor;
            } else {
                GUI.contentColor = backgroundColor;
                GUI.backgroundColor = Color.black * 0.75f;

                style.normal.textColor = backgroundColor;
                style.hover.textColor = Color.cyan;
            }

            EGL.BeginHorizontal();
                EGL.BeginVertical();
                        EGL.BeginHorizontal();
                        if ( GL.Button( "GENERATE ALL", style ) )
                        {
                            window.Close();
                            LayersGen.Generate();
                            TagsGen.Generate();
                            SortingLayersGen.Generate();
                            ScenesGen.Generate();
                            ShaderPropsGen.Generate( false );
                            AnimParamsGen.Generate();
                            AnimLayersGen.Generate();
                            AnimStatesGen.Generate();
                            NavAreasGen.Generate();
                        } 
                        GL.FlexibleSpace();
                        EGL.EndHorizontal();

                        EGL.BeginHorizontal();
                        if ( GL.Button( "FORCE GENERATE ALL", style ) )
                        {
                            window.Close();
                            LayersGen.ForceGenerate();
                            TagsGen.ForceGenerate();
                            SortingLayersGen.ForceGenerate();
                            ScenesGen.ForceGenerate();
                            ShaderPropsGen.ForceGenerate();
                            AnimParamsGen.ForceGenerate();
                            AnimLayersGen.ForceGenerate();
                            AnimStatesGen.ForceGenerate();
                            NavAreasGen.ForceGenerate();
                        } 
                        GL.FlexibleSpace();
                        EGL.EndHorizontal();
                EGL.EndVertical();
                EGL.BeginVertical();
            // ---------------------------------------------------------------------------------------
                        Color genOnReloadColor;
                        Color updateOnReloadColor;

                        style.normal.textColor =  Color.black;

                        if ( EditorGUIUtility.isProSkin ) {
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
                        } else {
                            if ( settings.regenerateOnMissing ) {
                                genOnReloadColor =  Color.green * 2;
                            } else {
                                genOnReloadColor =  Color.gray;
                            }

                            if ( settings.updateOnReload ) {
                                updateOnReloadColor =  Color.green * 2;
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
                        EGL.EndHorizontal();

                        EGL.BeginHorizontal();
                            GUI.backgroundColor = updateOnReloadColor;
                            if ( GL.Button( new GUIContent("Update On Reload", "Automatically re-generates the constants on editor recompile if any changes are detected."), style) ) {
                                settings.updateOnReload = !settings.updateOnReload;
                                EditorUtility.SetDirty( settings );
                            } 
                        EGL.EndHorizontal();

                EGL.EndVertical();
            EGL.EndHorizontal();
            // =========================================================================================

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
                    outputFileName = EGL.TextField( "Output File Name" ,outputFileName );

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
                                if ( generatorName == string.Empty || outputFileName == string.Empty || generatorName == null || outputFileName == null  )
                                {
                                    Debug.LogWarning( "Fill out all the fields" );
                                } 
                                else
                                {
                                    window.Close();
                                    TemplateGen.GenerateTemplate( generatorName, outputFileName );
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

        private void StartGUI( string name )
        {
            Rect r =  EGL.BeginVertical( );
            GL.Space(3);
            EGL.BeginHorizontal();

                if ( EditorGUIUtility.isProSkin ) {
                    GUI.backgroundColor = Color.black * 2;
                    GUI.contentColor = Color.white * 2;
                } else {
                    GUI.backgroundColor = Color.white;
                    GUI.contentColor = Color.black * 2;
                }

                GUI.Box( r, GUIContent.none );

                GUIStyle style = new GUIStyle( GUI.skin.label );
                style.fontStyle = FontStyle.Bold;

                if ( EditorGUIUtility.isProSkin ) {
                    style.normal.textColor  = Color.white * 40;
                } else {
                    style.normal.textColor  = Color.black;
                }

                EGL.LabelField(" " + name, style, GL.Width(90) );
        }

        private bool DrawGenButton()
        {
            GUIStyle style = new GUIStyle( GUI.skin.button );

            if ( EditorGUIUtility.isProSkin ) {
                GUI.contentColor = contentColor;
                GUI.backgroundColor = backgroundColor;

                style.normal.textColor = contentColor;
            } else {
                GUI.contentColor = backgroundColor;
                GUI.backgroundColor = Color.black * 0.75f;

                style.normal.textColor = backgroundColor;
                style.hover.textColor = Color.cyan;
            }

            bool clicked = GL.Button( 
                new GUIContent("Generate"), 
                style,
                GL.Height(20) 
            );
            return clicked;
        }

        private bool DrawForceGenButton()
        {
            GUIStyle style = new GUIStyle( GUI.skin.button );

            if ( EditorGUIUtility.isProSkin ) {
                GUI.contentColor = contentColor;
                GUI.backgroundColor = backgroundColor;

                style.normal.textColor = contentColor;
            } else {
                GUI.contentColor = backgroundColor;
                GUI.backgroundColor = Color.black * 0.75f;

                style.normal.textColor = backgroundColor;
                style.hover.textColor = Color.cyan;
            }

            bool clicked = GL.Button( 
                new GUIContent("Force Generate"), 
                style,
                GL.Height(20) 
            );
            return clicked;
        }    

        private void EndGUI()
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