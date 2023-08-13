using System.Collections.Generic;
using System.Diagnostics;

using BepInEx.Logging;

namespace KKS_VROON.Logging
{
    public class PluginLog
    {
        public static void Debug(object data)
        {
            LogSource.LogDebug(Format(data));
        }

        public static void Info(object data)
        {
            LogSource.LogInfo(Format(data));
        }

        public static void Warning(object data)
        {
            LogSource.LogWarning(Format(data));
        }

        public static void Error(object data)
        {
            LogSource.LogError(Format(data));
        }

        private static string Format(object data)
        {
            var methodBase = new StackTrace().GetFrame(2).GetMethod();
            var className = methodBase.ReflectedType.Name;
            var methodName = methodBase.Name;
            return $"[{className}.{methodName}] {data}";
        }

        #region Change
        // Log when the value changes.
        public static void Change(string name, object newValue, LogLevel level = LogLevel.Info)
        {
            if (LastValues.TryGetValue(name, out var oldValue))
            {
                if (!Equals(oldValue, newValue))
                {
                    LogSource.Log(level, Format($"Change(before) {name}: {oldValue}"));
                    LogSource.Log(level, Format($"Change(after ) {name}: {newValue}"));
                    LastValues[name] = newValue;
                }
            }
            else
            {
                LogSource.Log(level, Format($"Change(before) {name}: null"));
                LogSource.Log(level, Format($"Change(after ) {name}: {newValue}"));
                LastValues[name] = newValue;
            }
        }

        private static IDictionary<string, object> LastValues { get; } = new Dictionary<string, object>();
        #endregion

        #region Setup
        public static void Setup(ManualLogSource logger)
        {
            LogSource = logger;
        }

        private static ManualLogSource LogSource { get; set; }
        #endregion
    }
}
