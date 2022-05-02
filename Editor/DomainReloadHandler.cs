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
                object value = valueToAssign != null
                                ? Convert.ChangeType(valueToAssign, fieldType) 
                                : null;
                
                // If assignNewTypeInstance is set, create a new instance of this type and assign it to the field
                if (assignNewTypeInstance) value = Activator.CreateInstance(fieldType);
                
                try {
                    field.SetValue(null, value);
                    clearedValues++;
                }
                catch {
                    Debug.LogError("Unable to set field {field.Name}.");
                }

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
        // Debug.Log($"Cleared {clearedValues} members, executed {executedMethods} methods");

        Profiler.EndSample();
    }
}
