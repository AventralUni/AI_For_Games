using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockGlobals : MonoBehaviour
{
    public float DISTANCE = 2.5f;
    public float flockSpeed = 5f;
    public GameObject leader;

    [Range(0f, 20f)] public float cohesiveForce = 4;
    [Range(0f, 20f)] public float alignForce = 1;
    [Range(0f, 20f)] public float separationForce = 1;
    [Range(0f, 20f)] public float obstacle_separationForce = 1;
    [Range(0f, 20f)] public float leaderForce = 4;

    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
