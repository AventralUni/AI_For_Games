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

    public float Health = 10;

    public float rayLength = 1;

    [SerializeField] bool colliding = false;

    List<GameObject> getNearAgents()
    {
        var agents = GameObject.FindGameObjectsWithTag("Agent");
        List<GameObject> found = new List<GameObject>();

        //var agents = GameObject.FindGameObjectsWithTag("Agent");
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

    Vector3 lastSteer = Vector3.zero;
    Vector3 lastdir = Vector3.zero;

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

        ////fucky and make smooth
        if (colliding)
        {
            var dirx = Mathf.Sign(transform.forward.x);
            var diry = Mathf.Sign(transform.forward.z);

            //Move(-lastSteer);
            Move((transform.forward + avg_diff2.normalized) * g.flockSpeed);

            //if (lastdir == Vector3.back || lastdir == Vector3.forward)
            //{
            //    if (dirx > 0)
            //    {
            //        Move((Vector3.right + avg_diff2.normalized) * g.flockSpeed);
            //    }
            //    else
            //    {
            //        Move((Vector3.left + avg_diff2.normalized) * g.flockSpeed);
            //    }
            //}
            //else if (lastdir == Vector3.left || lastdir == Vector3.right)
            //{
            //    if (diry > 0)
            //    {
            //        Move((Vector3.forward + avg_diff2.normalized) * g.flockSpeed);
            //    }
            //    else
            //    {
            //        Move((Vector3.back + avg_diff2.normalized) * g.flockSpeed);
            //    }
            //}
            return;
        }

        float scale = 1;
        //need to turn to leader fast not just accelerate dependng on distance when visible

        Vector3 leader_sep = Vector3.zero;
        Vector3 avg_heading_to_leader = Vector3.zero;
        if (leader)
        {
            var zepoint = leader.transform.position /* - g.leader.transform.forward.normalized*/;
            var diff = zepoint - transform.position;
            scale = (zepoint - transform.position).magnitude / 1;

            if (Mathf.Abs(diff.magnitude) < .3)
            {
                leader_sep += diff / diff.sqrMagnitude;
            }

            avg_heading_to_leader = diff.normalized;
        }

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

        Vector3 centor = leader.transform.position;
        float rad = 1;

        Vector3 coffset = centor - transform.position;
        float t = coffset.magnitude / rad;
        var radiusin = (t < .9) ? Vector3.zero : coffset * t * t;

        //print($"{gameObject.name} : algn {align} : fwd {transform.forward}");

        var netSteer =
            cohesion * g.cohesiveForce +
            align * g.alignForce +
            separation * g.separationForce +
            avg_heading_to_leader * g.leaderForce +
            obstacle_separation * g.obstacle_separationForce;

        if (!tolead)
        {
        }

        netSteer += radiusin.normalized * g.radIn;

        Move(netSteer.normalized * g.flockSpeed);
    }

    RaycastHit hitleft;
    RaycastHit hitmid;
    RaycastHit hitright;
    RaycastHit hitraya;

    [SerializeField] LayerMask lm;
    Bounds bounds;

    void Start()
    {
        count++;
        g = GameObject.FindGameObjectWithTag("AIControl").GetComponent<FlockGlobals>();
        bounds = GetComponent<Collider>().bounds;

        var sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.color = new Color(flockID / 3f, flockID / 3f, flockID / 3f);

        transform.Rotate(transform.up * Random.Range(0, 360));
        Time.timeScale = 1f;
    }

    bool tolead;
    void Update()
    {
        if (Health <= 0 || !leader)
        {
            Destroy(transform.gameObject);
        }

        //RaycastHit hit;
        tolead = Physics.Raycast(transform.position, (transform.position - leader.transform.position).normalized, (transform.position - leader.transform.position).magnitude, LayerMask.GetMask("Obstacles"));

        //SteerFromObstacles();

        if (!tolead)
        {
            //transform.forward = (transform.position - g.leader.transform.position).normalized;
            //Move((transform.forward) * g.flockSpeed);
            colliding = false;
        }

        UpdateAI();
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
    }

    void SteerFromObstacles()
    {
        var p1 = new Vector3(transform.position.x - bounds.size.x / 2, transform.position.y, transform.position.z - bounds.size.z / 2);
        var p2 = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        var p3 = new Vector3(transform.position.x + bounds.size.x / 2, transform.position.y, transform.position.z - bounds.size.z / 2);

        Ray leftray = new Ray(p1, transform.forward);
        Ray midray = new Ray(p2, transform.forward);
        Ray rightray = new Ray(p3, transform.forward);

        colliding = false;
        if (Physics.Raycast(midray, out hitmid, rayLength, LayerMask.GetMask("Obstacles")))
        {
            if (hitmid.transform.CompareTag("Player"))
            {
                return;
            }

            colliding = true;
            var normal = hitmid.normal;
            var point = hitmid.point;
            var dirx = Mathf.Sign(transform.forward.x);
            var diry = Mathf.Sign(transform.forward.z);

            lastdir = normal;

            Ray raya1 = new Ray(hitmid.transform.position + normal, Vector3.Cross(normal, Vector3.up));
            Ray raya2 = new Ray(hitmid.transform.position + normal, -Vector3.Cross(normal, Vector3.up));

            if (Physics.Raycast(raya1, out hitraya, rayLength, LayerMask.GetMask("Obstacles")))
            {
                transform.forward = hitraya.normal;

                return;
            }
            else if (Physics.Raycast(raya2, out hitraya, rayLength, LayerMask.GetMask("Obstacles")))
            {
                transform.forward = hitraya.normal;
                return;
            }

            //fucky and make smooth
            if (normal == Vector3.back)
            {
                if (dirx > 0)
                {
                    transform.forward = Vector3.right;
                }
                else
                {
                    transform.forward = Vector3.left;
                }
            }
            else if (normal == Vector3.forward)
            {
                if (dirx > 0)
                {
                    transform.forward = Vector3.right;
                }
                else
                {
                    transform.forward = Vector3.left;
                }
            }
            else if (normal == Vector3.left)
            {
                if (diry > 0)
                {
                    transform.forward = Vector3.forward;
                }
                else
                {
                    transform.forward = Vector3.back;
                }
            }
            else if (normal == Vector3.right)
            {
                if (diry > 0)
                {
                    transform.forward = Vector3.forward;
                }
                else
                {
                    transform.forward = Vector3.back;
                }
            }

            //Move(transform.forward * g.flockSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        var bounds = GetComponent<Collider>().bounds;

        //var p1 = new Vector3(e.min.x, e.center.y, e.min.z);
        //print($"1 {p1}");
        var p1 = new Vector3(transform.position.x - bounds.size.x / 2, transform.position.y, transform.position.z - bounds.size.z / 2);
        var p2 = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        var p3 = new Vector3(transform.position.x + bounds.size.x / 2, transform.position.y, transform.position.z - bounds.size.z / 2);
        //print($"2 {p2}");
        //var p3 = new Vector3(e.max.x, e.center.y, e.min.z);
        //print($"3 {p3}");

        //Gizmos.DrawWireSphere(p1, .3f);
        //Gizmos.DrawWireSphere(p2, .3f);
        //Gizmos.DrawWireSphere(transform.position, g.DISTANCE);

        //Gizmos.color = (hitleft) ? Color.green : Color.red;
        //Gizmos.DrawRay(p1, transform.forward * 5f);

        //Ray midray = new Ray(p2, transform.forward * 5);
        Ray leftray = new Ray(p1, transform.forward);
        Ray midray = new Ray(p2, transform.forward);
        Ray rightray = new Ray(p3, transform.forward);

        //Gizmos.color = Physics.Raycast(leftray, out hitmid, .4f, LayerMask.GetMask("Obstacles")) ? Color.green : Color.red;
        //Gizmos.DrawRay(p1, transform.forward * .4f);

        //Gizmos.color = Physics.Raycast(rightray, out hitmid, .4f, LayerMask.GetMask("Obstacles")) ? Color.green : Color.red;
        //Gizmos.DrawRay(p3, transform.forward * .4f);
        //Gizmos.DrawSphere(p2, .3f);

        Gizmos.DrawLine(transform.position, leader.transform.position);


        Gizmos.color = Color.red;
        if (Physics.Raycast(midray, out hitmid, rayLength, LayerMask.GetMask("Obstacles")))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(p2, transform.forward * rayLength);
            var normal = hitmid.normal;
            var point = hitmid.point;

            //Gizmos.DrawSphere(point + normal / 2, 3);

            Ray raya = new Ray(point, Vector3.Cross(normal, Vector3.up));

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(hitmid.transform.position + normal, Vector3.Cross(normal, Vector3.up) * 1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(hitmid.transform.position + normal, -Vector3.Cross(normal, Vector3.up) * 1f);

            /*
            var dirx = Mathf.Sign(transform.forward.x);
            var diry = Mathf.Sign(transform.forward.z);

            if (normal == Vector3.back)
            {
                if (dirx > 0)
                {
                    transform.forward = Vector3.right;
                }
                else
                {
                    transform.forward = Vector3.left;
                }
            }
            else if (normal == Vector3.forward)
            {
                if (dirx > 0)
                {
                    transform.forward = Vector3.right;
                }
                else
                {
                    transform.forward = Vector3.left;
                }
            }
            else if (normal == Vector3.left)
            {
                //print(transform.forward);
                if (diry > 0)
                {
                    transform.forward = Vector3.forward;
                }
                else
                {
                    transform.forward = Vector3.back;
                }
            }
            else if (normal == Vector3.right)
            {
                if (diry > 0)
                {
                    transform.forward = Vector3.forward;
                }
                else
                {
                    transform.forward = Vector3.back;
                }
        }
    }
            */
        }

        //Gizmos.color = (hitright) ? Color.green : Color.red;
        //Gizmos.DrawRay(p3, transform.forward * 5f);

        //Gizmos.color = Color.blue;

        //
        //print($"1 {p1} " +
        //    $"2 {p2} " +
        //    $"3 {p3}");

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
