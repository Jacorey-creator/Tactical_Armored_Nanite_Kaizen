using UnityEngine;

public class PlayerXPScript : MonoBehaviour
{
    [SerializeField] private TankController playerTank;
    [SerializeField] private TankData level5TankData;
    [SerializeField] private TankData level10TankData;
    [SerializeField] private TankData level20TankData;
    [SerializeField] private int maxLevel = 20;

    private float xp;
    private int level = 1;
    private float xpRate = 0.5f;
    private UpgradeTank upgradeSystem;

    private void Start()
    {
        upgradeSystem = new UpgradeTank(playerTank);
    }

    private void Update()
    {
        GainXP(Time.deltaTime * GetXPRate());
    }

    private float GetXPRate()
    {
        if (level >= 20) return xpRate * 0.25f;
        if (level >= 10) return xpRate * 0.5f;
        if (level >= 5) return xpRate * 0.75f;
        return xpRate;
    }

    private void GainXP(float amount)
    {
        xp += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int newLevel = Mathf.FloorToInt(xp);

        if (newLevel > level)
        {
            level = newLevel;

            switch (level)
            {
                case 5:
                    upgradeSystem.ApplyUpgrade(level5TankData);
                    break;
                case 10:
                    upgradeSystem.ApplyUpgrade(level10TankData);
                    break;
                case 20:
                    upgradeSystem.ApplyUpgrade(level20TankData);
                    break;
            }

            if (level >= maxLevel)
            {
                xp = maxLevel;
                enabled = false; // Disables this script entirely
            }
        }
    }
}
