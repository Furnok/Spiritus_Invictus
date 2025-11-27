using UnityEngine;
using System;

[Serializable]
public struct S_StructPlayerAttackStep
{
    public int step;
    public int multipliers;
    public float ammountConvitionNeeded;
    public float timeHoldingInput;
    public float speed;
    public Color color;
    public S_StructDataProjectile projectileData;
}
