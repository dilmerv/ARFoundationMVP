using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "HelicopterMissions/Mission", order = 1)]
public class HelicopterMission : ScriptableObject
{
   public int missionIdentifier = 0;

   public int numberOfSurvivors = 0;

   public float secondsAllowed = 60.0f;

   public float minAngle = 0;

   public float maxAngle = 45.0f;

   public float minAreaSize = 1.5f;

   public float minDistanceFromTerminal = 2.5f;
   
   // state dynamic info data below
   public int numberOfSurvivorsRescued;

   public int numberOfSurvivorsPlaced;

   public float timeElapsed;

   public void ClearDynamicSessionInfo()
   {
      numberOfSurvivorsRescued = 0;
      numberOfSurvivorsPlaced = 0;
      timeElapsed = 0;
   }
}
