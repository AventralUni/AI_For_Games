using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{
    FlockGlobals g;
    Rigidbody rb;

    List<GameObject> getNear()
    {
        var agents = GameObject.FindGameObjectsWithTag("Agent");
        List<GameObject> others = new List<GameObject>();

        for (int i = 0; i < agents.Length; i++)
        {
            var other = agents[i];

            if (other == gameObject) continue;
            if (Vector3.Distance(transform.position, other.transform.position) > g.DISTANCE) continue;

            others.Add(other);
        }

        return others;
    }

    void UpdateAI()
    {
        var neighbours = getNear();

        if (neighbours.Count <= 0) return;

        //heading towards pack leader stuff
        Vector3 avg_heading_to_leader = Vector3.zero;
        if (g.leader)
        {
            avg_heading_to_leader = (g.leader.transform.position - transform.position).normalized;
        }

        //align
        Vector3 avg_heading = Vector3.zero;
        for (int i = 0; i < neighbours.Count; i++)
        {
            var diff = transform.position - neighbours[i].transform.position;
            if (Mathf.Abs(diff.magnitude) > g.DISTANCE / 2)
            {
                continue;
            }

            var other = neighbours[i];
            avg_heading += transform.forward;
        }
        avg_heading /= neighbours.Count;

        //separation
        Vector3 avg_diff = Vector3.zero;
        for (int i = 0; i < neighbours.Count; i++)
        {
            var diff = transform.position - neighbours[i].transform.position;
            if (Mathf.Abs(diff.magnitude) > g.DISTANCE / 2)
            {
                continue;
            }

            avg_diff += diff / diff.sqrMagnitude;
        }
        avg_diff /= neighbours.Count;

        //cohesion
        Vector3 avg_position = Vector3.zero;
        for (int i = 0; i < neighbours.Count; i++)
        {
            var other = neighbours[i];
            avg_position += other.transform.position;
        }
        avg_position /= neighbours.Count;
        avg_position -= transform.position;

        var align = avg_heading.normalized;
        var separation = avg_diff.normalized;
        var cohesion = avg_position.normalized;

        var netSteer =
            cohesion * g.cohesiveForce +
            align * g.alignForce +
            separation * g.separationForce + 
            avg_heading_to_leader * g.leaderForce;

        Move(netSteer * g.flockSpeed);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        g = GameObject.FindGameObjectWithTag("AIControl").GetComponent<FlockGlobals>();

        transform.Rotate(transform.up * Random.Range(0, 360));
    }

    void Update()
    {
        UpdateAI();
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, g.DISTANCE);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * .75f);
    }

    void Move(Vector3 velocity)
    {
        // if (gameObject.name == "FLockAI")
        // {
        //     print($"up {transform.up} forward {transform.forward} velocity {velocity}");
        // }

        //rb.velocity = velocity;
        transform.forward = velocity;
        transform.position += velocity * Time.deltaTime;
    }
}
