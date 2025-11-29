using UnityEngine;

namespace Lib.Debugger
{
    public static class LogHelper
    {
        public static void Log(string message, string tag = "")
        {
            Debug.Log(message);
        }

        public static void LogWarning(string message, string tag = "")
        {
            Debug.LogWarning(message);
        }

        public static void LogError(string message, string tag = "")
        {
            Debug.LogError(message);
        }

        public static void Log(string message, LogType logType)
        {
            switch (logType)
            {
                case LogType.Info:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
            }
        }
    }

    public enum LogType
    {
        Info,
        Warning,
        Error
    }
}