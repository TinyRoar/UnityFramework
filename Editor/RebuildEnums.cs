using System.Collections.Generic;
using System.IO;
using BitStrap;
using UnityEditor;
using UnityEngine;

public class RebuildEnums : EditorWindow
{
    [MenuItem("Tiny Roar/Reload Layer and Environments")]
    public static void ShowWindow()
    {
        MonoBehaviour.print("Reloading Layer and Environments...");

        // init
        string environments = "";
        string layer = "";

        Transform tempTransform = null;
        int childCount = 0;

        environments += "public enum GameEnvironment{None,";
        layer += "public enum Layer{None,";

        // go through Envirounments
        tempTransform = GameObject.Find("Environment").transform;
        childCount = tempTransform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            environments += tempTransform.GetChild(i).name;

            environments += ",";

            if (i == childCount - 1)
                environments += "}";
        }

        // go through Layer
        tempTransform = GameObject.Find("UI").transform;
        childCount = tempTransform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            layer += tempTransform.GetChild(i).name;

            layer += ",";

            if (i == childCount - 1)
                layer += "}";
        }

        MonoBehaviour.print(environments);
        MonoBehaviour.print(layer);

        // set filepath and write the lines
        string filePath = Application.dataPath + "/Scripts/Util/" + "GeneratedEnums.cs";
        MonoBehaviour.print(filePath);

        StreamWriter file = new StreamWriter(filePath);
        file.WriteLine(environments);
        file.WriteLine("");
        file.WriteLine(layer);
        file.Close();

        // Refresh the asset database once we're done.
        AssetDatabase.Refresh();

        MonoBehaviour.print("Enum file generated!");

    }
}
