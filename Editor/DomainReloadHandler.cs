using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.Profiling;

public class DomainReloadHandler
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void OnRuntimeLoad()
    {
        Profiler.BeginSample("DomainReloadHandler");
        int clearedValues = 0;
        int executedMethods = 0;

        /* Clear on reload */
        foreach(FieldInfo field in TypeCache.GetFieldsWithAttribute<ClearOnReloadAttribute>())
        {
            if (field != null && !field.FieldType.IsGenericParameter && field.IsStatic)
            {
                Type fieldType = field.FieldType;

                // Extract attribute and access its parameters
                var reloadAttribute = field.GetCustomAttribute<ClearOnReloadAttribute>();
                if (reloadAttribute == null)
                  continue;
                object valueToAssign = reloadAttribute.valueToAssign;
                bool assignNewTypeInstance = reloadAttribute.assignNewTypeInstance;

                // Use valueToAssign only if it's convertible to the field value type
                object value = null;
                if (valueToAssign != null) {
                    value = Convert.ChangeType(valueToAssign, fieldType);
                    if (value == null)
                        Debug.LogWarning($"Unable to assign value of type {valueToAssign.GetType()} to field {field.Name} of type {fieldType}.");
                }

                // If assignNewTypeInstance is set, create a new instance of this type and assign it to the field
                if (assignNewTypeInstance){
                    value = Activator.CreateInstance(fieldType, reloadAttribute.arguments);
                }

                try {
                    field.SetValue(null, value);
                    clearedValues++;
                }
                catch {
                    if (valueToAssign == null)
                        Debug.LogWarning($"Unable to clear field {field.Name}.");
                    else
                        Debug.LogWarning($"Unable to assign field {field.Name}.");
                }

            } else {
                Debug.LogWarning($"Inapplicable field {field.Name} to clear; must be static and non-generic.");
            }

        }

        /* Execute on reload */
        foreach(MethodInfo method in TypeCache.GetMethodsWithAttribute<ExecuteOnReloadAttribute>())
        {
            if (method != null && !method.IsGenericMethod && method.IsStatic)
            {
                method.Invoke(null, new object[] { });
                executedMethods++;
            }
        }
        Debug.Log($"Cleared {clearedValues} members; executed {executedMethods} methods.");

        Profiler.EndSample();
    }
}
