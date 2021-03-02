using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable
{
    void Killed();
}

public interface IDamageable<T>
{
    void Damage(T damageTaken);
}
