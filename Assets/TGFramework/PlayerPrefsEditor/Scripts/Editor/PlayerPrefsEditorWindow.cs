#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TGFramework
{
    /// <summary>
    /// Editor window to view, add, edit, and delete PlayerPrefs entries.
    /// Similar to the PlayerPrefs Editor asset. Keys are tracked in a list; use Load to refresh values from disk.
    /// </summary>
    public class PlayerPrefsEditorWindow : EditorWindow
    {
        private const string PrefsKey = "TGFramework_PlayerPrefsEditor_Entries";
        private const string WindowTitle = "PlayerPrefs Editor";

        public enum ValueType { String, Int, Float }

        [Serializable]
        private class Entry
        {
            public string key;
            public int type; // 0=String, 1=Int, 2=Float
            public string value;

            public ValueType TypeEnum
            {
                get => (ValueType)type;
                set => type = (int)value;
            }
        }

        [Serializable]
        private class EntryList
        {
            public List<Entry> entries = new();
        }

        private EntryList _entryList = new();
        private Vector2 _scroll;
        private string _newKey = "";
        private int _newType = 0;
        private string _newValue = "";
        private string _searchFilter = "";
        private int _selectedIndex = -1;
        private bool _listDirty;

        [MenuItem("TGFramework/PlayerPrefs Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<PlayerPrefsEditorWindow>(WindowTitle);
            window.minSize = new Vector2(420, 320);
        }

        private void OnEnable()
        {
            LoadStoredList();
        }

        private void OnGUI()
        {
            DrawToolbar();

            EditorGUILayout.Space(4);

            DrawAddBar();

            EditorGUILayout.Space(4);

            DrawSearchBar();

            EditorGUILayout.Space(4);

            DrawList();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh", "Reload"), EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                LoadFromPlayerPrefs();
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_SaveAs", "Save"), EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                SaveToPlayerPrefs();
            }

            GUILayout.Space(8);

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_TreeEditor.Trash", "Delete All"), EditorStyles.toolbarButton, GUILayout.Width(70)))
            {
                if (EditorUtility.DisplayDialog("Delete All PlayerPrefs", "Remove all listed keys and clear PlayerPrefs?", "Delete All", "Cancel"))
                {
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                    _entryList.entries.Clear();
                    SaveStoredList();
                }
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField($"Keys: {_entryList.entries.Count}", EditorStyles.miniLabel);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawAddBar()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Add new key", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            _newKey = EditorGUILayout.TextField("Key", _newKey ?? "", GUILayout.MinWidth(120));
            _newType = (int)(ValueType)EditorGUILayout.EnumPopup((ValueType)_newType, GUILayout.Width(70));
            _newValue = EditorGUILayout.TextField("Value", _newValue ?? "", GUILayout.ExpandWidth(true));

            GUI.enabled = !string.IsNullOrWhiteSpace(_newKey);
            if (GUILayout.Button(EditorGUIUtility.IconContent("P4_AddedRemote@2x"), GUILayout.Width(44)))
            {
                AddEntry(_newKey.Trim(), (ValueType)_newType, _newValue ?? "");
                _newKey = "";
                _newValue = "";
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawSearchBar()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search", GUILayout.Width(40));
            _searchFilter = EditorGUILayout.TextField(_searchFilter ?? "");
            if (GUILayout.Button("Clear", GUILayout.Width(44)))
            {
                _searchFilter = "";
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawList()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Key", EditorStyles.boldLabel, GUILayout.Width(140));
            GUILayout.Label("Type", EditorStyles.boldLabel, GUILayout.Width(50));
            GUILayout.Label("Value", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            GUILayout.Space(24);
            EditorGUILayout.EndHorizontal();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            var entries = _entryList.entries;
            var filter = (_searchFilter ?? "").Trim().ToLowerInvariant();
            var filtered = string.IsNullOrEmpty(filter)
                ? entries
                : entries.Where(e => (e.key ?? "").ToLowerInvariant().Contains(filter)).ToList();

            for (int i = 0; i < filtered.Count; i++)
            {
                var entry = filtered[i];
                var idx = entries.IndexOf(entry);
                var selected = idx == _selectedIndex;

                EditorGUILayout.BeginHorizontal(selected ? EditorStyles.selectionRect : GUIStyle.none);

                if (GUILayout.Button(entry.key ?? "", EditorStyles.label, GUILayout.Width(140), GUILayout.ExpandHeight(false)))
                {
                    _selectedIndex = idx;
                }

                var newType = (ValueType)EditorGUILayout.EnumPopup(entry.TypeEnum, GUILayout.Width(50));
                if (newType != entry.TypeEnum)
                {
                    entry.TypeEnum = newType;
                    _listDirty = true;
                }

                var newVal = EditorGUILayout.DelayedTextField(entry.value ?? "", GUILayout.ExpandWidth(true));
                if (newVal != (entry.value ?? ""))
                {
                    entry.value = newVal;
                    _listDirty = true;
                }

                if (GUILayout.Button(EditorGUIUtility.IconContent("d_TreeEditor.Trash"), GUILayout.Width(50)))
                {
                    if (EditorUtility.DisplayDialog("Delete key", $"Delete key \"{entry.key}\"?", "Delete", "Cancel"))
                    {
                        entries.Remove(entry);
                        _selectedIndex = Mathf.Clamp(_selectedIndex, 0, entries.Count - 1);
                        if (entries.Count == 0) _selectedIndex = -1;
                        _listDirty = true;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (_listDirty)
            {
                SaveStoredList();
                _listDirty = false;
            }

            EditorGUILayout.EndVertical();
        }

        private void AddEntry(string key, ValueType type, string value)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (_entryList.entries.Any(e => string.Equals(e.key, key, StringComparison.OrdinalIgnoreCase)))
            {
                if (EditorUtility.DisplayDialog("PlayerPrefs Editor", $"Key \"{key}\" already exists.", "Ok"))
                {
                    return;
                }
            }

            _entryList.entries.Add(new Entry { key = key, type = (int)type, value = value ?? "" });
            _listDirty = true;
        }

        private void LoadFromPlayerPrefs()
        {
            foreach (var e in _entryList.entries)
            {
                switch (e.TypeEnum)
                {
                    case ValueType.String:
                        e.value = PlayerPrefs.GetString(e.key, "");
                        break;
                    case ValueType.Int:
                        e.value = PlayerPrefs.GetInt(e.key, 0).ToString();
                        break;
                    case ValueType.Float:
                        e.value = PlayerPrefs.GetFloat(e.key, 0f).ToString();
                        break;
                }
            }
            _listDirty = true;
        }

        private void SaveToPlayerPrefs()
        {
            foreach (var e in _entryList.entries)
            {
                var key = e.key ?? "";
                if (string.IsNullOrEmpty(key)) continue;

                switch (e.TypeEnum)
                {
                    case ValueType.String:
                        PlayerPrefs.SetString(key, e.value ?? "");
                        break;
                    case ValueType.Int:
                        if (int.TryParse(e.value ?? "0", out var vi))
                            PlayerPrefs.SetInt(key, vi);
                        break;
                    case ValueType.Float:
                        if (float.TryParse(e.value ?? "0", System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var vf))
                            PlayerPrefs.SetFloat(key, vf);
                        break;
                }
            }

            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs saved.");
        }

        private void LoadStoredList()
        {
            var json = EditorPrefs.GetString(PrefsKey, "");
            if (string.IsNullOrEmpty(json))
            {
                _entryList = new EntryList();
                return;
            }
            try
            {
                _entryList = JsonUtility.FromJson<EntryList>(json);
                if (_entryList?.entries == null) _entryList = new EntryList();
            }
            catch
            {
                _entryList = new EntryList();
            }
        }

        private void SaveStoredList()
        {
            if (_entryList == null) _entryList = new EntryList();
            var json = JsonUtility.ToJson(_entryList);
            EditorPrefs.SetString(PrefsKey, json);
        }
    }
}
#endif
