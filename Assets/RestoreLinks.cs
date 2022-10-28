using System.Collections;
using System.Collections.Generic;
using Client.AppData;
using Client.AppData.Blueprints;
using UnityEditor;
using UnityEngine;

public static class RestoreLinks
{
    [MenuItem("Assets/FindLinks", false, -200)]
    public static void Restore()
    {
        var blueprints = AssetDatabase.FindAssets("t:Blueprint");

        foreach (var blueprintPath in blueprints)
        {
            var path = AssetDatabase.GUIDToAssetPath(blueprintPath);
            Debug.Log($"{path}");
            var blueprint = AssetDatabase.LoadAssetAtPath<Blueprint>(path);
            var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            foreach (var asset in assets)
            {
                if (asset is ComponentProviderBase provider)
                {
                    var type = provider.GetComponentType();
                    if (type.Namespace.Contains("View"))
                    {
                        blueprint.ViewComponents.Add(provider);
                    }
                    else
                    {
                        blueprint.ModelComponents.Add(provider);
                    }
                }
                
                EditorUtility.SetDirty(asset);
            }
            EditorUtility.SetDirty(blueprint);
        }
        
        AssetDatabase.Refresh();
    }
}
