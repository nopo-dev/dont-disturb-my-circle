public static class StatsHelper
{
    public static readonly int[] damageLevels = 
        { 5, 7, 9, 11, 13 };
    public static readonly int[] damageCosts =
        { 7000, 10000, 13000, 16000 };
    public static readonly float[] fireRateLevels = 
        { 0.1f, 0.09f, 0.08f, 0.07f, 0.06f };
    public static readonly int[] fireRateCosts =
        { 6000, 8000, 10000, 12000 };
    public static readonly int[] numSidesLevels = 
        { 1, 2, 3, 4, 5 };
    public static readonly int[] numSidesCosts =
        { 15000, 20000, 25000, 30000 };
    public static readonly float[] arcLevels =
        { 0, 15, 30, 45, 60 };
    public static readonly float[] focusArcLevels =
        { 0, 5, 10, 15, 20 };

    public static readonly int lifeCost = 5000;
    public static readonly int specialCost = 5000;

    public static readonly int startingHP = 3;
    public static readonly int startingMaxHP = 5;
    public static readonly int maxHP = 5;
    public static readonly int startingSpecials = 3;
    public static readonly int startingMaxSpecials = 5;
    public static readonly int maxSpecials = 5;
    public static readonly float movementSpeed = 7f;
    public static readonly float focusMovementSpeed = 3.5f;
    public static readonly float invulnerableTime = 2f;

    public static readonly int maxCombo = 16;
    public static readonly int comboBuildup = 8;
    public static readonly float comboDropOff = 3f;
    public static readonly float comboBuildDropOff = 0.5f;

    public static readonly float[] scaleLevels = 
        { 1f, 1.5f, 2f, 2.5f, 3f };

    public static readonly int[] scaleCosts = 
        { 8000, 12000, 16000, 20000 };

    public static readonly float levelLength = 50f;
}