using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{
    public static int count = 0;
    FlockGlobals g;

    public int flockID;
    public GameObject leader;

    float agentSmoothTime = 0.5f;
    Vector3 currentVelocity;

    public float Health = 1;
    public float rayLength = 1;
    public float driveSpeed = 1;

    [SerializeField] bool colliding = false;

    List<GameObject> getNearAgents()
    {
        var agents = GameObject.FindGameObjectsWithTag("Agent");
        List<GameObject> found = new List<GameObject>();

        for (int i = 0; i < agents.Length; i++)
        {
            var other = agents[i];

            if (other == gameObject || other.GetComponent<FlockAgent>().flockID != flockID) continue;
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
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(other.transform.position.x, other.transform.position.z)) > g.DISTANCE) continue;

            found.Add(other);
        }

        return found;
    }

    void UpdateAI()
    {
        var obstacles = getNearObstacles();

        //obstacle separation
        Vector3 avg_diff2 = Vector3.zero;
        for (int i = 0; i < obstacles.Count; i++)
        {
            var diff = transform.position - obstacles[i].transform.position;
            if (Mathf.Abs(diff.magnitude) > g.OBSTACLEDISTANCE * 1f)
            {
                continue;
            }

            avg_diff2 += diff / diff.sqrMagnitude;
        }
        avg_diff2 /= obstacles.Count;

        Vector3 avg_heading_to_leader = Vector3.zero;
        if (leader)
        {
            var diff = leader.transform.position - transform.position;

            avg_heading_to_leader = diff.normalized;
        }

        //alone
        var neighbours = getNearAgents();
        if (neighbours.Count <= 0)
        {
            Move((avg_heading_to_leader * g.leaderForce + avg_diff2.normalized * g.obstacle_separationForce).normalized * g.flockSpeed);
            return;
        }

        //align
        Vector3 avg_heading = Vector3.zero;
        for (int i = 0; i < neighbours.Count; i++)
        {
            var other = neighbours[i];
            var diff = transform.position - other.transform.position;

            avg_heading += other.transform.forward;
        }
        avg_heading /= neighbours.Count;

        //separation
        Vector3 avg_diff1 = Vector3.zero;
        for (int i = 0; i < neighbours.Count; i++)
        {
            var diff = transform.position - neighbours[i].transform.position;
            if (Mathf.Abs(diff.magnitude) > g.DISTANCE * .75f)
            {
                continue;
            }

            avg_diff1 += diff / diff.sqrMagnitude;
        }
        avg_diff1 /= neighbours.Count;

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

        Vector3 center = leader.transform.position;
        float radius = 1;

        Vector3 coffset = center - transform.position;
        float t = coffset.magnitude / radius;
        var radiusin = (t < .9) ? Vector3.zero : coffset * t * t;

        var netSteer =
            cohesion * g.cohesiveForce +
            align * g.alignForce +
            separation * g.separationForce +
            avg_heading_to_leader * g.leaderForce +
            obstacle_separation * g.obstacle_separationForce;

        netSteer += radiusin.normalized * g.radIn;

        Move(netSteer.normalized * driveSpeed);
    }

    void Start()
    {
        g = GameObject.FindGameObjectWithTag("FlockControl").GetComponent<FlockGlobals>();
        transform.Rotate(transform.up * Random.Range(0, 360));
    }

    void Update()
    {
        UpdateAI();
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
    }
    void Move(Vector3 velocity)
    {
        transform.forward = velocity;
        transform.position += velocity * Time.deltaTime;
    }
}
