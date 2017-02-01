using System;

namespace TinyRoar.Framework
{

    public static class CodeHelper
    {
        /*
         * Creates an Instance of a class with a specific classname
         * Usage: MapUtility mu = CodeHelper.CreateInstance<MapUtility>("MapUtility");
         * not usage for singleton classes but for everything outside the Unity stuff
         */
        public static T CreateInstance<T>(string className)
        {
            Type t = Type.GetType(className);
            return (T)Activator.CreateInstance(t);
        }

        public static T CreateInstance<T>(Type t)
        {
            return (T)Activator.CreateInstance(t);
        }

    }

}