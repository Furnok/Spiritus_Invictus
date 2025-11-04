using UnityEngine;
using System;

[Serializable]
public struct PlayerAttackStep
{
    public int step;
    public int multipliers;
    public float ammountConvitionNeeded;
    public float timeHoldingInput;
    public float speed;
    public ProjectileData projectileData;
}
