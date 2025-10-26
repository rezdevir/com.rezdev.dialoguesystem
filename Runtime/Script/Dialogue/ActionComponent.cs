using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;


[System.Serializable]
    public class GameObjectFunction
{
    public string Action_Name;
    public GameObject targetObject;
    public string selectedFunction;
        
    [HideInInspector] public string[] parameterTypes;
    [HideInInspector] public string[] parameterNames;
    public List<string> parameterValues = new();  // editable in inspector
}



[AddComponentMenu("Dialogue/Action")]
public class ActionComponent : MonoBehaviour
{

        
        [SerializeField]
        public List<GameObjectFunction> objects = new List<GameObjectFunction>();


    void Start()
    {
        DialogManager.Instance.OnActionDelegate += ActionDispatcher;
    }


    void ActionDispatcher(string action)
    {
        bool IsAvailable = false;
        foreach (var act in objects)
        {
            if (act.Action_Name.Equals(action))
            {
                IsAvailable = true;
                GameObject action_obj = Instantiate(act.targetObject);
                Debug.Log(act.selectedFunction);



                var parts = act.selectedFunction.Split('.');

                if (parts.Length != 2)
                {
                    Debug.LogWarning($"Invalid function format: {act.selectedFunction}");
                }

                string className = parts[0];
                string methodName = parts[1];
                var comp =
                action_obj.GetComponents<MonoBehaviour>().FirstOrDefault(c => c.GetType().Name == className);

                if (comp == null)
                {
                    Debug.LogWarning($"Component {className} not found on {action_obj.name}");
                    return;
                }

                // Find the method
                var method = comp.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
                if (method == null)
                {
                    Debug.LogWarning($"Method {methodName} not found in {className}");
                    return;
                }

                // Call it
                // method.Invoke(comp, null);


            var parameters = method.GetParameters();
            object[] paramValues = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var pType = parameters[i].ParameterType;
                string value = act.parameterValues.Count > i ? act.parameterValues[i] : "";

                try
                {
                    if (pType == typeof(int))
                        paramValues[i] = int.Parse(value);
                    else if (pType == typeof(float))
                        paramValues[i] = float.Parse(value);
                    else if (pType == typeof(bool))
                        paramValues[i] = bool.Parse(value);
                    else if (pType == typeof(string))
                        paramValues[i] = value;
                    else
                        Debug.LogWarning($"Unsupported parameter type {pType.Name}");
                }
                catch
                {
                    Debug.LogWarning($"Failed to parse parameter {value} for {pType.Name}");
                }
            }

            method.Invoke(comp, paramValues);


            }
        }

        if (!IsAvailable)
            Debug.LogWarning(action + " is not set in action list");
       
    }
}
