using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(ActionComponent))]
public class FunctionSelectorEditor : Editor
{
    private Dictionary<GameObject, MethodInfo[]> methodCache = new();
    private Dictionary<GameObject, string[]> methodNamesCache = new();

    SerializedProperty objectsProp;

    private void OnEnable()
    {
        objectsProp = serializedObject.FindProperty("objects");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("GameObject Function List", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        for (int i = 0; i < objectsProp.arraySize; i++)
        {
            var element = objectsProp.GetArrayElementAtIndex(i);
            var actionNameProp = element.FindPropertyRelative("Action_Name");
            var targetProp = element.FindPropertyRelative("targetObject");
            var funcProp = element.FindPropertyRelative("selectedFunction");
            var paramTypeProp = element.FindPropertyRelative("parameterTypes");
            var paramNameProp = element.FindPropertyRelative("parameterNames");
            var paramValueProp = element.FindPropertyRelative("parameterValues");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(actionNameProp, new GUIContent("Action Name"));
            EditorGUILayout.PropertyField(targetProp, new GUIContent("Target Object"));

            GameObject go = (GameObject)targetProp.objectReferenceValue;
            if (go != null)
            {

                MethodInfo[] methods;
                if (!methodCache.ContainsKey(go))
                {
                     methods = GetPublicMethods(go);
                    methodCache[go] = methods;
                    methodNamesCache[go] = methods.Select(m => $"{m.DeclaringType.Name}.{m.Name}").ToArray();
                }

                 methods = methodCache[go];
                var names = methodNamesCache[go];

                int selectedIndex = Mathf.Max(0, System.Array.IndexOf(names, funcProp.stringValue));
                int newIndex = EditorGUILayout.Popup("Select Function", selectedIndex, names);

                if (newIndex != selectedIndex || funcProp.stringValue == "")
                {
                    funcProp.stringValue = names[newIndex];
                    var method = methods[newIndex];
                    var parameters = method.GetParameters();

                    // Resize and store parameter metadata
                    paramTypeProp.ClearArray();
                    paramNameProp.ClearArray();
                    paramValueProp.ClearArray();

                    for (int p = 0; p < parameters.Length; p++)
                    {
                        paramTypeProp.InsertArrayElementAtIndex(p);
                        paramNameProp.InsertArrayElementAtIndex(p);
                        paramValueProp.InsertArrayElementAtIndex(p);

                        paramTypeProp.GetArrayElementAtIndex(p).stringValue = parameters[p].ParameterType.Name;
                        paramNameProp.GetArrayElementAtIndex(p).stringValue = parameters[p].Name;
                        paramValueProp.GetArrayElementAtIndex(p).stringValue = "";
                    }
                }

                // Draw editable parameters
                if (paramNameProp.arraySize > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Parameters:", EditorStyles.boldLabel);
                    for (int p = 0; p < paramNameProp.arraySize; p++)
                    {
                        string type = paramTypeProp.GetArrayElementAtIndex(p).stringValue;
                        string name = paramNameProp.GetArrayElementAtIndex(p).stringValue;
                        var valProp = paramValueProp.GetArrayElementAtIndex(p);

                        valProp.stringValue = EditorGUILayout.TextField($"{type} {name}", valProp.stringValue);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("No parameters");
                }
            }

            if (GUILayout.Button("Remove"))
            {
                objectsProp.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add GameObject Slot"))
        {
            objectsProp.InsertArrayElementAtIndex(objectsProp.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private MethodInfo[] GetPublicMethods(GameObject go)
    {
        var result = new List<MethodInfo>();
        foreach (var comp in go.GetComponents<MonoBehaviour>())
        {
            if (comp == null) continue;
            var methods = comp.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName);
            result.AddRange(methods);
        }
        return result.ToArray();
    }
}
