using System;
using UnityEngine;

public static class TankEvents
{
    public static event Action<TankDamageEvent> OnTankDamaged;

    public static void RaiseTankDamaged(TankDamageEvent damageEvent)
    {
        OnTankDamaged?.Invoke(damageEvent);
    }
}
