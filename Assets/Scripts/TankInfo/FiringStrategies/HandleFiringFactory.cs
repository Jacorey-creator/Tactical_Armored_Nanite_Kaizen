using UnityEngine;

public static class HandleFiringFactory
{
    public static IHandleFiring GetHandler(TankControllers type)
    {
        return type switch
        {
            TankControllers.Player => new PlayerFiringHandler(),
            TankControllers.AI => new AIFiringHandler(),
            _ => null
        };
    }
}
