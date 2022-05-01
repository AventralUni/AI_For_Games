using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDebug : MonoBehaviour
{
    BoxCollider2D bc;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //var e = GetComponent<Collider>().bounds;

        //var p2 = new Vector3(e.max.x, e.center.y, e.max.z);
        //Gizmos.DrawSphere(p2, 3f);
    }
}
