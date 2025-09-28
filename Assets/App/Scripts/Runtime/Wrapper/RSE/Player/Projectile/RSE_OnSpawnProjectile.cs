using UnityEngine;

[CreateAssetMenu(fileName = "RSE_OnSpawnProjectile", menuName = "Data/RSE/Player/Projectile/RSE_OnSpawnProjectile")]
public class RSE_OnSpawnProjectile : BT.ScriptablesObject.RuntimeScriptableEvent<ProjectileInitializeData> {}

public struct ProjectileInitializeData
{
    public Vector3 locationSpawn;
    public Vector3 direction;
}