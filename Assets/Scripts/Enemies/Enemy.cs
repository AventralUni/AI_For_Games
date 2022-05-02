using System.Collections.Generic;
using UnityEngine;

public class Enemy : GameBehavior
{
    [SerializeField]
    Transform model = default;

    EnemyFactory originFactory;

    public FlockGlobals g;
    public List<GameObject> flock = new List<GameObject>();
    bool hadFlock;

    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    Direction direction;
    DirectionChange directionChange;
    float directionAngleFrom, directionAngleTo;
    float progress, progressFactor;
    float pathOffset;
    float speed;
    EnemyType enemyType;

    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory!");
            originFactory = value;
        }
    }

    public float Scale { get; private set; }

    float Health { get; set; }

    public void ApplyDamage(float damage)
    {
        Debug.Assert(damage >= 0f, "Negative damage applied.");

        if (flock.Count > 0)
        {
            int e = flock.Count - 1;
            flock[e].GetComponent<FlockAgent>().Health -= damage;
            return;
        }

        Health -= damage;
    }

    public void OnDestroy()
    {
        if (hadFlock)
        {
            g.flockCount -= 1;
            for (int i = 0; i < flock.Count; i++)
            {
                Destroy(flock[i].gameObject);
            }
        }
    }

    public override bool GameUpdate()
    {
        if (Health <= 0f)
        {
            Recycle();
            return false;
        }

        for (int i = 0; i < flock.Count; i++)
        {
            if (flock[i].GetComponent<FlockAgent>().Health <= 0)
            {
                Destroy(flock[i].gameObject);
                flock.RemoveAt(i);
            }
        }

        progress += Time.deltaTime * progressFactor;
        while (progress >= 1f)
        {
            if (tileTo == null)
            {
                Game.EnemyReachedDestination();
                Recycle();
                return false;
            }
            progress = (progress - 1f) / progressFactor;
            PrepareNextState();
            progress *= progressFactor;
        }
        if (directionChange == DirectionChange.None)
        {
            transform.localPosition =
                Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        }
        else
        {
            float angle = Mathf.LerpUnclamped(
                directionAngleFrom, directionAngleTo, progress
            );
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        return true;
    }

    public override void Recycle()
    {
        OriginFactory.Reclaim(this);
    }

    public void Initialize(
        float scale, float speed, float pathOffset, float health, EnemyType type
    )
    {
        Scale = scale;
        model.localScale = new Vector3(scale, scale, scale);
        this.speed = speed;
        this.pathOffset = pathOffset;
        Health = health;
        enemyType = type;
    }

    public void SpawnOn(GameTile tile)
    {
        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        progress = 0f;
        PrepareIntro();

        g = GameObject.FindGameObjectWithTag("FlockControl").GetComponent<FlockGlobals>();

        if (enemyType == EnemyType.Small) return;
        if (g.flockCount > 12) return;

        var rand = Random.Range(0, 2);
        if (rand != 1) return;

        for (int i = 0; i < 12; i++)
        {
            GameObject agent = Instantiate(g.agentPrefab, tile.transform.position, Quaternion.identity);
            var flockagent = agent.GetComponent<FlockAgent>();
            flockagent.leader = gameObject;

            Color col;
            if (transform.GetComponentInChildren<MeshRenderer>().sharedMaterial)
                col = transform.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;
            else
                col = Color.magenta;

            agent.GetComponentInChildren<SpriteRenderer>().color = col;

            flockagent.flockID = g.flockCount;
            flockagent.driveSpeed = speed;

            flock.Add(agent);
        }

        hadFlock = true;
        g.flockCount++;
    }

    void PrepareNextState()
    {
        tileFrom = tileTo;
        tileTo = tileTo.NextTileOnPath;
        positionFrom = positionTo;
        if (tileTo == null)
        {
            PrepareOutro();
            return;
        }
        positionTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
        direction = tileFrom.PathDirection;
        directionAngleFrom = directionAngleTo;
        switch (directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
    }

    void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, 0f);
        progressFactor = speed;
    }

    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
        model.localPosition = new Vector3(pathOffset - 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f - pathOffset));
    }

    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90f;
        model.localPosition = new Vector3(pathOffset + 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f + pathOffset));
    }

    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + (pathOffset < 0f ? 180f : -180f);
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localPosition = positionFrom;
        progressFactor =
            speed / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffset), 0.2f));
    }

    void PrepareIntro()
    {
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f * speed;
    }

    void PrepareOutro()
    {
        positionTo = tileFrom.transform.localPosition;
        directionChange = DirectionChange.None;
        directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f * speed;
    }
}