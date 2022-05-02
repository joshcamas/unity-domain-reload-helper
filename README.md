# UnityDomainReloadHelper[^1]

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

## Changes

On my game project which admittedly has a lot of assemblies, I added a profile statement to the [original project](https://github.com/joshcamas/unity-domain-reload-helper) and [it took 5 seconds to run](https://github.com/shanecelis/UnityDomainReloadHelper/blob/master/Documentation%7E/original-not-helping.png) the `DomainReloadHandler` code, which virtually nullifies the gains from turning off domain reloading. So it made the allure of these convenience attributes impractical.

Alexey Zakharov [mentioned using TypeCache in Steinhauer's
post](https://forum.unity.com/threads/attribute-to-clear-static-fields-on-play-start.790226/#post-5262665).
In a quick hack, I used [TypeCache](https://docs.unity3d.com/ScriptReference/TypeCache.html) and instead of `DomainReloadHandler` taking 5 seconds, [it took 46
ms](https://github.com/shanecelis/UnityDomainReloadHelper/blob/master/Documentation%7E/modified-helping.png). 

There are some disadvantages however. TypeCache only supports finding fields, methods, types, and derived types. But I think speed is a worthwhile advantage.

## License

This project is released under the MIT license.

## Acknowledgments

The [original project](https://github.com/joshcamas/unity-domain-reload-helper) was written by Josh Steinhauer. Kudos on the design. I didn't know about TypeCache either, and I wrote similar assembly walking code that's still present in [Minibuffer Console](http://seawisphunter.com/products/minibuffer/).

[^1]: This is not the original project. It was forked by me. See Changes section for more details.
