using System.Collections;
using Core;
using LearnXR.Core;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class HelicopterMissionManager : Singleton<HelicopterMissionManager>
{
    [SerializeField] private float survivorPlacementDelay = 1.0f;

    [SerializeField] private float survivorPlacementOffset = 0.5f;
    
    [SerializeField] private float checkMissionStateFrequency = 1.0f;
    
    [SerializeField] private HelicopterMission[] allMissions;

    public HelicopterMission currentHelicopterMission;

    [SerializeField] private GameObject[] survivorsPrefabs;

    private int currentMissionIdentifier;

    public UnityEvent onMissionInfoLoaded = new();
    public UnityEvent onSurvivorsPlaced = new();
    public UnityEvent onMissionFailed = new();
    public UnityEvent onMissionCompleted = new();

    private MissionRequirements missionRequirements = MissionRequirements.NoMet;

    public bool AllLevelsBeaten => currentMissionIdentifier == allMissions.Length;

    private void Start()
    {
        GameUIManager.Instance.onWizardStepsDismissed.AddListener(() =>
        {
            LoadMissionInfo();
        });
        
        GameUIManager.Instance.onFailedStepDismissed.AddListener(() =>
        {
            LoadMissionInfo(restore: true);
        });
        
        GameUIManager.Instance.onWonStepDismissed.AddListener(() =>
        {
            LoadMissionInfo(restore: false);
        });
        
        AREnvironmentScannerManager.Instance.onPlaceableArea.AddListener((worldPosition) =>
        {
            if (GameManager.Instance.currentGameMode == GameMode.Scanning)
            {
                StartCoroutine(PlaceSurvivors(worldPosition));
                GameManager.Instance.currentGameMode = GameMode.PlacingInProgress;
            }
        });
    }
    
    private IEnumerator PlaceSurvivors(Vector3 worldPosition)
    {
        int placed = currentHelicopterMission.numberOfSurvivorsPlaced;
        if (placed >= currentHelicopterMission.numberOfSurvivors) yield return null;

        while (currentHelicopterMission.numberOfSurvivorsPlaced < currentHelicopterMission.numberOfSurvivors)
        {
            var survivorPrefab = survivorsPrefabs[Random.Range(0, survivorsPrefabs.Length)];
            var worldPositionWithOffset = new Vector3(worldPosition.x,
                worldPosition.y + (survivorPlacementOffset * currentHelicopterMission.numberOfSurvivorsPlaced),
                worldPosition.z);
            Instantiate(survivorPrefab, worldPositionWithOffset, Quaternion.identity);
            currentHelicopterMission.numberOfSurvivorsPlaced++;
            yield return new WaitForSeconds(survivorPlacementDelay);
        }

        GameManager.Instance.currentGameMode = GameMode.PlacingCompleted;
        onSurvivorsPlaced.Invoke();
    }

    private void LoadMissionInfo(bool restore = false)
    {
        // clear anything created before
        var survivors = GameObject.FindGameObjectsWithTag(Constants.SURVIVORS_TAG);
        foreach (var survivor in survivors)
        {
            Destroy(survivor.gameObject);
        }

        currentHelicopterMission = allMissions[restore ? --currentMissionIdentifier : currentMissionIdentifier];
        currentHelicopterMission.ClearDynamicSessionInfo();

        if (currentHelicopterMission.missionIdentifier < allMissions.Length)
        {
            currentMissionIdentifier++;
            onMissionInfoLoaded.Invoke();
            StartCoroutine(CheckMissionState());
        }
    }

    #region Check Mission State Information Status
    
    private IEnumerator CheckMissionState()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkMissionStateFrequency);
            var reqs = CheckMissionRequirements();

            if (reqs == MissionRequirements.AllMet)
            {
                onMissionCompleted.Invoke();
                break;
            }

            if (reqs == MissionRequirements.RescuedSurvivorsPastTime ||
                reqs == MissionRequirements.NobodyRescuedPastTime)
            {
                onMissionFailed.Invoke();
                break;
            }
        }
    }
    
    private MissionRequirements CheckMissionRequirements()
    {
        int rescueSurvivorsGoal = currentHelicopterMission.numberOfSurvivors;
        float allowTimeToRescue = currentHelicopterMission.secondsAllowed;
        var missionReqs = MissionRequirements.NoMet;

        if (currentHelicopterMission.numberOfSurvivorsRescued == rescueSurvivorsGoal &&
            currentHelicopterMission.timeElapsed <= allowTimeToRescue)
        {
            missionReqs = MissionRequirements.AllMet;
        }
        else if (currentHelicopterMission.numberOfSurvivorsRescued == rescueSurvivorsGoal &&
                 currentHelicopterMission.timeElapsed > allowTimeToRescue)
        {
            missionReqs = MissionRequirements.RescuedSurvivorsPastTime;
        }
        else if (currentHelicopterMission.timeElapsed > allowTimeToRescue)
        {
            missionReqs = MissionRequirements.NobodyRescuedPastTime;
        }
        return missionReqs;
    }
    
    #endregion
}