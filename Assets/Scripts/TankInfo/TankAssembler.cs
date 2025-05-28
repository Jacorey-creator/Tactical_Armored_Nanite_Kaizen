using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TankAssembler
{
    public static void ConfigureTank(TankController controller, TankData data)
    {
        ITankMovementStrategy moveStrategy = MovementStrategyFactory.GetStrategy(
            data.movement_strategy, controller.gameObject, controller.inputSource, controller.GetAITarget());

        IFiringStrategy fireStrategy = FiringStrategyFactory.GetStrategy(data.firing_strategy);
        IHandleFiring handler = HandleFiringFactory.GetHandler(data.movement_strategy);

        controller.SetTankVisual(data.tank_prefab);
        controller.SetTankData(data, moveStrategy, fireStrategy, handler);
    }
}


