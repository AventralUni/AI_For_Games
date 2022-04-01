using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{
    FlockGlobals g;
    Rigidbody rb;

    Vector3 currentVelocity;
    float agentSmoothTime = 0.5f;

    List<GameObject> getNearAgents()
    {
        var agents = GameObject.FindGameObjectsWithTag("Agent");
        List<GameObject> found = new List<GameObject>();

        //var agents = GameObject.FindGameObjectsWithTag("Agent");
        for (int i = 0; i < agents.Length; i++)
        {
            var other = agents[i];

            if (other == gameObject) continue;

            if (Vector3.Distance(transform.position, other.transform.position) > g.DISTANCE) continue;

            found.Add(other);
        }

        return found;
    }

    List<GameObject> getNearObstacles()
    {
        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        List<GameObject> found = new List<GameObject>();

        for (int i = 0; i < obstacles.Length; i++)
        {
            var other = obstacles[i];
            if (Vector3.Distance(transform.position, other.transform.position) > g.DISTANCE) continue;

            found.Add(other);
        }

        return found;
    }

    void UpdateAI()
    {
        var neighbours = getNearAgents();
        var obstacles = getNearObstacles();

        if (neighbours.Count <= 0) return;

        Vector3 avg_heading_to_leader = Vector3.zero;
        if (g.leader)
        {
            avg_heading_to_leader = (g.leader.transform.position - transform.position).normalized;
        }

        //align
        Vector3 avg_heading = Vector3.zero;
        for (int i = 0; i < neighbours.Count; i++)
        {
            var other = neighbours[i];
            var diff = transform.position - other.transform.position;
            //if (Mathf.Abs(diff.magnitude) > g.DISTANCE * .5f)
            //{
            //    continue;
            //}

            avg_heading += other.transform.forward;
        }
        avg_heading /= neighbours.Count;

        //separation
        Vector3 avg_diff1 = Vector3.zero;
        for (int i = 0; i < neighbours.Count; i++)
        {
            var diff = transform.position - neighbours[i].transform.position;
            if (Mathf.Abs(diff.magnitude) > g.DISTANCE * .5f)
            {
                continue;
            }

            avg_diff1 += diff / diff.sqrMagnitude;
        }
        avg_diff1 /= neighbours.Count;

        //obstacle separation 
        Vector3 avg_diff2 = Vector3.zero;
        for (int i = 0; i < obstacles.Count; i++)
        {
            var diff = transform.position - obstacles[i].transform.position;
            if (Mathf.Abs(diff.magnitude) > g.DISTANCE * 1.5f)
            {
                continue;
            }

            avg_diff2 += diff / diff.sqrMagnitude;
        }
        avg_diff2 /= obstacles.Count;

        //cohesion
        Vector3 avg_position = Vector3.zero;
        for (int i = 0; i < neighbours.Count; i++)
        {
            var diff = transform.position - neighbours[i].transform.position;

            if (Mathf.Abs(diff.magnitude) > g.DISTANCE * 2)
            {
                continue;
            }

            var other = neighbours[i];
            avg_position += other.transform.position;
        }
        avg_position /= neighbours.Count;
        avg_position -= transform.position;

        avg_position = Vector3.SmoothDamp(transform.forward, avg_position, ref currentVelocity, agentSmoothTime);

        var align = avg_heading.normalized;
        var separation = avg_diff1.normalized;
        var cohesion = avg_position.normalized;
        var obstacle_separation = avg_diff2.normalized;

        //print($"{gameObject.name} : algn {align} : fwd {transform.forward}");

        var netSteer =
            cohesion * g.cohesiveForce +
            align * g.alignForce +
            separation * g.separationForce + 
            avg_heading_to_leader * g.leaderForce +
            obstacle_separation * g.obstacle_separationForce;

        Move(netSteer.normalized * g.flockSpeed);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        g = GameObject.FindGameObjectWithTag("AIControl").GetComponent<FlockGlobals>();

        //Time.timeScale = 0.25f;

        transform.Rotate(transform.up * Random.Range(0, 360));
        //Time.timeScale = 0.1f;
    }

    void Update()
    {
        UpdateAI();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, g.DISTANCE);

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
