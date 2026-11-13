using System;
using System.Collections.Generic;
using UnityEngine;

namespace TGFramework
{
    [Serializable]
    public class TutorialModule
    {
        [Header("Module Info")]
        public string moduleName;
        public string description;
        public Texture2D icon;
        
        [Header("Content")]
        [TextArea(5, 10)]
        public string overview;
        
        [TextArea(3, 5)]
        public string quickStart;
        
        [Header("Code Examples")]
        public List<CodeExample> codeExamples = new List<CodeExample>();
        
        [Header("Features")]
        public List<string> features = new List<string>();
        
        [Header("Related Modules")]
        public List<string> relatedModules = new List<string>();
        
        [Serializable]
        public class CodeExample
        {
            public string title;
            [TextArea(5, 15)]
            public string code;
            public string explanation;
        }
    }
}
