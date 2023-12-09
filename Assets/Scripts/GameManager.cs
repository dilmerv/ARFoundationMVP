using Core;
using LearnXR.Core;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameMode currentGameMode = GameMode.Idle;

    private void Start()
    {
        HelicopterMissionManager.Instance.onMissionInfoLoaded.AddListener(() =>
        {
            currentGameMode = GameMode.Scanning;
        });
        
        HelicopterMissionManager.Instance.onSurvivorsPlaced.AddListener(() =>
        {
            currentGameMode = GameMode.Playing;
        });
        
        HelicopterMissionManager.Instance.onMissionCompleted.AddListener(() =>
        {
            currentGameMode = GameMode.Completed;
        });
        
        HelicopterMissionManager.Instance.onMissionFailed.AddListener(() =>
        {
            currentGameMode = GameMode.Failed;
        });
    }
}