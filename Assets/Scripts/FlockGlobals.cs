using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockGlobals : MonoBehaviour
{
    public float DISTANCE = 2.5f;
    public float flockSpeed = 5f;
    public GameObject leader;

    [Range(0f, 1f)] public float cohesiveForce = 1;
    [Range(0f, 1f)] public float leaderForce = 1;
    [Range(0f, 1f)] public float alignForce = 1;
    [Range(0f, 1f)] public float separationForce = 1;

    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
