#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager;

using static System.IO.Path;
using static UnityEditor.AssetDatabase;

namespace TGFramework
{
    public class ProjectSetupWindow : EditorWindow
    {
        private Vector2 _scrollPos;

        private static List<AssetItem> assetItems = new List<AssetItem>()
        {
            new AssetItem("DOTween Pro", "Demigiant/Editor ExtensionsVisual Scripting", "DOTween Pro.unitypackage"),
            new AssetItem("Lean Pool", "Carlos Wilkes/Editor ExtensionsSystem", "Lean Pool.unitypackage")
        };

        private static List<PackageItem> packageItems = new List<PackageItem>()
        {
            new PackageItem("Artees semver", "games.artees.semver"),
            new PackageItem("Newtonsoft", "com.unity.nuget.newtonsoft-json"),
            new PackageItem("Hierarchy Decorator", "git+https://github.com/WooshiiDev/HierarchyDecorator.git"),
            new PackageItem("UniTask", "git+https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask"),
            new PackageItem("Easy Text Effect", "git+https://github.com/LeiQiaoZhi/Easy-Text-Effect.git")
        };

        [MenuItem("TGFramework/Project Setup/Essential Assets & Packages")]
        public static void ShowWindow()
        {
            var window = GetWindow<ProjectSetupWindow>("Project Setup");
            window.minSize = new Vector2(400, 500);
        }
        
        [MenuItem("TGFramework/Project Setup/Create Base Folders")]
        public static void CreateFolders() {
            Folders.Create("Game", "Animations", "Audio", "Data", "Fonts", "Prefabs", "Scenes", "Scripts", "Sprites");
            Refresh();
            Folders.Move("Game", "Scenes");
            Folders.Move("Game", "Settings");
            Folders.Delete("TutorialInfo");
            Refresh();
            
            DeleteAsset("Assets/Readme.asset");
            Refresh();
            
            // Optional: Disable Domain Reload
            // EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;
        }

        private void OnGUI()
        {
            GUILayout.Label("Essential Assets & Packages", EditorStyles.boldLabel);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            GUILayout.Space(10);
            GUILayout.Label("Assets", EditorStyles.boldLabel);
            foreach (var asset in assetItems)
            {
                asset.Selected = EditorGUILayout.ToggleLeft(asset.Name, asset.Selected);
            }

            GUILayout.Space(10);
            GUILayout.Label("Packages", EditorStyles.boldLabel);
            foreach (var package in packageItems)
            {
                package.Selected = EditorGUILayout.ToggleLeft(package.DisplayName, package.Selected);
            }

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Import Selected"))
            {
                ImportSelected();
            }

            if (GUILayout.Button("Import All"))
            {
                SelectAll();
                ImportSelected();
            }
        }

        private void SelectAll()
        {
            assetItems.ForEach(a => a.Selected = true);
            packageItems.ForEach(p => p.Selected = true);
        }

        private void ImportSelected()
        {
            foreach (var asset in assetItems)
            {
                if (asset.Selected)
                {
                    asset.Import();
                }
            }

            foreach (var package in packageItems)
            {
                if (!package.Selected)
                {
                    continue;
                }
                
                if (!package.IsInstalled())
                {
                    package.Install();
                }
                else
                {
                    TGDebug.Log(package.DisplayName + " is already installed.");
                }
            }
        }

        // ----- Data Classes -----
        private class AssetItem
        {
            public string Name;
            public string Folder;
            public string FileName;
            public bool Selected;

            public AssetItem(string name, string folder, string fileName)
            {
                Name = name;
                Folder = folder;
                FileName = fileName;
                Selected = false;
            }

            public void Import()
            {
                string basePath = Application.platform == RuntimePlatform.OSXEditor ?
                    Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Library/Unity/Asset Store-5.x") :
                    Path.Combine(EditorPrefs.GetString("AssetStoreCacheRootPath", Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Unity")), "Asset Store-5.x");

                string fullPath = Path.Combine(basePath, Folder, FileName);
                if (File.Exists(fullPath))
                {
                    AssetDatabase.ImportPackage(fullPath, false);
                    TGDebug.Log($"Imported Asset: {Name}");
                }
                else
                {
                    TGDebug.LogWarning($"Asset not found: {fullPath}");
                }
            }
        }

        private class PackageItem
        {
            public string DisplayName;
            public string Name;
            public bool Selected;

            public PackageItem(string displayName, string name)
            {
                DisplayName = displayName;
                Name = name;
                Selected = false;
            }

            public bool IsInstalled()
            {
                var listRequest = Client.List(true);
                while (!listRequest.IsCompleted) { }
                foreach (var package in listRequest.Result)
                {
                    if (package.packageId.Contains(Name))
                    {
                        return true;
                    }
                }
                return false;
            }

            public void Install()
            {
                var request = Client.Add(Name);
                EditorApplication.update += () =>
                {
                    if (request.IsCompleted)
                    {
                        if (request.Status == StatusCode.Success)
                        {
                            TGDebug.Log($"Installed Package: {request.Result.packageId}");
                        }
                        else
                        {
                            TGDebug.LogError($"Failed to install {Name}: {request.Error.message}");
                        }
                        EditorApplication.update -= null;
                    }
                };
            }
        }
        
        static class Folders {
            public static void Create(string root, params string[] folders) {
                var fullpath = Combine(Application.dataPath, root);
                if (!Directory.Exists(fullpath)) {
                    Directory.CreateDirectory(fullpath);
                }

                foreach (var folder in folders) {
                    CreateSubFolders(fullpath, folder);
                }
            }
        
            static void CreateSubFolders(string rootPath, string folderHierarchy) {
                var folders = folderHierarchy.Split('/');
                var currentPath = rootPath;

                foreach (var folder in folders) {
                    currentPath = Combine(currentPath, folder);
                    if (!Directory.Exists(currentPath)) {
                        Directory.CreateDirectory(currentPath);
                    }
                }
            }
        
            public static void Move(string newParent, string folderName) {
                var sourcePath = $"Assets/{folderName}";
                if (IsValidFolder(sourcePath)) {
                    var destinationPath = $"Assets/{newParent}/{folderName}";
                    var error = MoveAsset(sourcePath, destinationPath);

                    if (!string.IsNullOrEmpty(error)) {
                        TGDebug.LogError($"Failed to move {folderName}: {error}");
                    }
                }
            }
        
            public static void Delete(string folderName) {
                var pathToDelete = $"Assets/{folderName}";

                if (IsValidFolder(pathToDelete)) {
                    DeleteAsset(pathToDelete);
                }
            }
        }
    }
}
#endif