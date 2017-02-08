using System;
using UnityEngine;

namespace TinyRoar.Framework
{
    public static class Print
    {
        public static void Log(string message)
        {
            Debug.Log(message);
        }

        public static void Log(string message, Color color, bool bold = false, bool italic = false)
        {
            // make sure to only do coloring in editor
            #if UNITY_EDITOR
            message = AddColorTag(message, color);

            if (bold)
                message = AddBoldTag(message);

            if (italic)
                message = AddItalicTag(message);
            #endif

            Debug.Log(message);
        }

        private static string AddColorTag(string message, Color color)
        {
            string start = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">";
            string end = "</color>";

            message = message.Insert(0, start);
            message = message.Insert(message.Length, end);

            return message;
        }

        private static string AddBoldTag(string message)
        {
            string start = "<b>";
            string end = "</b>";

            message = message.Insert(0, start);
            message = message.Insert(message.Length, end);

            return message;
        }

        private static string AddItalicTag(string message)
        {
            string start = "<i>";
            string end = "</i>";

            message = message.Insert(0, start);
            message = message.Insert(message.Length, end);

            return message;
        }
    }
}
