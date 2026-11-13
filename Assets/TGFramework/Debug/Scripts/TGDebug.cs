using UnityEngine;

namespace TGFramework
{
    public static class TGDebug
    {
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        /// <summary>
        /// Logs a message to the Unity Console. Only compiled and executed in the editor.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(object message)
        {
            Debug.Log(message);
        }
        
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        /// <summary>
        /// Logs a message to the Unity Console with a specific context object. Only compiled and executed in the editor.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="context">The context object associated with the log entry.</param>
        public static void Log(object message, Object context)
        {
            Debug.Log(message, context);
        }
        
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        /// <summary>
        /// Logs a warning message to the Unity Console. Only compiled and executed in the editor.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        /// <summary>
        /// Logs a warning message to the Unity Console with a specific context object. Only compiled and executed in the editor.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        /// <param name="context">The context object associated with the log entry.</param>
        public static void LogWarning(object message, Object context)
        {
            Debug.LogWarning(message, context);
        }
        
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        /// <summary>
        /// Logs an error message to the Unity Console. Only compiled and executed in the editor.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        /// <summary>
        /// Logs an error message to the Unity Console with a specific context object. Only compiled and executed in the editor.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="context">The context object associated with the log entry.</param>
        public static void LogError(object message, Object context)
        {
            Debug.LogError(message, context);
        }
        
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        /// <summary>
        /// Logs an exception to the Unity Console. Only compiled and executed in the editor.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public static void LogException(System.Exception exception)
        {
            Debug.LogException(exception);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        /// <summary>
        /// Logs an exception to the Unity Console with a specific context object. Only compiled and executed in the editor.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="context">The context object associated with the log entry.</param>
        public static void LogException(System.Exception exception, Object context)
        {
            Debug.LogException(exception, context);
        }
    }

}