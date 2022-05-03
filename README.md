# UnityDomainReloadHelper

A couple of attributes that help when [Domain Reloading](https://docs.unity3d.com/2019.3/Documentation/Manual/DomainReloading.html) is disabled, which significantly decreases the time it takes for Unity to play a scene. By default Unity provides [RuntimeInitializeOnLoadMethod](https://docs.unity3d.com/Manual/DomainReloading.html) attribute to assist but it can be a little cumbersome. Here are a few helpful additions!

## ClearOnReloadAttribute
Use the `ClearOnReload` attribute on static fields that you wish to reset on playmode. You can either "clear" the field (set the value to default), set it to a specified value, or make it assign itself a new instance of its type using a default constructor.

## ExecuteOnReloadAttribute
Use the `ExecuteOnReload` attribute on static methods that you want to execute when entering play mode with domain reloading disabled.

### Examples
```csharp
public class CharacterManager : MonoBehaviour
{
  // Will set value to default (null).
  [ClearOnReload]
  static CharacterManager instance;
  
  // Will set variable to given value (10).
  [ClearOnReload(valueToAssign=10)]
  static int startsAsTen;
  
  // Will reset value, creating a new instance using default constructor.
  [ClearOnReload(assignNewTypeInstance=true)]
  static CharacterManager myNeverNullManager;
  
  // Will execute this method.
  [ExecuteOnReload]
  static void RunThis() 
  {
    Debug.Log("Clean up here.")
  }

  // Does not work on properties!
  // [ClearOnReload] 
  static int number { get; set; }

  // Does not work on events!
  // [ClearOnReload] 
  static event Action onDoSomething;

  // However, one can use ExecuteOnReload to do their own clean up.
  [ExecuteOnReload]
  static void CleanUpEvents() 
  {
    foreach(Delegate d in onDoSomething.GetInvocationList())
      onDoSomething -= d;
   
  }
}
```

## FAQ

- Why not support clearing properties and events?

[TypeCache](https://docs.unity3d.com/ScriptReference/TypeCache.html) makes finding attributes fast; however, it only supports finding fields, methods, types, and derived types.

## License

This project is released under the MIT license.

## Acknowledgments

This [project](https://github.com/joshcamas/unity-domain-reload-helper) is written by [Josh Steinhauer](https://twitter.com/joshcamas) with contributions from [Yevhen Bondarenko](https://github.com/JGroxz) and [Shane Celis](https://twitter.com/shanecelis).
