﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;
using Random = UnityEngine.Random;

public class RoadManager : MonoBehaviour
{
    public static RoadManager Instance;    // Singleton pattern **Instance**
    
    [Tooltip("Length of total road of the level.")]
    public int RoadLength;

    /* Prefabs */
    public List<GameObject> TurnPrefabs;
    public GameObject RoadStraightPrefab;
    public GameObject PolePrefab;
    public GameObject FinishPrefab;
    
    public List<GameObject> Roads;
    public List<GameObject> Poles;

    public void Awake()
    {
        // Singleton Pattern initialization.
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
        Roads = new List<GameObject>();
        Poles = new List<GameObject>();
        
    }

    public void Start()
    {
        RoadLength = Random.Range(15,35);
        GenerateRoad();
    }
    
    
    /// <summary>
    /// Main function of road generation.
    /// </summary>
    public void GenerateRoad()
    {
        /* INITIAL ROAD PARTS */
        Roads.Add(Instantiate(RoadStraightPrefab));
        Roads.Add(Instantiate(RoadStraightPrefab));
        Roads[1].transform.position = Roads[0].transform.position + (Roads[0].transform.GetComponent<Road>().EndPosition.forward * 10);

        /* ROAD GENERRATION */
        for (var i = 0; i < RoadLength; i++)
        {
            GameObject generatedRoad = new GameObject();
            Destroy(generatedRoad);
            
            var lastRoad = Roads[Roads.Count - 1];
            var beforeLastRoad = Roads[Roads.Count - 2];

            
            // Random value will be used as road prefab selection.
            var randomValue = Random.Range(0, TurnPrefabs.Count * 2);
            
            // If the random value greater of equal then turn prefabs count, we will generate a straight road.
            // If the last two road is also a turn prefab, we should now generate a straight road regardless to whatever the random value is.
            if (randomValue >= TurnPrefabs.Count || (lastRoad.GetComponent<Road>().RoadType != RoadType.Straight && beforeLastRoad.GetComponent<Road>().RoadType == RoadType.Straight))    // %50 Straight, %50 Turning && Put turn only after straight roads
            {
                //Generate straight road
                generatedRoad = Instantiate(RoadStraightPrefab);
                generatedRoad.transform.forward = lastRoad.GetComponent<Road>().EndPosition.forward;

            }
            else
            {
                // Generate turn road
                var isFound = false;
                while (!isFound)
                {
                    generatedRoad = Instantiate(TurnPrefabs[randomValue]);
                    generatedRoad.transform.forward = lastRoad.GetComponent<Road>().EndPosition.forward;
                    if (generatedRoad.GetComponent<Road>().EndPosition.forward == Vector3.back)
                    {
                        Destroy(generatedRoad);
                        randomValue = GetRandomValueWithException(0, TurnPrefabs.Count, randomValue);
                    }
                    else
                    {
                        isFound = true;
                    }
                }
                
                // Check if last road is straight.
                // If it's, we need to generate a pole to that corner
                // But the last road is also a corner, it will have it's own pole so we shouldn't generate new one.
                if (lastRoad.GetComponent<Road>().RoadType == RoadType.Straight)
                {
                    var pole = Instantiate(PolePrefab, generatedRoad.transform.GetComponent<Road>().PolePosition);
                    pole.transform.localPosition = Vector3.zero;
                    Poles.Add(pole);
                }
                
            }

            generatedRoad.transform.position = lastRoad.transform.GetComponent<Road>().EndPosition.position +
                                               (lastRoad.transform.GetComponent<Road>().EndPosition.forward * 5);

            Roads.Add(generatedRoad);
        }
        
        /* Finish zone generation    */
        var finishGO = Instantiate(FinishPrefab);
        finishGO.transform.forward = Roads[Roads.Count - 1].GetComponent<Road>().EndPosition.forward;
        finishGO.transform.position = Roads[Roads.Count - 1].transform.GetComponent<Road>().EndPosition.position +
                                      (Roads[Roads.Count - 1].transform.GetComponent<Road>().EndPosition.forward * 5);
    }


    /// <summary>
    /// Returns the closest pole to the player.
    /// </summary>
    /// <param name="source"></param>
    /// <returns>Returns GameObject of the closest pole</returns>
    public GameObject GetClosestPole(Transform source)
    {
        var index = -1;
        var minDistance = float.MaxValue;
        for (var i = 0; i < Poles.Count; i++)
        {
            var pole = Poles[i];
            var distance = Vector3.Distance(source.position, pole.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                index = i;
            }
        }
        
        return index >= 0 ? Poles[index] : null;
    }


}
