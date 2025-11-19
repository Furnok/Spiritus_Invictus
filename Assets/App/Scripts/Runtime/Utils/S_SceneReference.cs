using System;
using UnityEngine;

[Serializable]
public class S_SceneReference
{
    [SerializeField] private string sceneName = "";
    [SerializeField] private string sceneGUID = "";
    [SerializeField] private string scenePath = "";

    public string Name => sceneName;

    public string GUID => sceneGUID;

    public string Path => scenePath;
}