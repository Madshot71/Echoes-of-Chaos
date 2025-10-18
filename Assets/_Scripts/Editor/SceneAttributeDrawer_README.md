# SceneAttributeDrawer

A custom Unity property drawer for fields marked with `[Scene]`. This drawer replaces string fields in the inspector with a SceneAsset picker, making scene selection safer and more user-friendly.

## Features
- Displays a SceneAsset picker for string fields marked with `[Scene]`.
- Automatically sets the string value to the selected scene's name.
- Supports arrays and lists of scenes.
- Reduces manual errors from typing scene names.

## Usage
1. Add the `SceneAttribute` to any string field in your MonoBehaviour or ScriptableObject:
   ```csharp
   [Scene]
   public string sceneName;
   ```
2. In the inspector, select a scene using the SceneAsset picker. The field will store the scene's name as a string.

## Example
```csharp
using UnityEngine;

public class Example : MonoBehaviour {
    [Scene]
    public string mainMenuScene;
    [Scene]
    public string[] levelScenes;
}
```

## Installation
Copy `SceneAttribute.cs` and `SceneAttributeDrawer.cs` into your project's `Assets/_Scripts/Editor` and `Assets` folders.

## License
MIT License
