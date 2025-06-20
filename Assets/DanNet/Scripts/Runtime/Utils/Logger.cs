namespace Dan
{
    internal static class Logger
    {
        internal enum LogType
        {
            Info,
            Warning,
            Error
        }
        
        internal static void Log(string message, LogType logType = LogType.Info)
        {
#if UNITY_EDITOR
            switch (logType)
            {
                case LogType.Info:
                    UnityEngine.Debug.Log(message);
                    break;
                case LogType.Warning:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                case LogType.Error:
                    UnityEngine.Debug.LogError(message);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
#endif
        }
    }
}