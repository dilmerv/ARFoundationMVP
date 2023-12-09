namespace Core
{
    public enum RotorAxis
    {
        X,
        Y,
        Z
    }

    public enum MissionRequirements
    {
        NoMet,
        AllMet,
        RescuedSurvivorsPastTime,
        NobodyRescuedPastTime
    }

    public enum GameMode
    {
        Idle,
        Scanning,
        PlacingInProgress,
        PlacingCompleted,
        Playing,
        Completed,
        Failed
    }
}