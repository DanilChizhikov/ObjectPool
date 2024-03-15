# ObjectPool
![](https://img.shields.io/badge/unity-2022.3+-000.svg)

## Description
This package shows an implementation of the ObjectPool template, which allows you to optimize memory management.

## Table of Contents
- [Getting Started](#Getting-Started)
    - [Install manually (using .unitypackage)](#Install-manually-(using-.unitypackage))
    - [Install via UPM (using Git URL)](#Install-via-UPM-(using-Git-URL))
- [Basic Usage](#Basic-Usage)
    - [Scene Settings](#Scene-Settings)
    - [Runtime Code](#Runtime-Code)
- [License](#License)

## Getting Started
Prerequisites:
- [GIT](https://git-scm.com/downloads)
- [Unity](https://unity.com/releases/editor/archive) 2022.3+

### Install manually (using .unitypackage)
1. Download the .unitypackage from [releases](https://github.com/DanilChizhikov/ObjectPool/releases/) page.
2. Open ObjectPool.x.x.x.unitypackage

### Install via UPM (using Git URL)
1. Navigate to your project's Packages folder and open the manifest.json file.
2. Add this line below the "dependencies": { line
    - ```json title="Packages/manifest.json"
      "com.danilchizhikov.objectpool": "https://github.com/DanilChizhikov/ObjectPool.git?path=Assets/ObjectPool",
      ```
UPM should now install the package.

## Basic Usage

### Scene Settings
First, you need to add a PoolContext to the main stage of your project, which can be done through the components menu.
`MbsCore/Pool/Context`

Or you can create this context yourself, while creating the necessary dependencies.

### Runtime Code
First, you need to initialize the PoolService, this can be done using different methods.
Here we will show the easiest way, which is not the method that we recommend using!
```csharp
public class PoolServiceBootstrap : MonoBehaviour
{
    [SerializeField] private ObjectPoolSettings _settings = default;

    private static IPoolService _service;

    public static IPoolService Service => _service;

    private void Awake()
    {
        if (_service != null)
        {
            Destroy(gameObject);
            return;
        }

        _service = new PoolService(_settings);
    }
}
```

The system allows you to get objects in different ways, all available methods are listed below.
```csharp
using UnityEngine;

namespace MbsCore.ObjectPool
{
    public interface IPoolService
    {
        int GetCloneCount<T>(T origin, CloneScope scope) where T : Component;
        int GetCloneCount(GameObject origin, CloneScope scope);
        void PrepareClones<T>(T origin, int capacity) where T : Component;
        void PrepareClones(GameObject origin, int capacity);
        T GetClone<T>(T origin, Transform parent = null) where T : Component;
        T GetClone<T>(T origin, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component;
        GameObject GetClone(GameObject origin, Transform parent = null);
        GameObject GetClone(GameObject origin, Vector3 position, Quaternion rotation, Transform parent = null);
        void ReturnClone<T>(T clone) where T : Component;
        void ReturnClone(GameObject clone);
        void Remove<T>(T origin) where T : Component;
        void Remove(GameObject origin);
    }
}
```

For example:
```csharp
public class Example : MonoBehaviour
{
    [SerializeField] private ExampleView _exampleViewPrefab = default;
    [SerializeField] private GameObject _prefab = default;

    private ExampleView _exampleViewClone;
    private GameObject _clone;
    private IPoolSerivce _poolService;

    [Inject]
    public void Construct(IPoolSerivce poolService)
    {
        _poolService = poolService;
    }

    public void CreateView()
    {
        _exampleViewClone = _poolService.GetClone(_exampleViewPrefab);
    }

    public void CreateGameObjectClone()
    {
        _clone = _poolService.GetClone(_prefab);
    }

    public void ReturnClone()
    {
        _poolService.ReturnClone(_exampleViewClone);
    }

    public void ReturnClone()
    {
        _poolService.ReturnClone(_clone);
    }

    private void Awake()
    {
        //Prepares the required number of objects so that you don't waste time creating them in the future.
        _poolService.PrepareClones(_exampleViewPrefab, 10);
        _poolService.PrepareClones(_prefab, 10);
    }

    private void OnDestroy()
    {
        //Destroys all clones that were created from this prototype.
        _poolService.Remove(_exampleViewPrefab);
        _poolService.Remove(_prefab);
    }
}
```

## License

MIT