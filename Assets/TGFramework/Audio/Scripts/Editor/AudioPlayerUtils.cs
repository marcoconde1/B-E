using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TGFramework
{
    public class AudioPlayerUtils
    {
        public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioClipType = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo methodPlay = audioClipType.GetMethod(
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[]{typeof(AudioClip), typeof(int), typeof(bool)},
                null
                );
            
            methodPlay.Invoke(
                null,
                new object[] { clip, startSample, loop }
                );
        }
        
        public static void StopAllClips()
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioClipType = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo methodPlay = audioClipType.GetMethod(
                "StopAllPreviewClips",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[]{},
                null
            );
            
            methodPlay.Invoke(
                null,
                new object[] { }
            );
        }
        
        public static bool IsClipPlaying()
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type[] callParameterTypes = Array.Empty<Type>();
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo methodPlay = audioUtilClass.GetMethod(
                "IsPreviewClipPlaying",
                BindingFlags.Static | BindingFlags.Public,
                null,
                callParameterTypes,
                null
            );
            
            return (bool)methodPlay.Invoke(
                null,
                callParameterTypes
            );
        } 
    }
}