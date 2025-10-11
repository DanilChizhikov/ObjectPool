# ObjectPool

## Status
![](https://img.shields.io/badge/unity-2022.3+-000.svg)
![Tests](https://img.shields.io/badge/Tests-Passed-brightgreen.svg)

## Description
This package provides an implementation of the ObjectPool pattern for Unity, designed to optimize memory management when working with frequently created and destroyed objects.

## Table of Contents
- [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Manual Installation](#manual-installation)
    - [UPM Installation](#upm-installation)
- [Basic Usage](#basic-usage)
    - [Service Initialization](#service-initialization)
    - [Methods](#methods)
      - [Prepare Clones](#prepare-clones)
      - [Get Clones Count](#get-clones-count)
      - [Get Clones](#get-clones)
      - [Return Clones](#return-clones)
      - [Remove Clones](#remove-clones)
- [Testing](#testing)
- [Dependencies](#dependencies)
- [License](#license)

## Getting Started
### Prerequisites
- [GIT](https://git-scm.com/downloads)
- [Unity](https://unity.com/releases/editor/archive) 2022.3+

### Manual Installation
1. Download the .unitypackage from the [releases](https://github.com/DanilChizhikov/ObjectPool/releases/) page.
2. Import ObjectPool.x.x.x.unitypackage into your project.

### UPM Installation
1. Open the manifest.json file in your project's Packages folder.
2. Add the following line to the dependencies section:
    ```json
    "com.dtech.object-pool": "https://github.com/DanilChizhikov/ObjectPool.git",
    ```
3. Unity will automatically import the package.

If you want to set a target version, ObjectPool uses the `v*.*.*` release tag so you can specify a version like #v0.1.0.

For example `https://github.com/DanilChizhikov/ObjectPool.git#v0.1.0`.

## Basic Usage

### Service Initialization
```csharp
public class PoolServiceBootstrap : MonoBehaviour
{
    private static IPoolService _service;

    public static IPoolService Service => _service;

    private void Awake()
    {
        if (_service != null)
        {
            Destroy(gameObject);
            return;
        }

        _service = new PoolService();
        DontDestroyOnLoad(gameObject);
    }
}
```

### Methods

#### Prepare Clones
Prepares a pool of objects in advance to avoid runtime allocation spikes.

```csharp
// For GameObjects
void PrepareClones(GameObject origin, IPoolConfig config);

// For Components
void PrepareClones<T>(T origin, IPoolConfig config) where T : Component;
```

Parameters:
- `origin`: The original GameObject or Component to be cloned.
- `config`: The configuration for the pool, including the capacity and scope.

Example:
```csharp
// Create a pool configuration
[System.Serializable]
public class PoolConfig : IPoolConfig
{
    public int MaxCapacity => 100;
    public int InitialCapacity => 10;
}

// In your initialization code
var config = new PoolConfig();
_poolService.PrepareClones(bulletPrefab, config);
```

#### Get Clones Count
Gets the number of clones currently in the pool for a given origin object.

```csharp
// For GameObjects
int GetCloneCount(GameObject origin, CloneScope scope);

// For Components
int GetCloneCount<T>(T origin, CloneScope scope) where T : Component;
```

Parameters:
- `origin`: The original GameObject or Component for which to get the clone count.
- `scope`: The scope to count clones in (Active, Passive, or All)

Example:
```csharp
// Get count of all bullet clones (both active and inactive)
int totalBullets = _poolService.GetCloneCount(bulletPrefab, CloneScope.All);

// Get count of currently active bullet clones
int activeBullets = _poolService.GetCloneCount(bulletPrefab, CloneScope.Active);

// Get count of currently passive bullet clones
int passiveBullets = _poolService.GetCloneCount(bulletPrefab, CloneScope.Passive);
```

#### Get Clones
Gets a clone from the pool, either creating a new one or reusing an existing one.

```csharp
// For GameObjects
GameObject GetClone(GameObject origin, Transform parent = null);
GameObject GetClone(GameObject origin, Vector3 position, Quaternion rotation, Transform parent = null);

// For Components
T GetClone<T>(T origin, Transform parent = null) where T : Component;
T GetClone<T>(T origin, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component;
```

Example:
```csharp
// Get a bullet at default position/rotation
var bullet1 = _poolService.GetClone(bulletPrefab);

// Get a bullet at specific position/rotation
var bullet2 = _poolService.GetClone(
    bulletPrefab, 
    spawnPoint.position, 
    spawnPoint.rotation
);
```

#### Return Clones
Returns a clone to the pool for later reuse.
```csharp
// For GameObjects
void ReturnClone(GameObject clone);

// For Components
void ReturnClone<T>(T clone) where T : Component;
```

Example:
```csharp
// Return a bullet to the pool when it's no longer needed
private void OnBulletHit(Bullet bullet)
{
    _poolService.ReturnClone(bullet);
    // or for GameObjects: _poolService.ReturnClone(bullet.gameObject);
}
```

#### Remove Clones
Removes all clones of a specific origin from the pool.
```csharp
// For GameObjects
void Remove(GameObject origin);

// For Components
void Remove<T>(T origin) where T : Component;
```

Example:
```csharp
// When you no longer need a specific type of object
_poolService.Remove(bulletPrefab);

// For components
_poolService.Remove(bulletPrefab.GetComponent<Bullet>());
```

## Testing
The project includes a comprehensive set of unit tests that verify the correct operation of the object pool. All tests are passing successfully.

To run the tests:
1. Open the Test Runner window (Window > General > Test Runner)
2. Select PlayMode tests
3. Click "Run All"

## Dependencies
- [Extensions](https://github.com/DanilChizhikov/Extensions) 0.1+

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.