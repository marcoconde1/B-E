using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToolbarExtender
{
    private const string kElementPath = "TGFramework/Scene Selector";
    private static string[] scenePaths;
    private static EditorBuildSettingsScene[] buildSettingsScenes;

    [MainToolbarElement(kElementPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement CreateSceneSelectorDropdown()
    {
        string activeSceneName;
        if (Application.isPlaying)
        {
            activeSceneName = SceneManager.GetActiveScene().name;
        }
        else
        {
            activeSceneName = EditorSceneManager.GetActiveScene().name;
        }

        if (activeSceneName.Length == 0)
        {
            activeSceneName = "Untitled";
        }

        var icon = EditorGUIUtility.IconContent("UnityLogo").image as Texture2D;
        var content = new MainToolbarContent(activeSceneName, icon, "Select active scene.");
        return new MainToolbarDropdown(content, ShowDropdownMenu);
    }

    private static void ShowDropdownMenu(Rect dropDownRect)
    {
        var menu = new GenericMenu();
        if (scenePaths.Length == 0)
        {
            menu.AddDisabledItem(new GUIContent("No Scenes in Project"));
        }
        foreach (string scenePath in scenePaths)
        {
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            var isActiveScene = EditorSceneManager.GetActiveScene().name == sceneName;
            var isInBuildSettings = false;
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path == scenePath)
                {
                    isInBuildSettings = true;
                    break;
                }
            }
            var name = isInBuildSettings ? "In Build/" + sceneName : sceneName;
            menu.AddItem(new GUIContent(name), isActiveScene, () =>
            {
                SwitchScene(scenePath);
            });
        }
        menu.DropDown(dropDownRect);
    }

    private static void SwitchScene(string scenePath)
    {
        if (Application.isPlaying)
        {
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        else
        {
            if (File.Exists(scenePath))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
            }
        }
    }

    private static void RefreshSceneList()
    {
        scenePaths = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories);
        buildSettingsScenes = EditorBuildSettings.scenes;
    }

    private static void SceneSwitched(Scene oldScene, Scene newScene)
    {
        MainToolbar.Refresh(kElementPath);
    }

    static ToolbarExtender()
    {
        RefreshSceneList();
        EditorApplication.projectChanged += RefreshSceneList;
        SceneManager.activeSceneChanged += SceneSwitched;
        EditorSceneManager.activeSceneChangedInEditMode += SceneSwitched;
    }
}