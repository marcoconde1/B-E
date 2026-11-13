using System.Collections.Generic;
using UnityEngine;

namespace TGFramework
{
    [CreateAssetMenu(fileName = "AudioDatabase", menuName = "TGFramework/Audio/Database")]
    public class AudioClipDataModel : ScriptableObject
    {
        [System.Serializable]
        public class AudioEntry
        {
            public string Key;
            public AudioClip Clip;
        }

        [System.Serializable]
        public class AudioCollection
        {
            public string CollectionName;
            public List<AudioEntry> Clips;
        }

        [Header("Collections")]
        public List<AudioCollection> Collections;
    }
}