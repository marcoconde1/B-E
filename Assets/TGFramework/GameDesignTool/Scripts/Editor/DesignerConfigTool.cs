using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TGFramework
{
    public class DesignerConfigTool : EditorWindow
    {
        private readonly Dictionary<string, List<ScriptableObject>> _configsBySection = new();
        private readonly Dictionary<ScriptableObject, Editor> _editorCache = new();
        private readonly HashSet<ScriptableObject> _expandedConfigs = new();
        private readonly List<string> _sections = new();

        private int _selectedSectionIndex;
        private Vector2 _leftScroll;
        private Vector2 _rightScroll;

        [MenuItem("TGFramework/Designer Config Tool")]
        public static void ShowWindow()
        {
            GetWindow<DesignerConfigTool>("Designer Configs");
        }

        private void OnEnable()
        {
            LoadAllDesignerConfigs();
        }

        private void LoadAllDesignerConfigs()
        {
            _configsBySection.Clear();
            _sections.Clear();
            ClearEditorCache();

            var guids = AssetDatabase.FindAssets("t:ScriptableObject");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (so == null) continue;

                var type = so.GetType();
                var configAttribute = type.GetCustomAttribute<DesignerConfigAttribute>();
                if (configAttribute != null)
                {
                    var sectionName = string.IsNullOrWhiteSpace(configAttribute.Section) ? "General" : configAttribute.Section;
                    if (!_configsBySection.TryGetValue(sectionName, out var sectionConfigs))
                    {
                        sectionConfigs = new List<ScriptableObject>();
                        _configsBySection[sectionName] = sectionConfigs;
                    }

                    sectionConfigs.Add(so);
                }
            }

            _sections.AddRange(_configsBySection.Keys.OrderBy(section => section, StringComparer.OrdinalIgnoreCase));
            foreach (var section in _sections)
            {
                _configsBySection[section].Sort((a, b) => string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase));
            }

            _selectedSectionIndex = Mathf.Clamp(_selectedSectionIndex, 0, Math.Max(0, _sections.Count - 1));
        }

        private void OnGUI()
        {
            if (_sections.Count == 0)
            {
                EditorGUILayout.HelpBox("Didnt found ScriptableObjects with [DesignerConfig(\"Section\")].", MessageType.Info);
                if (GUILayout.Button("Reload"))
                {
                    LoadAllDesignerConfigs();
                }

                return;
            }

            EditorGUILayout.BeginHorizontal();
            DrawSectionPanel();
            DrawConfigPanel();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            if (GUILayout.Button("Reload"))
            {
                LoadAllDesignerConfigs();
            }
        }

        private void DrawSectionPanel()
        {
            const float sectionPanelWidth = 220f;

            EditorGUILayout.BeginVertical(GUILayout.Width(sectionPanelWidth));
            EditorGUILayout.LabelField("Sections", EditorStyles.boldLabel);
            _leftScroll = EditorGUILayout.BeginScrollView(_leftScroll, GUI.skin.box);

            for (int i = 0; i < _sections.Count; i++)
            {
                bool isSelected = i == _selectedSectionIndex;
                if (GUILayout.Toggle(isSelected, _sections[i], "Button"))
                {
                    _selectedSectionIndex = i;
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawConfigPanel()
        {
            EditorGUILayout.BeginVertical();

            var selectedSection = _sections[_selectedSectionIndex];
            EditorGUILayout.LabelField(selectedSection, EditorStyles.boldLabel);
            _rightScroll = EditorGUILayout.BeginScrollView(_rightScroll, GUI.skin.box);

            var sectionConfigs = _configsBySection[selectedSection];
            foreach (var config in sectionConfigs)
            {
                if (config == null)
                {
                    continue;
                }

                DrawConfigInspector(config);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawConfigInspector(ScriptableObject config)
        {
            bool isExpanded = _expandedConfigs.Contains(config);
            bool nextExpanded = EditorGUILayout.Foldout(isExpanded, config.name, true);
            if (nextExpanded != isExpanded)
            {
                if (nextExpanded)
                {
                    _expandedConfigs.Add(config);
                }
                else
                {
                    _expandedConfigs.Remove(config);
                }
            }

            if (!nextExpanded)
            {
                return;
            }

            EditorGUI.indentLevel++;
            var editor = GetOrCreateEditor(config);
            if (editor != null)
            {
                EditorGUI.BeginChangeCheck();
                editor.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(config);
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        private Editor GetOrCreateEditor(ScriptableObject config)
        {
            if (_editorCache.TryGetValue(config, out var cachedEditor) && cachedEditor != null)
            {
                return cachedEditor;
            }

            var createdEditor = Editor.CreateEditor(config);
            _editorCache[config] = createdEditor;
            return createdEditor;
        }

        private void OnDisable()
        {
            ClearEditorCache();
        }

        private void ClearEditorCache()
        {
            foreach (var editor in _editorCache.Values)
            {
                if (editor != null)
                {
                    DestroyImmediate(editor);
                }
            }

            _editorCache.Clear();
        }
    }
}
