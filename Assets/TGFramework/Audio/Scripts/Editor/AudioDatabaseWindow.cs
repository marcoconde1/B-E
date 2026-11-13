#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
namespace TGFramework
{
    public class AudioDatabaseWindow : EditorWindow
    {
        private const string WINDOW_TITLE = "Audio System";

        private AudioClipDataModel _database;
        private SerializedObject _serializedDatabase;
        private SerializedProperty _collectionsProp;

        private int _selectedCollectionIndex = -1;

        private Vector2 _collectionsScroll;
        private Vector2 _entriesScroll;

        private string _entrySearch = string.Empty;

        private GUIStyle _titleStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _boxStyle;
        private bool _stylesInitialized;

        [MenuItem("TGFramework/Audio/Audio System")]
        public static void ShowWindow()
        {
            var window = GetWindow<AudioDatabaseWindow>(WINDOW_TITLE);
            window.minSize = new Vector2(800, 500);
            window.Initialize();
        }

        private void Initialize()
        {
            if (_database == null)
            {
                AutoFindDatabase();
            }

            if (_database != null)
            {
                _serializedDatabase = new SerializedObject(_database);
                _collectionsProp = _serializedDatabase.FindProperty("Collections");
            }
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void OnGUI()
        {
            if (!_stylesInitialized)
            {
                InitializeStyles();
            }

            DrawToolbar();

            if (_database == null)
            {
                DrawNoDatabasePanel();
                return;
            }

            if (_serializedDatabase == null)
            {
                _serializedDatabase = new SerializedObject(_database);
            }

            _serializedDatabase.Update();

            EditorGUILayout.BeginHorizontal();

            DrawCollectionsPanel();
            DrawEntriesPanel();

            EditorGUILayout.EndHorizontal();

            _serializedDatabase.ApplyModifiedProperties();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            var newDatabase = (AudioClipDataModel)EditorGUILayout.ObjectField(
                _database, typeof(AudioClipDataModel), false, GUILayout.MinWidth(200));

            if (newDatabase != _database)
            {
                SetDatabase(newDatabase);
            }

            if (GUILayout.Button("Create New Database", EditorStyles.toolbarButton, GUILayout.Width(160)))
            {
                CreateNewDatabaseAsset();
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawNoDatabasePanel()
        {
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField("No Audio Database selected", _headerStyle, GUILayout.Height(24));
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField(
                "Select an existing AudioClipDataModel asset, or create a new one.",
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(16);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Auto Find Database", GUILayout.Width(180), GUILayout.Height(30)))
            {
                AutoFindDatabase();
                Initialize();
            }

            if (GUILayout.Button("Create New Database", GUILayout.Width(180), GUILayout.Height(30)))
            {
                CreateNewDatabaseAsset();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
        }

        private void DrawCollectionsPanel()
        {
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.Width(250));

            EditorGUILayout.LabelField("Collections", _headerStyle);

            EditorGUILayout.Space(4);

            _collectionsScroll = EditorGUILayout.BeginScrollView(_collectionsScroll);

            if (_collectionsProp != null && _collectionsProp.isArray)
            {
                for (int i = 0; i < _collectionsProp.arraySize; i++)
                {
                    var collectionProp = _collectionsProp.GetArrayElementAtIndex(i);
                    var nameProp = collectionProp.FindPropertyRelative("CollectionName");

                    string label = string.IsNullOrEmpty(nameProp.stringValue)
                        ? $"Collection {i}"
                        : nameProp.stringValue;

                    bool isSelected = i == _selectedCollectionIndex;
                    var buttonStyle = isSelected ? EditorStyles.miniButtonMid : EditorStyles.miniButton;

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(label, buttonStyle))
                        {
                            _selectedCollectionIndex = i;
                        }

                        if (GUILayout.Button("X", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                        {
                            if (EditorUtility.DisplayDialog(
                                "Delete Collection",
                                $"Are you sure you want to delete collection '{label}'?",
                                "Delete", "Cancel"))
                            {
                                _collectionsProp.DeleteArrayElementAtIndex(i);
                                _selectedCollectionIndex = Mathf.Clamp(_selectedCollectionIndex, 0, _collectionsProp.arraySize - 1);
                            }
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(4);

            if (GUILayout.Button("Add Collection", GUILayout.Height(24)))
            {
                AddCollection();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawEntriesPanel()
        {
            EditorGUILayout.BeginVertical(_boxStyle);

            if (_collectionsProp == null || !_collectionsProp.isArray || _collectionsProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No collections. Create one on the left to start adding audio entries.", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            if (_selectedCollectionIndex < 0 || _selectedCollectionIndex >= _collectionsProp.arraySize)
            {
                _selectedCollectionIndex = 0;
            }

            var collectionProp = _collectionsProp.GetArrayElementAtIndex(_selectedCollectionIndex);
            var nameProp = collectionProp.FindPropertyRelative("CollectionName");
            var clipsProp = collectionProp.FindPropertyRelative("Clips");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Collection Name", GUILayout.Width(110));
            nameProp.stringValue = EditorGUILayout.TextField(nameProp.stringValue);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search", GUILayout.Width(50));
            _entrySearch = EditorGUILayout.TextField(_entrySearch);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add Entry", GUILayout.Width(100)))
            {
                AddEntry(clipsProp);
            }

            if (GUILayout.Button("Sort By Key", GUILayout.Width(100)))
            {
                SortEntriesByKey(clipsProp);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8);

            _entriesScroll = EditorGUILayout.BeginScrollView(_entriesScroll);

            if (clipsProp != null && clipsProp.isArray)
            {
                for (int i = 0; i < clipsProp.arraySize; i++)
                {
                    var entryProp = clipsProp.GetArrayElementAtIndex(i);
                    var keyProp = entryProp.FindPropertyRelative("Key");
                    var clipProp = entryProp.FindPropertyRelative("Clip");

                    if (!string.IsNullOrEmpty(_entrySearch))
                    {
                        if (!keyProp.stringValue.ToLower().Contains(_entrySearch.ToLower()))
                        {
                            continue;
                        }
                    }

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Key", GUILayout.Width(40));
                    keyProp.stringValue = EditorGUILayout.TextField(keyProp.stringValue);

                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        clipsProp.DeleteArrayElementAtIndex(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(clipProp, new GUIContent("Clip"));

                    using (new EditorGUI.DisabledScope(clipProp.objectReferenceValue == null))
                    {
                        var playIcon = EditorGUIUtility.IconContent("d_PlayButton");
                        var stopIcon = EditorGUIUtility.IconContent("d_PreMatQuad");

                        if (GUILayout.Button(playIcon, GUILayout.Width(24), GUILayout.Height(18)))
                        {
                            var clip = clipProp.objectReferenceValue as AudioClip;
                            if (clip != null)
                            {
                                AudioPlayerUtils.PlayClip(clip);
                            }
                        }

                        if (GUILayout.Button(stopIcon, GUILayout.Width(24), GUILayout.Height(18)))
                        {
                            AudioPlayerUtils.StopAllClips();
                        }
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(4);
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        private void AddCollection()
        {
            if (_collectionsProp == null)
                return;

            int index = _collectionsProp.arraySize;
            _collectionsProp.InsertArrayElementAtIndex(index);

            var collectionProp = _collectionsProp.GetArrayElementAtIndex(index);
            var nameProp = collectionProp.FindPropertyRelative("CollectionName");
            var clipsProp = collectionProp.FindPropertyRelative("Clips");

            nameProp.stringValue = $"NewCollection_{index}";
            clipsProp.ClearArray();

            _selectedCollectionIndex = index;

            MarkDatabaseDirty();
        }

        private void AddEntry(SerializedProperty clipsProp)
        {
            if (clipsProp == null || !clipsProp.isArray)
                return;

            int index = clipsProp.arraySize;
            clipsProp.InsertArrayElementAtIndex(index);

            var entryProp = clipsProp.GetArrayElementAtIndex(index);
            var keyProp = entryProp.FindPropertyRelative("Key");
            var clipProp = entryProp.FindPropertyRelative("Clip");

            keyProp.stringValue = $"new_key_{index}";
            clipProp.objectReferenceValue = null;

            MarkDatabaseDirty();
        }

        private void SortEntriesByKey(SerializedProperty clipsProp)
        {
            if (clipsProp == null || !clipsProp.isArray || clipsProp.arraySize <= 1)
                return;

            var entries = new List<(string key, AudioClip clip)>();

            for (int i = 0; i < clipsProp.arraySize; i++)
            {
                var entryProp = clipsProp.GetArrayElementAtIndex(i);
                var keyProp = entryProp.FindPropertyRelative("Key");
                var clipProp = entryProp.FindPropertyRelative("Clip");

                entries.Add((keyProp.stringValue, (AudioClip)clipProp.objectReferenceValue));
            }

            entries.Sort((a, b) => string.Compare(a.key, b.key, System.StringComparison.OrdinalIgnoreCase));

            clipsProp.ClearArray();

            for (int i = 0; i < entries.Count; i++)
            {
                clipsProp.InsertArrayElementAtIndex(i);
                var entryProp = clipsProp.GetArrayElementAtIndex(i);
                var keyProp = entryProp.FindPropertyRelative("Key");
                var clipProp = entryProp.FindPropertyRelative("Clip");

                keyProp.stringValue = entries[i].key;
                clipProp.objectReferenceValue = entries[i].clip;
            }

            MarkDatabaseDirty();
        }

        private void AutoFindDatabase()
        {
            var guids = AssetDatabase.FindAssets("t:AudioClipDataModel");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var db = AssetDatabase.LoadAssetAtPath<AudioClipDataModel>(path);
                SetDatabase(db);
                return;
            }

            var defaultDb = CreateDefaultDatabaseInResources();
            SetDatabase(defaultDb);
        }

        private void CreateNewDatabaseAsset()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Audio Database",
                "AudioDatabase",
                "asset",
                "Choose a location to save the new Audio Database asset.");

            if (string.IsNullOrEmpty(path))
                return;

            var db = ScriptableObject.CreateInstance<AudioClipDataModel>();
            db.Collections = new List<AudioClipDataModel.AudioCollection>();

            AssetDatabase.CreateAsset(db, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SetDatabase(db);

            EditorGUIUtility.PingObject(db);
        }

        private AudioClipDataModel CreateDefaultDatabaseInResources()
        {
            const string defaultPath = "Assets/Resources/Audio/AudioDatabase.asset";

            var existing = AssetDatabase.LoadAssetAtPath<AudioClipDataModel>(defaultPath);
            if (existing != null)
            {
                return existing;
            }

            var directory = Path.GetDirectoryName(defaultPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var db = ScriptableObject.CreateInstance<AudioClipDataModel>();
            db.Collections = new List<AudioClipDataModel.AudioCollection>();

            AssetDatabase.CreateAsset(db, defaultPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return db;
        }

        private void SetDatabase(AudioClipDataModel database)
        {
            _database = database;
            if (_database != null)
            {
                _serializedDatabase = new SerializedObject(_database);
                _collectionsProp = _serializedDatabase.FindProperty("Collections");
                _selectedCollectionIndex = 0;
            }
            else
            {
                _serializedDatabase = null;
                _collectionsProp = null;
                _selectedCollectionIndex = -1;
            }
        }

        private void MarkDatabaseDirty()
        {
            if (_database == null)
                return;

            EditorUtility.SetDirty(_database);
        }

        private void InitializeStyles()
        {
            _titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleLeft
            };

            _headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft
            };

            _boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(8, 8, 8, 8)
            };

            _stylesInitialized = true;
        }
    }
}
#endif

