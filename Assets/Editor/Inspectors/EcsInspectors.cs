// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Leopotam.EcsLite.UnityEditor;
using UnityEditor;
using UnityEngine;

namespace Client.CustomEditors.Inspectors
{
    internal static class EcsComponentInspectors
    {
        internal static readonly Dictionary<Type, IEcsComponentInspectorExtended> Inspectors = new Dictionary<Type, IEcsComponentInspectorExtended>();

        static EcsComponentInspectors()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IEcsComponentInspectorExtended).IsAssignableFrom(type) && !type.IsInterface &&
                        !type.IsAbstract)
                    {
                        if (Activator.CreateInstance(type) is IEcsComponentInspectorExtended inspector)
                        {
                            var componentType = inspector.GetFieldType();
                            if (!Inspectors.TryGetValue(componentType, out var prevInspector)
                                || inspector.GetPriority() > prevInspector.GetPriority())
                            {
                                Inspectors[componentType] = inspector;
                            }
                        }
                    }
                }
            }
        }

        public static (bool, bool, object) Render(string label, Type type, object value, EcsEntityDebugView debugView)
        {
            if (Inspectors.TryGetValue(type, out var inspector))
            {
                var (changed, newValue) = inspector.OnGui(label, value, debugView);
                return (true, changed, newValue);
            }

            return (false, false, null);
        }

        public static (bool, bool, object) Render(Rect rect, string label, Type type, object value, EcsEntityDebugView debugView)
        {
            if (Inspectors.TryGetValue(type, out var inspector))
            {
                var (changed, newValue) = inspector.OnGui(rect, label, value, debugView);
                return (true, changed, newValue);
            }

            return (false, false, null);
        }

        public static (bool, object) RenderEnum(string label, object value, bool isFlags)
        {
            var enumValue = (Enum) value;
            Enum newValue;
            if (isFlags)
            {
                newValue = EditorGUILayout.EnumFlagsField(label, enumValue);
            }
            else
            {
                newValue = EditorGUILayout.EnumPopup(label, enumValue);
            }

            if (Equals(newValue, value))
            {
                return (default, default);
            }

            return (true, newValue);
        }

        public static (bool, object) RenderEnum(Rect rect, string label, object value, bool isFlags)
        {
            var enumValue = (Enum) value;
            Enum newValue;
            if (isFlags)
            {
                newValue = EditorGUI.EnumFlagsField(rect, label, enumValue);
            }
            else
            {
                newValue = EditorGUI.EnumPopup(rect, label, enumValue);
            }

            if (Equals(newValue, value))
            {
                return (default, default);
            }

            return (true, newValue);
        }
    }

    internal interface IEcsComponentInspectorExtended : IEcsComponentInspector
    {
        (bool, object) OnGui(Rect rect, string label, object value, EcsEntityDebugView entityView);
    }

    internal abstract class EcsComponentInspectorTypedExtended<T> : EcsComponentInspectorTyped<T>, IEcsComponentInspectorExtended
    {
        public (bool, object) OnGui(Rect rect, string label, object value, EcsEntityDebugView entityView)
        {
            if (value == null && !IsNullAllowed())
            {
                EditorGUI.LabelField(rect, label, "null");
                return (default, default);
            }

            var typedValue = (T) value;
            var changed = OnGuiTyped(rect, label, ref typedValue, entityView);
            if (changed)
            {
                return (true, typedValue);
            }

            return (default, default);
        }

        public abstract bool OnGuiTyped(Rect rect, string label, ref T value, EcsEntityDebugView entityView);
    }
}