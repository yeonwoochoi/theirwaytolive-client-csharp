using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Util
{
    public class Logger
    {
        [Conditional(("ENABLE_LOG"))]
        public static void Info(object sender, object message) {
            var fullName = sender.GetType().FullName;
            var level = " [<b><color=\"blue\">INFO</color></b>] ";
            //var timestamp = $"<b>{DateTime.Now:yyyy-MM-dd HH-mm-ssZ}</b>";
            UnityEngine.Debug.Log(new StringBuilder()
                .Append(level)
                .Append($"<b><color=\"grey\">{fullName}</color></b> : ")
                .Append($"<b>{message}</b>")
                .ToString());
        }
        
        [Conditional("ENABLE_LOG")]
        public static void Error(object sender, object message, Exception e) {
            var fullName = sender.GetType().FullName;
            var level = " [<b><color=\"red\">ERROR</color></b>] ";
            //var timestamp = $"<b>{DateTime.Now:yyyy-MM-dd HH-mm-ssZ}</b>";
            UnityEngine.Debug.LogError(new StringBuilder()
                .Append(level)
                .Append($"<b><color=\"grey\">{fullName}</color></b> : ")
                .Append($"<b>{message}</b>")
                .Append($"<b>{e.Message}</b>")
                .ToString());
        }

        [Conditional("ENABLE_LOG")]
        public static void Debug(object sender, object message) {
            var fullName = sender.GetType().FullName;
            var level = " [<b><color=\"green\">DEBUG</color></b>] ";
            //var timestamp = $"<b>{DateTime.Now:yyyy-MM-dd HH-mm-ssZ}</b>";
            UnityEngine.Debug.Log(new StringBuilder()
                .Append(level)
                .Append($"<b><color=\"grey\">{fullName}</color></b> : ")
                .Append($"<b>{message}</b>")
                .ToString());
        }

        [Conditional("ENABLE_LOG")]
        public static void Warn(object sender, object message) {
            var fullName = sender.GetType().FullName;
            var level = " [<b><color=\"pink\">WARN</color></b>] ";
            //var timestamp = $"<b>{DateTime.Now:yyyy-MM-dd HH-mm-ssZ}</b>";
            UnityEngine.Debug.LogWarning(new StringBuilder()
                .Append(level)
                .Append($"<b><color=\"grey\">{fullName}</color></b> : ")
                .Append($"<b>{message}</b>")
                .ToString());
        }
    }
}