#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TGFramework
{
    public class TutorialWindow : EditorWindow
    {
        private TutorialData _tutorialData;
        private Vector2 _scrollPosition;
        private Vector2 _codeScrollPosition;
        private int _selectedModuleIndex = 0;
        private int _selectedCodeExampleIndex = -1;
        private bool _showOverview = true;
        private bool _showQuickStart = false;
        private bool _showCodeExamples = false;
        private bool _showFeatures = false;
        private bool _showRelatedModules = false;
        
        private GUIStyle _titleStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _codeStyle;
        private GUIStyle _boxStyle;
        private bool _stylesInitialized = false;
        
        [MenuItem("TGFramework/Framework/View Documentation")]
        public static void ShowWindow()
        {
            var window = GetWindow<TutorialWindow>("TGFramework - Documentation");
            window.minSize = new Vector2(800, 600);
            window.LoadTutorialData();
        }
        
        private void LoadTutorialData()
        {
            var guids = AssetDatabase.FindAssets("t:TutorialData");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _tutorialData = AssetDatabase.LoadAssetAtPath<TutorialData>(path);
            }
            
            if (_tutorialData == null)
            {
                Debug.LogWarning("TutorialData not found. Creating a new one...");
                CreateDefaultTutorialData();
            }
        }
        
        private void CreateDefaultTutorialData()
        {
            _tutorialData = CreateInstance<TutorialData>();
            _tutorialData.frameworkName = "TGFramework";
            _tutorialData.version = "0.0.1";
            _tutorialData.description = "TGFramework Tutorial";
            
            // ServiceLocator Module
            var serviceLocatorModule = new TutorialModule
            {
                moduleName = "ServiceLocator",
                description = "Sistema de registro y obtención de servicios globales",
                overview = "El ServiceLocator es un patrón de diseño que permite registrar y obtener servicios de forma global sin necesidad de pasar referencias manualmente. Es útil para sistemas como AudioSystem, GameManager, etc.\n\n" +
                          "Características principales:\n" +
                          "• Registro simple de servicios\n" +
                          "• Obtención type-safe\n" +
                          "• Método TryGet para servicios opcionales",
                quickStart = "1. Registra tu servicio:\n   ServiceLocator.Register<MyService>(myServiceInstance);\n\n" +
                           "2. Obtén el servicio desde cualquier lugar:\n   var service = ServiceLocator.Get<MyService>();\n\n" +
                           "3. Para servicios opcionales:\n   var service = ServiceLocator.TryGet<MyService>();",
                codeExamples = new List<TutorialModule.CodeExample>
                {
                    new TutorialModule.CodeExample
                    {
                        title = "Registro Básico",
                        code = "// En el Awake o Start de tu MonoBehaviour\npublic class GameManager : MonoBehaviour\n{\n    private void Awake()\n    {\n        ServiceLocator.Register<GameManager>(this);\n    }\n}",
                        explanation = "Registra el GameManager como servicio global al iniciar."
                    },
                    new TutorialModule.CodeExample
                    {
                        title = "Obtención de Servicio",
                        code = "// Desde cualquier script\npublic class PlayerController : MonoBehaviour\n{\n    private void Start()\n    {\n        var gameManager = ServiceLocator.Get<GameManager>();\n        gameManager.DoSomething();\n    }\n}",
                        explanation = "Obtiene el GameManager registrado y lo usa."
                    },
                    new TutorialModule.CodeExample
                    {
                        title = "Servicio Opcional",
                        code = "// Usa TryGet para servicios que pueden no existir\nvar audioSystem = ServiceLocator.TryGet<AudioSystem>();\nif (audioSystem != null)\n{\n    audioSystem.PlayMusic(\"background\");\n}",
                        explanation = "TryGet retorna null si el servicio no está registrado, evitando excepciones."
                    }
                },
                features = new List<string>
                {
                    "Registro simple con Register<T>()",
                    "Obtención type-safe con Get<T>()",
                    "Método TryGet<T>() para servicios opcionales",
                    "No requiere referencias manuales",
                    "Útil para sistemas singleton"
                },
                relatedModules = new List<string> { "DependencyInjector", "AudioSystem" }
            };
            
            // AudioSystem Module
            var audioSystemModule = new TutorialModule
            {
                moduleName = "AudioSystem",
                description = "Sistema completo de gestión de audio con música y efectos de sonido",
                overview = "AudioSystem es un sistema robusto para manejar música y efectos de sonido en tu juego. Incluye características avanzadas como:\n\n" +
                          "• Pooling de AudioSources para SFX\n" +
                          "• Fade in/out para música\n" +
                          "• Sistema de base de datos de audio con ScriptableObject\n" +
                          "• Volúmenes separados para música y SFX\n" +
                          "• Persistencia de volúmenes con PlayerPrefs",
                quickStart = "1. Crea un AudioClipDataModel (ScriptableObject)\n2. Agrega tus clips de audio con keys únicas\n3. Usa AudioSystem.PlayMusic(\"key\") para música\n4. Usa AudioSystem.PlaySFX(\"key\") para efectos",
                codeExamples = new List<TutorialModule.CodeExample>
                {
                    new TutorialModule.CodeExample
                    {
                        title = "Reproducir Música",
                        code = "// Reproducir música sin fade\nAudioSystem.PlayMusic(\"background_music\");\n\n// Reproducir música con fade in\nAudioSystem.PlayMusic(\"background_music\", fadeDuration: 2f);",
                        explanation = "Reproduce música de fondo. El fadeDuration es opcional."
                    },
                    new TutorialModule.CodeExample
                    {
                        title = "Reproducir Efectos de Sonido",
                        code = "// Reproducir un SFX\nAudioSystem.PlaySFX(\"jump\");\n\n// Reproducir un SFX aleatorio\nAudioSystem.PlayRandomSFX(\"jump1\", \"jump2\", \"jump3\");\n\n// Reproducir SFX secuencialmente\nAudioSystem.PlaySequentialSFX(\"step1\", \"step2\", \"step3\");",
                        explanation = "Múltiples formas de reproducir efectos de sonido."
                    },
                    new TutorialModule.CodeExample
                    {
                        title = "Control de Volumen",
                        code = "// Ajustar volumen de música\nAudioSystem.SetMusicVolume(0.5f);\n\n// Ajustar volumen de SFX\nAudioSystem.SetSFXVolume(0.7f);\n\n// Obtener volúmenes actuales\nfloat musicVol = AudioSystem.Instance.MusicVolume;\nfloat sfxVol = AudioSystem.Instance.SfxVolume;",
                        explanation = "Los volúmenes se guardan automáticamente en PlayerPrefs."
                    },
                    new TutorialModule.CodeExample
                    {
                        title = "Detener Música",
                        code = "// Detener inmediatamente\nAudioSystem.StopMusic();\n\n// Detener con fade out\nAudioSystem.StopMusic(fadeDuration: 1.5f);",
                        explanation = "Detiene la música actual, con o sin fade out."
                    }
                },
                features = new List<string>
                {
                    "Pooling automático de AudioSources",
                    "Fade in/out para música",
                    "Sistema de base de datos con ScriptableObject",
                    "Volúmenes separados (música/SFX)",
                    "Persistencia de configuración",
                    "Reproducción aleatoria y secuencial de SFX"
                },
                relatedModules = new List<string> { "ServiceLocator" }
            };
            
            // DependencyInjector Module
            var dependencyInjectorModule = new TutorialModule
            {
                moduleName = "DependencyInjector",
                description = "Sistema de inyección de dependencias usando atributos",
                overview = "El DependencyInjector permite inyectar dependencias automáticamente en campos y propiedades marcados con el atributo [Inject]. Esto reduce el acoplamiento y facilita el testing.\n\n" +
                          "Cómo funciona:\n" +
                          "• Marca campos/propiedades con [Inject]\n" +
                          "• Llama a DependencyInjector.InjectDependencies()\n" +
                          "• Las dependencias se inyectan automáticamente",
                quickStart = "1. Marca tus campos con [Inject]\n2. Llama DependencyInjector.InjectDependencies(this, dependencies...)\n3. Las dependencias se inyectan automáticamente",
                codeExamples = new List<TutorialModule.CodeExample>
                {
                    new TutorialModule.CodeExample
                    {
                        title = "Inyección Básica",
                        code = "public class PlayerController : MonoBehaviour\n{\n    [Inject] private AudioSystem _audioSystem;\n    [Inject] private GameManager _gameManager;\n    \n    private void Start()\n    {\n        var audioSystem = ServiceLocator.Get<AudioSystem>();\n        var gameManager = ServiceLocator.Get<GameManager>();\n        \n        DependencyInjector.InjectDependencies(this, audioSystem, gameManager);\n    }\n}",
                        explanation = "Los campos marcados con [Inject] se llenan automáticamente."
                    },
                    new TutorialModule.CodeExample
                    {
                        title = "Inyección en Propiedades",
                        code = "public class WeaponSystem\n{\n    [Inject] private AudioSystem AudioSystem { get; set; }\n    \n    public void Fire()\n    {\n        AudioSystem.PlaySFX(\"gunshot\");\n    }\n}",
                        explanation = "También funciona con propiedades privadas."
                    }
                },
                features = new List<string>
                {
                    "Inyección automática de dependencias",
                    "Soporte para campos y propiedades",
                    "Usa atributos [Inject]",
                    "Reduce acoplamiento",
                    "Facilita testing"
                },
                relatedModules = new List<string> { "ServiceLocator" }
            };
            
            // TGDebug Module
            var tgDebugModule = new TutorialModule
            {
                moduleName = "TGDebug",
                description = "Sistema de debug que solo funciona en el editor",
                overview = "TGDebug es un wrapper sobre Unity's Debug que solo compila en el editor. Esto significa que todos los logs se eliminan automáticamente en builds finales, mejorando el rendimiento.\n\n" +
                          "Ventajas:\n" +
                          "• No afecta el rendimiento en builds\n" +
                          "• Misma API que Debug de Unity\n" +
                          "• Compilación condicional automática",
                quickStart = "Usa TGDebug igual que Debug.Log:\nTGDebug.Log(\"Mensaje\");\nTGDebug.LogWarning(\"Advertencia\");\nTGDebug.LogError(\"Error\");",
                codeExamples = new List<TutorialModule.CodeExample>
                {
                    new TutorialModule.CodeExample
                    {
                        title = "Uso Básico",
                        code = "// Log normal\nTGDebug.Log(\"Jugador inició sesión\");\n\n// Log con contexto\nTGDebug.Log(\"Error en objeto\", gameObject);\n\n// Warnings y Errors\nTGDebug.LogWarning(\"Valor fuera de rango\");\nTGDebug.LogError(\"Error crítico\");",
                        explanation = "Misma API que Debug, pero solo compila en editor."
                    },
                    new TutorialModule.CodeExample
                    {
                        title = "Log de Excepciones",
                        code = "try\n{\n    // código que puede fallar\n}\ncatch (Exception e)\n{\n    TGDebug.LogException(e);\n    TGDebug.LogException(e, this); // con contexto\n}",
                        explanation = "También soporta logging de excepciones."
                    }
                },
                features = new List<string>
                {
                    "Solo compila en editor",
                    "Eliminado automáticamente en builds",
                    "Misma API que Debug de Unity",
                    "Mejora el rendimiento en producción"
                },
                relatedModules = new List<string>()
            };
            
            // GameDesignTool Module
            var gameDesignToolModule = new TutorialModule
            {
                moduleName = "GameDesignTool",
                description = "Herramienta para diseñadores: gestión de configuraciones",
                overview = "GameDesignTool permite crear ScriptableObjects marcados con [DesignerConfig] que aparecen automáticamente en una ventana de editor. Esto permite a los diseñadores modificar configuraciones sin tocar código.\n\n" +
                          "Cómo usarlo:\n" +
                          "1. Crea un ScriptableObject\n" +
                          "2. Márcalo con [DesignerConfig]\n" +
                          "3. Ábrelo desde Tools/Designer Config Tool",
                quickStart = "1. Crea un ScriptableObject y márcalo con [DesignerConfig]\n2. Abre Tools/Designer Config Tool\n3. Edita tus configuraciones",
                codeExamples = new List<TutorialModule.CodeExample>
                {
                    new TutorialModule.CodeExample
                    {
                        title = "Crear Configuración de Diseñador",
                        code = "[DesignerConfig]\n[CreateAssetMenu(menuName = \"Game/Config/EnemyConfig\")]\npublic class EnemyConfig : ScriptableObject\n{\n    public float health = 100f;\n    public float speed = 5f;\n    public int damage = 10;\n}",
                        explanation = "Cualquier ScriptableObject con [DesignerConfig] aparece en la herramienta."
                    }
                },
                features = new List<string>
                {
                    "Ventana de editor dedicada",
                    "Acceso fácil para diseñadores",
                    "No requiere código para editar",
                    "Organización automática"
                },
                relatedModules = new List<string>()
            };
            
            // ProjectSetup Module
            var projectSetupModule = new TutorialModule
            {
                moduleName = "ProjectSetup",
                description = "Herramienta para configurar el proyecto inicial",
                overview = "ProjectSetup es una ventana de editor que facilita la configuración inicial del proyecto. Permite:\n\n" +
                          "• Instalar paquetes esenciales\n" +
                          "• Importar assets necesarios\n" +
                          "• Crear estructura de carpetas base",
                quickStart = "1. Abre TGFramework/Project Setup/Essential Assets & Packages\n2. Selecciona lo que necesitas\n3. Haz clic en Import Selected",
                codeExamples = new List<TutorialModule.CodeExample>
                {
                    new TutorialModule.CodeExample
                    {
                        title = "Crear Estructura de Carpetas",
                        code = "// Desde el menú:\n// TGFramework/Project Setup/Create Base Folders\n\n// Esto crea:\n// Game/\n//   - Animations\n//   - Data\n//   - Fonts\n//   - Prefabs\n//   - Scenes\n//   - Scripts\n//   - Sprites\n//   - Settings",
                        explanation = "Crea automáticamente la estructura de carpetas base del proyecto."
                    }
                },
                features = new List<string>
                {
                    "Instalación de paquetes",
                    "Importación de assets",
                    "Creación de estructura de carpetas",
                    "Configuración inicial rápida"
                },
                relatedModules = new List<string>()
            };
            
            _tutorialData.modules = new List<TutorialModule>
            {
                serviceLocatorModule,
                audioSystemModule,
                dependencyInjectorModule,
                tgDebugModule,
                gameDesignToolModule,
                projectSetupModule
            };
        }
        
        private void OnGUI()
        {
            if (!_stylesInitialized)
            {
                InitializeStyles();
            }
            
            EditorGUILayout.BeginHorizontal();
            
            DrawModulesList();
            
            DrawModuleContent();
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawModulesList()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField(_tutorialData.frameworkName, _titleStyle);
            EditorGUILayout.LabelField($"Version {_tutorialData.version}", EditorStyles.miniLabel);
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Modules", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            for (int i = 0; i < _tutorialData.modules.Count; i++)
            {
                var module = _tutorialData.modules[i];
                var isSelected = i == _selectedModuleIndex;
                
                if (isSelected)
                {
                    GUI.backgroundColor = _tutorialData.primaryColor;
                }
                
                if (GUILayout.Button(module.moduleName, GUILayout.Height(30)))
                {
                    _selectedModuleIndex = i;
                    _selectedCodeExampleIndex = -1;
                    ResetSectionStates();
                }
                
                GUI.backgroundColor = Color.white;
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawModuleContent()
        {
            if (_selectedModuleIndex < 0 || _selectedModuleIndex >= _tutorialData.modules.Count)
                return;
            
            var module = _tutorialData.modules[_selectedModuleIndex];
            
            EditorGUILayout.BeginVertical(_boxStyle);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            EditorGUILayout.LabelField(module.moduleName, _headerStyle);
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(module.description, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(10);
            
            DrawSection("📖 Overview", ref _showOverview, () => {
                EditorGUILayout.LabelField(module.overview, EditorStyles.wordWrappedLabel);
            });
            
            DrawSection("🚀 Quick Start", ref _showQuickStart, () => {
                EditorGUILayout.LabelField(module.quickStart, EditorStyles.wordWrappedLabel);
            });
            
            DrawSection("💻 Code Examples", ref _showCodeExamples, () => {
                if (module.codeExamples != null && module.codeExamples.Count > 0)
                {
                    for (int i = 0; i < module.codeExamples.Count; i++)
                    {
                        var example = module.codeExamples[i];
                        var isSelected = i == _selectedCodeExampleIndex;
                        
                        EditorGUILayout.BeginVertical(_boxStyle);
                        
                        if (GUILayout.Button(example.title, EditorStyles.boldLabel))
                        {
                            _selectedCodeExampleIndex = isSelected ? -1 : i;
                        }
                        
                        if (isSelected)
                        {
                            EditorGUILayout.Space(5);
                            EditorGUILayout.LabelField("Code:", EditorStyles.miniLabel);
                            
                            _codeScrollPosition = EditorGUILayout.BeginScrollView(_codeScrollPosition, GUILayout.Height(150));
                            EditorGUILayout.TextArea(example.code, _codeStyle, GUILayout.ExpandHeight(true));
                            EditorGUILayout.EndScrollView();
                            
                            EditorGUILayout.Space(5);
                            EditorGUILayout.LabelField("Brief:", EditorStyles.miniLabel);
                            EditorGUILayout.LabelField(example.explanation, EditorStyles.wordWrappedLabel);
                        }
                        
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space(5);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No example code for this module.", MessageType.Info);
                }
            });
            
            DrawSection("✨ Features", ref _showFeatures, () => {
                if (module.features != null && module.features.Count > 0)
                {
                    foreach (var feature in module.features)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("•", GUILayout.Width(15));
                        EditorGUILayout.LabelField(feature, EditorStyles.wordWrappedLabel);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            });
            
            DrawSection("🔗 Related Modules", ref _showRelatedModules, () => {
                if (module.relatedModules != null && module.relatedModules.Count > 0)
                {
                    foreach (var related in module.relatedModules)
                    {
                        if (GUILayout.Button(related, EditorStyles.linkLabel))
                        {
                            var index = _tutorialData.modules.FindIndex(m => m.moduleName == related);
                            if (index >= 0)
                            {
                                _selectedModuleIndex = index;
                                _selectedCodeExampleIndex = -1;
                                ResetSectionStates();
                            }
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No related modules.", MessageType.Info);
                }
            });
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        
        private void DrawSection(string title, ref bool isExpanded, System.Action content)
        {
            EditorGUILayout.BeginVertical(_boxStyle);
            isExpanded = EditorGUILayout.Foldout(isExpanded, title, true);
            
            if (isExpanded)
            {
                EditorGUILayout.Space(5);
                content?.Invoke();
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
        
        private void ResetSectionStates()
        {
            _showOverview = true;
            _showQuickStart = false;
            _showCodeExamples = false;
            _showFeatures = false;
            _showRelatedModules = false;
        }
        
        private void InitializeStyles()
        {
            _titleStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            
            _headerStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };
            
            _codeStyle = new GUIStyle(EditorStyles.textArea)
            {
                font = EditorStyles.label.font,
                wordWrap = true,
                richText = false
            };
            
            _boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10)
            };
            
            _stylesInitialized = true;
        }
    }
}
#endif
