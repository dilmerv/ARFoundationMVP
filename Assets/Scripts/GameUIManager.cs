using LearnXR.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameUIManager : Singleton<GameUIManager>
{
    [SerializeField] private Button firstStepCompleted;

    [SerializeField] private TextMeshProUGUI firstStepDescription;

    [SerializeField] private Button secondStepCompleted;

    [SerializeField] private Button wonStepCompleted;

    [SerializeField] private Button failedStepCompleted;
    
    // mission details
    [SerializeField] private TextMeshProUGUI missionNumber;
    
    [SerializeField] private TextMeshProUGUI survivor;
    
    [SerializeField] private TextMeshProUGUI maxTime;
    
    [SerializeField] private TextMeshProUGUI elapsedTime;
    
    [SerializeField] private TextMeshProUGUI minAngle;
    
    [SerializeField] private TextMeshProUGUI maxAngle;

    public UnityEvent onWizardScanStarted = new();
    
    public UnityEvent onWizardStepsDismissed = new();
    
    public UnityEvent onWonStepDismissed = new();
    
    public UnityEvent onFailedStepDismissed = new();

    private Transform firstStep, secondStep, goalStep, wonStep, failedStep;

    private void Start()
    {
        firstStep = firstStepCompleted.transform.parent;
        secondStep = secondStepCompleted.transform.parent;
        goalStep = missionNumber.transform.parent;
        wonStep = wonStepCompleted.transform.parent;
        failedStep = failedStepCompleted.transform.parent;
        
        firstStepCompleted.onClick.AddListener(() =>
        {
            firstStep.gameObject.SetActive(false);
            secondStep.gameObject.SetActive(true);
            onWizardScanStarted.Invoke();
        });
        
        secondStepCompleted.onClick.AddListener(() =>
        {
            secondStep.gameObject.SetActive(false);
            onWizardStepsDismissed.Invoke();
            goalStep.gameObject.SetActive(true);
        });
        
        wonStepCompleted.onClick.AddListener(() =>
        {
            goalStep.gameObject.SetActive(true);
            wonStep.gameObject.SetActive(false);
            onWonStepDismissed.Invoke();
        });
        
        failedStepCompleted.onClick.AddListener(() =>
        {
            goalStep.gameObject.SetActive(true);
            failedStep.gameObject.SetActive(true);
            onFailedStepDismissed.Invoke();
        });
        
        ARCapabilitiesManager.Instance.onCapabilitiesDetermined.AddListener((capabilities) =>
        {
            firstStepDescription.text = string.Empty;
            capabilities.ForEach(capability =>
            {
                firstStepDescription.text += $"{capability}\n";
            });
            
            firstStep.gameObject.SetActive(true);
        });
        
        //TODO HelicopterMissionManager onMissionCompleted and OnMissionFailed
    }

    public void UpdateStats()
    {
        //TODO implement showing stats on the goal screen
    }


    private void Update()
    {
        //TODO display elapsed time for the current level
    }
}
