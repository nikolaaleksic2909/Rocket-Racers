// GameSettings.cs
public static class GameSettings
{
    public enum Difficulty { Easy, Medium, Hard }
    public static Difficulty SelectedDifficulty = Difficulty.Easy;
    public static float AIMissileInterval = 10f;
    public static float RecoveryThreshold = 0.3f;
    public static float AIMaxSpeed = 90f;
    public static float AIMeterDownRate = 35f;
}
