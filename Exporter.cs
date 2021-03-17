using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Exporter : MonoBehaviour {

    [MenuItem("Tools/Export package with tags and physics layers")]
    public static void ExportPackage()
    {
        string[] projectContent = new string[] { "Assets/rp_alison_rigged_001_U3D", "Assets/rp_eric_rigged_001_U3D", "Assets/Rolex Day Date Rose Gold",
                                                 "ProjectSettings/DynamicsManager.asset" ,
                                                 "ProjectSettings/InputManager.asset",
                                                 "ProjectSettings/TagManager.asset" };
        AssetDatabase.ExportPackage(projectContent, "ExportPackage.unitypackage",
                                                    ExportPackageOptions.Interactive |
                                                    ExportPackageOptions.Recurse |
                                                    ExportPackageOptions.IncludeDependencies);
        Debug.Log("Project Exported");
    }
}
