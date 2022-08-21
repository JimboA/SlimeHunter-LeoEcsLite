// ----------------------------------------------------------------------------
// The MIT License
// UnityEditor integration https://github.com/Leopotam/ecslite-unityeditor
// for LeoECS Lite https://github.com/Leopotam/ecslite
// Copyright (c) 2021-2022 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
using Client.AppData;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace Client.CodeGeneration
{
    internal class TemplatesGenerator : ScriptableObject
    {
        private const string Title = "Game template generator";
        private const string ComponentProviderTemplate = "ComponentProvider.cs.txt";

        [MenuItem("Assets/Create/Game/GenerateSOProvidersForComponents", false, -200)]
        public static void GenerateComponentProviders()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => Attribute.IsDefined(t, typeof(GenerateDataProviderAttribute)) && t.IsValueType && !t.IsAbstract);

            foreach (var type in types)
            {
                var attribute = Attribute.GetCustomAttribute(type, typeof(GenerateDataProviderAttribute));
                if (attribute == null)
                {
                    Debug.LogWarning($"Can't get attribute from type: {type.Name}");
                    continue;
                }
                
                var folderPath = GetFolder(ProjectPreferences.GeneratedComponentProvidersPath);
                CreateTemplate(GetTemplateContent(ComponentProviderTemplate), 
                    $"{folderPath}/{type.Name}SOProvider.cs", $"{folderPath}/{type.Name}.cs", type.Namespace);
            }
            AssetDatabase.Refresh();
        }

        private static string GetFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var guid = AssetDatabase.CreateFolder(Path.GetDirectoryName(path), Path.GetFileName(path));
                return AssetDatabase.GUIDToAssetPath(guid);
            }

            return path;
        }

        private static string GetAssetPath()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(path) && AssetDatabase.Contains(Selection.activeObject))
            {
                if (!AssetDatabase.IsValidFolder(path))
                {
                    path = Path.GetDirectoryName(path);
                }
            }
            else
            {
                path = "Assets";
            }

            return path;
        }

        private static Texture2D GetIcon()
        {
            return EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
        }

        private static string CreateTemplateInternal(string proto, string fileName, string nameSpace)
        {
            var res = CreateTemplate(proto, fileName, nameSpace);
            if (res != null)
            {
                EditorUtility.DisplayDialog(Title, res, "Close");
            }

            return res;
        }
        
        private static string CreateTemplate(string proto, string fileName, string scriptName, string nameSpace)
        {
            if (string.IsNullOrEmpty(scriptName))
            {
                return "Invalid fileName";
            }
            
            if (string.IsNullOrEmpty(nameSpace))
            {
                return "Invalid nameSpace";
            }
            
            proto = proto.Replace("#NS#", nameSpace);
            proto = proto.Replace("#SCRIPTNAME#", SanitizeClassName(Path.GetFileNameWithoutExtension(scriptName)));
            try
            {
                if (File.Exists(fileName))
                {
                    File.WriteAllText(fileName, proto);
                }
                else
                {
                    File.WriteAllText(AssetDatabase.GenerateUniqueAssetPath(fileName), proto);
                }
                
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
            return null;
        }
        private static string CreateTemplate(string proto, string fileName, string nameSpace)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "Invalid fileName";
            }
            
            if (string.IsNullOrEmpty(nameSpace))
            {
                return "Invalid nameSpace";
            }
            
            proto = proto.Replace("#NS#", nameSpace);
            proto = proto.Replace("#SCRIPTNAME#", SanitizeClassName(Path.GetFileNameWithoutExtension(fileName)));
            try
            {
                File.WriteAllText(AssetDatabase.GenerateUniqueAssetPath(fileName), proto);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
            return null;
        }

        private static string SanitizeClassName(string className)
        {
            var sb = new StringBuilder();
            var needUp = true;
            foreach (var c in className)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(needUp ? char.ToUpperInvariant(c) : c);
                    needUp = false;
                }
                else
                {
                    needUp = true;
                }
            }

            return sb.ToString();
        }

        private static string GetTemplateContent(string proto)
        {
            // hack: its only one way to get current editor script path. :(
            var pathHelper = CreateInstance<TemplatesGenerator>();
            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(pathHelper)));
            DestroyImmediate(pathHelper);
            try
            {
                return File.ReadAllText(Path.Combine(path ?? "", proto));
            }
            catch
            {
                return null;
            }
        }

        private static void CreateAndRenameAsset(string fileName, Texture2D icon, Action<string> onSuccess)
        {
            var action = CreateInstance<CustomEndNameAction>();
            action.Callback = onSuccess;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, fileName, icon, null);
        }

        internal sealed class CustomEndNameAction : EndNameEditAction
        {
            [NonSerialized] public Action<string> Callback;

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Callback?.Invoke(pathName);
            }
        }
    }
}