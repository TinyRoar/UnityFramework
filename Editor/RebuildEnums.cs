using System.IO;
using TinyRoar.Framework;
using UnityEditor;
using UnityEngine;

public class RebuildEnums : EditorWindow
{
    static string environmentsEnums = "";
    static string layerEnums = "";

    [MenuItem("Tiny Roar/Build Enum")]
    public static void ShowWindow()
    {
        MonoBehaviour.print("Reloading Layer and Environments...");

        // init
        environmentsEnums = "public enum GameEnvironment{None,";
        layerEnums = "public enum Layer{None,";

        CreateEnvironmentEnums();
        CreateLayerEnums();

        WriteFile();

        // Refresh the asset database once we're done.
        AssetDatabase.Refresh();

        MonoBehaviour.print("Enum file generated!");
    }

    private static void CreateEnvironmentEnums()
    {
        Transform tempTransform = GameObject.Find("Environment").transform;
        int childCount = tempTransform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            environmentsEnums += tempTransform.GetChild(i).name;
            environmentsEnums += ",";

            if (i == childCount - 1)
                environmentsEnums += "}";
        }
    }

    private static void CreateLayerEnums()
    {
        Transform tempTransform = GameObject.Find("UI").transform;
        int childCount = tempTransform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            if (tempTransform.GetChild(i).GetComponent<LayerConfig>() == null)
            {
                // add layerconfig if null

                int componentsCount = tempTransform.GetChild(i).gameObject.GetComponents<MonoBehaviour>().Length;
                LayerConfig layerConfig = tempTransform.GetChild(i).gameObject.AddComponent<LayerConfig>();

                // move layerconfig to top
                for (int j = 0; j < componentsCount; j++)
                {
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(layerConfig);
                }

                Debug.Log("Added LayerConfig to " + tempTransform.GetChild(i).name);
            }

            layerEnums += tempTransform.GetChild(i).name;
            layerEnums += ",";

            if (i == childCount - 1)
                layerEnums += "}";
        }
    }

    private static void WriteFile()
    {

        Directory.GetFiles(Application.dataPath);
        string filePath;
        string[] generateEnumArray = AssetDatabase.FindAssets("GeneratedEnums");

        if (generateEnumArray.Length > 0)
        {
            filePath = AssetDatabase.GUIDToAssetPath(generateEnumArray[0]);
        }
        else
        {
            // set filepath and write the lines
            filePath = Application.dataPath + "/GeneratedEnums.cs";
            Debug.Log(filePath);
            Debug.Log("GeneratedEnums.cs was created at: '" + Application.dataPath + "'.\n" +
                      "You should consider to move it to your other script files.");
        }

        StreamWriter file = new StreamWriter(filePath);
        file.WriteLine(environmentsEnums);
        file.WriteLine("");
        file.WriteLine(layerEnums);
        file.Close();
    }
}
