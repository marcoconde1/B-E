#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TGFramework
{
    public static class TutorialDataCreator
    {
        [MenuItem("TGFramework/Framework/Documentation/Create Documentation Data")]
        public static void CreateTutorialData()
        {
            var tutorialData = ScriptableObject.CreateInstance<TutorialData>();
            tutorialData.frameworkName = "TGFramework";
            tutorialData.version = "1.0.0";
            tutorialData.description = "TGFramework Tutorial";
            
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
                codeExamples = new System.Collections.Generic.List<TutorialModule.CodeExample>
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
                features = new System.Collections.Generic.List<string>
                {
                    "Registro simple con Register<T>()",
                    "Obtención type-safe con Get<T>()",
                    "Método TryGet<T>() para servicios opcionales",
                    "No requiere referencias manuales",
                    "Útil para sistemas singleton"
                },
                relatedModules = new System.Collections.Generic.List<string> { "DependencyInjector", "AudioSystem" }
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
                codeExamples = new System.Collections.Generic.List<TutorialModule.CodeExample>
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
                features = new System.Collections.Generic.List<string>
                {
                    "Pooling automático de AudioSources",
                    "Fade in/out para música",
                    "Sistema de base de datos con ScriptableObject",
                    "Volúmenes separados (música/SFX)",
                    "Persistencia de configuración",
                    "Reproducción aleatoria y secuencial de SFX"
                },
                relatedModules = new System.Collections.Generic.List<string> { "ServiceLocator" }
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
                codeExamples = new System.Collections.Generic.List<TutorialModule.CodeExample>
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
                features = new System.Collections.Generic.List<string>
                {
                    "Inyección automática de dependencias",
                    "Soporte para campos y propiedades",
                    "Usa atributos [Inject]",
                    "Reduce acoplamiento",
                    "Facilita testing"
                },
                relatedModules = new System.Collections.Generic.List<string> { "ServiceLocator" }
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
                codeExamples = new System.Collections.Generic.List<TutorialModule.CodeExample>
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
                        code = "try\n{\n    // código que puede fallar\n}\ncatch (System.Exception e)\n{\n    TGDebug.LogException(e);\n    TGDebug.LogException(e, this); // con contexto\n}",
                        explanation = "También soporta logging de excepciones."
                    }
                },
                features = new System.Collections.Generic.List<string>
                {
                    "Solo compila en editor",
                    "Eliminado automáticamente en builds",
                    "Misma API que Debug de Unity",
                    "Mejora el rendimiento en producción"
                },
                relatedModules = new System.Collections.Generic.List<string>()
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
                codeExamples = new System.Collections.Generic.List<TutorialModule.CodeExample>
                {
                    new TutorialModule.CodeExample
                    {
                        title = "Crear Configuración de Diseñador",
                        code = "[DesignerConfig]\n[CreateAssetMenu(menuName = \"Game/Config/EnemyConfig\")]\npublic class EnemyConfig : ScriptableObject\n{\n    public float health = 100f;\n    public float speed = 5f;\n    public int damage = 10;\n}",
                        explanation = "Cualquier ScriptableObject con [DesignerConfig] aparece en la herramienta."
                    }
                },
                features = new System.Collections.Generic.List<string>
                {
                    "Ventana de editor dedicada",
                    "Acceso fácil para diseñadores",
                    "No requiere código para editar",
                    "Organización automática"
                },
                relatedModules = new System.Collections.Generic.List<string>()
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
                codeExamples = new System.Collections.Generic.List<TutorialModule.CodeExample>
                {
                    new TutorialModule.CodeExample
                    {
                        title = "Crear Estructura de Carpetas",
                        code = "// Desde el menú:\n// TGFramework/Project Setup/Create Base Folders\n\n// Esto crea:\n// Game/\n//   - Animations\n//   - Data\n//   - Fonts\n//   - Prefabs\n//   - Scenes\n//   - Scripts\n//   - Sprites\n//   - Settings",
                        explanation = "Crea automáticamente la estructura de carpetas base del proyecto."
                    }
                },
                features = new System.Collections.Generic.List<string>
                {
                    "Instalación de paquetes",
                    "Importación de assets",
                    "Creación de estructura de carpetas",
                    "Configuración inicial rápida"
                },
                relatedModules = new System.Collections.Generic.List<string>()
            };

            // PlayerPrefs Editor Module
            var playerPrefsEditorModule = new TutorialModule
            {
                moduleName = "PlayerPrefs Editor",
                description = "Ventana de editor para visualizar y modificar PlayerPrefs",
                overview = "PlayerPrefs Editor es una herramienta de editor que te permite ver, crear, editar y borrar claves de PlayerPrefs sin escribir código adicional.\n\n" +
                          "Uso típico:\n" +
                          "• Depurar valores de progreso, opciones o datos de usuario\n" +
                          "• Ajustar valores en caliente sin recompilar\n" +
                          "• Limpiar PlayerPrefs durante el desarrollo",
                quickStart = "1. Abre el menú TGFramework/PlayerPrefs Editor.\n" +
                             "2. Usa el botón \"Load\" para leer los valores actuales de PlayerPrefs.\n" +
                             "3. Crea nuevas claves desde la sección \"Add new key\".\n" +
                             "4. Edita los valores directamente en la tabla.\n" +
                             "5. Pulsa \"Save\" para guardar los cambios en PlayerPrefs.\n" +
                             "6. Opcional: usa \"Delete All\" para limpiar todas las claves.",
                codeExamples = new System.Collections.Generic.List<TutorialModule.CodeExample>
                {
                    new TutorialModule.CodeExample
                    {
                        title = "Guardar una opción de volumen",
                        code = "// Guardar volumen de música\nPlayerPrefs.SetFloat(\"audio_music_volume\", 0.8f);\nPlayerPrefs.Save();\n\n// PlayerPrefs Editor mostrará la key \"audio_music_volume\" y su valor.",
                        explanation = "Cualquier clave que guardes con PlayerPrefs se puede inspeccionar y modificar desde PlayerPrefs Editor."
                    },
                    new TutorialModule.CodeExample
                    {
                        title = "Guardar progreso del jugador",
                        code = "// Guardar nivel alcanzado\nPlayerPrefs.SetInt(\"player_level\", 5);\nPlayerPrefs.Save();\n\n// Luego abre TGFramework/PlayerPrefs Editor para verificar o editar el valor.",
                        explanation = "Ideal para depurar progreso, monedas, experiencia, etc."
                    }
                },
                features = new System.Collections.Generic.List<string>
                {
                    "Listado de claves con búsqueda",
                    "Crear, editar y borrar claves",
                    "Importar claves existentes desde el sistema (editor)",
                    "Persistencia de la lista de claves en EditorPrefs",
                    "Botón para borrar todos los PlayerPrefs"
                },
                relatedModules = new System.Collections.Generic.List<string>
                {
                    "AudioSystem",
                    "ProjectSetup"
                }
            };
            
            tutorialData.modules = new System.Collections.Generic.List<TutorialModule>
            {
                serviceLocatorModule,
                audioSystemModule,
                dependencyInjectorModule,
                tgDebugModule,
                gameDesignToolModule,
                projectSetupModule,
                playerPrefsEditorModule
            };

            // If a TutorialData asset already exists, update it instead of forcing a new one.
            var existingGuids = AssetDatabase.FindAssets("t:TutorialData");
            if (existingGuids != null && existingGuids.Length > 0)
            {
                string existingPath = AssetDatabase.GUIDToAssetPath(existingGuids[0]);
                var existingData = AssetDatabase.LoadAssetAtPath<TutorialData>(existingPath);
                if (existingData != null)
                {
                    existingData.frameworkName = tutorialData.frameworkName;
                    existingData.version = tutorialData.version;
                    existingData.description = tutorialData.description;
                    existingData.modules = tutorialData.modules;

                    EditorUtility.SetDirty(existingData);
                    AssetDatabase.SaveAssets();

                    EditorUtility.DisplayDialog(
                        "Documentation Updated",
                        $"Se actualizó el TutorialData existente en:\n{existingPath}",
                        "OK");

                    Selection.activeObject = existingData;
                    return;
                }
            }

            // Otherwise, create a new asset.
            string path = EditorUtility.SaveFilePanelInProject(
                "Guardar Tutorial Data",
                "TGFrameworkTutorial",
                "asset",
                "Guarda el TutorialData con toda la documentación"
            );
            
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(tutorialData, path);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Éxito", 
                    "TutorialData creado exitosamente.\n\nAbre el tutorial desde:\nTGFramework > Framework > Tutorial", 
                    "OK");
                Selection.activeObject = tutorialData;
            }
        }
    }
}
#endif
