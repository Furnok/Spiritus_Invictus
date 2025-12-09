using UnityEngine;

public interface I_ReflectableProjectile
{
    public bool CanReflect();
    public void Reflect(Transform reflectOwner);
}
