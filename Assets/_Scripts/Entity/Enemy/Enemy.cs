using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ===== ENEMY CLASS =====
public class Enemy : Entity
{
    [Header("Enemy Stats")]
    [field: SerializeField] public EnemyConfig Config { get; private set; }
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;
    public float attackRange = 2f;
    public float detectionRange = 15f;
    public float chaseSpeed = 3.5f;
    public float chaseDuration = 5f;

    public float _checkForTargetTime = 5;
    private float _targetCheckTimer;
    private float _initTimer;

    public Transform Target { get; private set; }

    private float lastAttackTime;
    private float chaseStartTime;
    private bool isChasing;
    private SwarmAgent swarmAgent;

    public bool IsChasing => isChasing;
    public bool CanAttack => Time.time >= lastAttackTime + attackCooldown;

    protected override void OnAwake()
    {
        base.OnAwake();
        swarmAgent = GetComponent<SwarmAgent>();
    }

    protected override void OnStart()
    {
        base.OnStart();
        _targetCheckTimer = _checkForTargetTime;
    }

    protected override void Update()
    {
        base.Update();

        if (_initTimer < 5)
            return;

        _targetCheckTimer += Time.deltaTime;

        if (_targetCheckTimer >= _checkForTargetTime)
        {
            _targetCheckTimer = 0f;
            Target = GetPlayerTarget();
        }

        if (Target == null)
            return;

        float distanceToTarget = Vector3.Distance(transform.position, Target.position);

        if (!isChasing && distanceToTarget <= detectionRange)
            StartChase();

        if (isChasing && Time.time >= chaseStartTime + chaseDuration)
            StopChase();

        if (isChasing)
            if (distanceToTarget <= attackRange)
                if (CanAttack)
                    Attack();
    }

    private Transform GetPlayerTarget()
    {
        List<Player> players = Physics
            .OverlapSphere(transform.position, detectionRange)
            .Select(c => c.GetComponent<Player>())
            .ToList();

        if (players == null)
            return null;

        if (players.Count > 1)
            return GetClosestPlayer(players).transform;

        return players.First().transform;
    }

    private Player GetClosestPlayer(IEnumerable<Player> players)
    {
        return players
            .OrderBy(player => Vector3.Distance(transform.position, player.transform.position))
            .FirstOrDefault();
    }

    public void StartChase()
    {
        isChasing = true;
        chaseStartTime = Time.time;
    }

    public void StopChase()
        => isChasing = false;

    private void Attack()
    {
        lastAttackTime = Time.time;

        Vector3 directionToTarget = (Target.position - transform.position).normalized;
        Rotatable?.Rotate(directionToTarget, RotationConfig.RotationSpeed);

        IDamageable targetDamageable = Target.GetComponent<IDamageable>();
        targetDamageable?.TakeDamage(new Damage() { Amount = attackDamage });

        Debug.Log($"{gameObject.name} attacked for {attackDamage} damage!");
    }

    protected override void OnDemise(Damage damage)
    {
        base.OnDemise(damage);

        if (swarmAgent != null && swarmAgent.SwarmManager != null)
            swarmAgent.SwarmManager.RemoveAgent(swarmAgent);

        Destroy(gameObject);
    }
}