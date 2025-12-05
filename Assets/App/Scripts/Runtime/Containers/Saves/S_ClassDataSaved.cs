using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class S_ClassDataSaved
{
    [Title("General")]
    public string dateSaved = "";

    [Title("Player Pos")]
    public Vector3 position = Vector3.zero;

    public Quaternion rotation = Quaternion.identity;

    [Title("Player Stats")]
    public float health = 0;

    public float conviction = 0;

    [Title("Enemy")]
    public List<S_ClassEnemySaved> enemy = new();
}