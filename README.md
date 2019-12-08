# unity-domain-reload-helper
A couple of attributes that help disabling [Domain Reloading](https://docs.unity3d.com/2019.3/Documentation/Manual/DomainReloading.html) in Unity easier. By default, there are a few attributes to aid in resetting static fields. These are however quite clunky. Here's a few helpful replacements!

## ClearOnReloadAttribute
Use on static fields, properties or events that you wish to reset on playmode

## ExecuteOnReloadAttribute
Use on static methods that you want to execute during a domain reload

### Examples
```
public class CharacterManager : MonoBehaviour
{

  [ClearOnReload]
  static CharacterManager instance;
  
  [ClearOnReload]
  static event Action onDoSomething;
  
  [ExecuteOnReloadAttribute]
  static void RunThis() 
  {
      Debug.Log("Clean up here or something")
  }
}
```
