using UnityEngine;
using UnityEngine.AI;

public static class MovementStrategyFactory
{
    public static ITankMovementStrategy GetStrategy(
         TankControllers type,
         GameObject tankObject,
         MonoBehaviour inputSource = null,
         Transform cameraPivot = null,
         Transform aiTarget = null)
    {
        switch (type)
        {
            case TankControllers.Player:
                if (inputSource is ITankInput playerInput && cameraPivot != null)
                {
                    return new PlayerMovementStrategy(playerInput, cameraPivot);
                }
                Debug.LogWarning("Missing input or camera pivot for PlayerInput strategy.");
                return null;

            case TankControllers.AI:
                var agent = tankObject.GetComponent<NavMeshAgent>();
                if (agent != null && aiTarget != null)
                {
                    return new AIMovementStrategy(agent, aiTarget);
                }
                Debug.LogWarning("Missing NavMeshAgent or AI target for AI strategy.");
                return null;

            default:
                Debug.LogError($"Unknown movement strategy type: {type}");
                return null;
        }
    }
}
