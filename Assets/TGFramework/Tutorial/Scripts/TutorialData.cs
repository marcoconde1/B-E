using System.Collections.Generic;
using UnityEngine;

namespace TGFramework
{
    [CreateAssetMenu(fileName = "TGFrameworkTutorial", menuName = "TGFramework/Tutorial Data", order = 1)]
    public class TutorialData : ScriptableObject
    {
        [Header("General Info")]
        public string frameworkName = "TGFramework";
        public string version = "1.0.0";
        public string description = "TGFramework Tutorial";
        
        [Header("Framework Modules")]
        public List<TutorialModule> modules = new List<TutorialModule>();
        
        [Header("Settings")]
        public Color primaryColor = new Color(0.2f, 0.6f, 1f);
        public Color secondaryColor = new Color(0.3f, 0.3f, 0.3f);
        public bool showCodeSyntaxHighlighting = true;
    }
}
