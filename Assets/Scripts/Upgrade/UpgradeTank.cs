using System;
using UnityEngine;

public class UpgradeTank
{
    private TankController tankController;

    public UpgradeTank(TankController controller)
    {
        tankController = controller;
    }

    public void ApplyUpgrade(TankData newTankData)
    {
        if (newTankData == null)
        {
            Debug.LogError("Cannot upgrade, TankData is null!");
            return;
        }

        try
        {
            Debug.Log("Applying upgrade...");

            // Ensure that the newTankData has a valid prefab
            if (newTankData.tank_prefab == null)
            {
                Debug.LogError("Tank prefab in newTankData is null!");
                return;
            }

            // Apply the new visual and data
            tankController.SetTankVisual(newTankData.tank_prefab);
            tankController.SetTankData(newTankData);
            Debug.Log("Upgrade applied successfully!");
        }
        catch (Exception ex)
        {
            Debug.LogError("An error occurred while applying the upgrade: " + ex.Message);
        }
    }
}