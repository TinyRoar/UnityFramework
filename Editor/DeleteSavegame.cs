using System.IO;
using TinyRoar.Framework;
using UnityEditor;
using UnityEngine;

public class DeleteSavegame : EditorWindow
{
    static string environmentsEnums = "";
    static string layerEnums = "";

    [MenuItem("Tiny Roar/Delete Savegame")]
    public static void ShowWindow()
    {
        DataManagement.Instance.Reset();
        PlayerPrefs.DeleteAll();
        Print.Log("Savegame deleted!");
    }
}
