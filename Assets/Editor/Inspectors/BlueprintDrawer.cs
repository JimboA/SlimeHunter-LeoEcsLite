using System;
using System.Collections.Generic;
using System.Linq;
using Client.AppData;
using Client.AppData.Blueprints;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace Client.CustomEditors.Inspectors
{
    /// <summary>
    /// Custom inspector for Blueprints. Without ODIN it's a pain so pretty simple and straightforward.
    /// </summary>
    [CustomEditor(typeof(Blueprint))]
    [CanEditMultipleObjects]
    public class BlueprintDrawer : Editor
    {
        private List<Type> _componentTypesCache;
        private ReorderableList _modelList;
        private ReorderableList _viewList;
        private SerializedProperty _modelComponentsProp;
        private SerializedProperty _viewComponentsProp;
        private SearchComponentProvidersProvider _searchProvider;
        private Type _typeToAdd;
        private void OnEnable()
        {
            _componentTypesCache = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(ComponentProviderBase)) && !t.IsAbstract).ToList();

            _searchProvider = ScriptableObject.CreateInstance<SearchComponentProvidersProvider>();
            _searchProvider.Construct(_componentTypesCache);

            _modelComponentsProp = serializedObject.FindProperty("modelComponents");
            _viewComponentsProp = serializedObject.FindProperty("viewComponents");
            _modelList = new ReorderableList(serializedObject, _modelComponentsProp, true, true, true, true);
            _viewList = new ReorderableList(serializedObject, _viewComponentsProp, true, true, true, true);
            
            _modelList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "modelComponents"); };
            _modelList.drawElementCallback = DrawModelElement;
            _modelList.elementHeightCallback = GetModelElementHeight;
            _modelList.onAddCallback = AddElement;
            _modelList.onRemoveCallback = RemoveElement;
            
            _viewList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "viewComponents"); };
            _viewList.drawElementCallback = DrawViewElement;
            _viewList.elementHeightCallback = GetViewElementHeight;
            _viewList.onAddCallback = AddElement;
            _viewList.onRemoveCallback = RemoveElement;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();

            _modelList.DoLayoutList();
            _viewList.DoLayoutList();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawProperties(SerializedProperty prop, bool drawChildren)
        {
            string lastPropPath = string.Empty;
            foreach (SerializedProperty p in prop)
            {
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();
                    if (p.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperties(p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if(string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath))
                        continue;
                    lastPropPath = p.propertyPath;
                    EditorGUILayout.PropertyField(p, drawChildren);
                }
            }
        }

        private void DrawProperties(Rect rect, SerializedProperty prop, bool drawChildren)
        {
            string lastPropPath = string.Empty;
            foreach (SerializedProperty p in prop)
            {
                rect.y += EditorGUIUtility.singleLineHeight;
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    p.isExpanded = EditorGUI.Foldout(rect, p.isExpanded, p.displayName);
                    if (p.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperties(rect, p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if(string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath))
                        continue;
                    lastPropPath = p.propertyPath;
                    EditorGUI.PropertyField(rect, p, drawChildren);
                }
            }
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty element)
        {
            rect.x += 6;
            rect.width -= 10;
            rect.height = EditorGUIUtility.singleLineHeight;

            if (element.objectReferenceValue == null)
                return;
                
            var provider = element.objectReferenceValue as ComponentProviderBase;
            var label = "";
            if (provider)
                label = provider.GetComponentType().Name;
            element.isExpanded = EditorGUI.Foldout(rect, element.isExpanded, label);

            if (element.isExpanded)
            {
                var nestedObject = new SerializedObject(element.objectReferenceValue);
                var val = nestedObject.FindProperty("Value");
                foreach (SerializedProperty p in val)
                {
                    rect.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, p);
                }
                nestedObject.ApplyModifiedProperties();
            }
                
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
        
        private void DrawModelElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _modelComponentsProp.GetArrayElementAtIndex(index);
            DrawElement(rect, index, isActive, isFocused, element);
        }
        
        private void DrawViewElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _viewComponentsProp.GetArrayElementAtIndex(index);
            DrawElement(rect, index, isActive, isFocused, element);
        }

        private float GetElementHeight(int index, SerializedProperty element)
        {
            float additionalProps = 0;
            var baseProp = EditorGUI.GetPropertyHeight(element, true);
            if (element.isExpanded && element.objectReferenceValue != null)
            {
                var nestedObject = new SerializedObject(element.objectReferenceValue);
                var val = nestedObject.FindProperty("Value");
                foreach (SerializedProperty p in val)
                {
                    additionalProps += EditorGUIUtility.singleLineHeight;
                }
            }
                
            var spacingBetweenElements = EditorGUIUtility.singleLineHeight / 2;
            return baseProp + spacingBetweenElements + additionalProps;
        }
        
        private float GetModelElementHeight(int index)
        {
            var element = _modelComponentsProp.GetArrayElementAtIndex(index);
            return GetElementHeight(index, element);
        }
        
        private float GetViewElementHeight(int index)
        {
            var element = _viewComponentsProp.GetArrayElementAtIndex(index);
            return GetElementHeight(index, element);
        }

        private void AddElementWithSearchTreeTest(ReorderableList l)
        {
            var pos = Event.current.mousePosition;
            pos.x -= 120;
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(pos)), _searchProvider);
        }

        private void AddElement(ReorderableList l)
        {
            var pos = Event.current.mousePosition;
            pos.x -= 120;
            _searchProvider.OnSetIndexCallback = (o) =>
            {
                var type = (Type)o;
                var blueprint = target as Blueprint;
                var list = l.serializedProperty;
                var index = list.arraySize;
                if (blueprint != null && type != null)
                {
                    var instance = (ComponentProviderBase) ScriptableObject.CreateInstance(type);
                    instance.name = type.Name;
                    list.InsertArrayElementAtIndex(index);
                    var el = list.GetArrayElementAtIndex(index);
                    AssetDatabase.AddObjectToAsset(instance, blueprint);
                    EditorUtility.SetDirty(blueprint);
                    EditorUtility.SetDirty(instance);
                    AssetDatabase.SaveAssets();
                    el.objectReferenceValue = instance;
                    el.serializedObject.ApplyModifiedProperties();
                }
            };
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(pos)), _searchProvider);
        }

        private void RemoveElement(ReorderableList l)
        {
            var blueprint = target as Blueprint;
            var element = l.serializedProperty.GetArrayElementAtIndex(l.index);
            var obj = element.objectReferenceValue;
            if (obj != null)
            {
                DestroyImmediate(obj, true);
                EditorUtility.SetDirty(blueprint);
                AssetDatabase.SaveAssets();
                element.objectReferenceValue = null;
            }
            
            ReorderableList.defaultBehaviours.DoRemoveButton(l);
        }

        private void OnDestroy()
        {
            DestroyImmediate(_searchProvider);
        }
    }
}