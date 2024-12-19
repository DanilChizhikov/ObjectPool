# ObjectPool
![](https://img.shields.io/badge/unity-2022.3+-000.svg)

## Description
This package shows an implementation of the ObjectPool template, which allows you to optimize memory management.

## Table of Contents
- [Getting Started](#Getting-Started)
    - [Install manually (using .unitypackage)](#Install-manually-(using-.unitypackage))
    - [Install via UPM (using Git URL)](#Install-via-UPM-(using-Git-URL))
- [Basic Usage](#Basic-Usage)
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
You need to describe your own providers inheriting from `class PoolProvider<TConfig, TObject>`
For example you can see `class TestPoolProvider` on EditorTests folder.

## License

MIT