using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockGlobals : MonoBehaviour
{
    public float DISTANCE = 2.5f;
    public float OBSTACLEDISTANCE = 2.5f;

    public float flockSpeed = 5f;
    public GameObject agentPrefab; 

    [Range(0f, 4000f)] public float cohesiveForce = 4;
    [Range(0f, 8000f)] public float alignForce = 1;
    [Range(0f, 4000f)] public float separationForce = 1;
    [Range(0f, 4000f)] public float obstacle_separationForce = 1;
    [Range(0f, 4000f)] public float leaderForce = 4;
    [Range(0f, 4000f)] public float radIn = 4;


    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
