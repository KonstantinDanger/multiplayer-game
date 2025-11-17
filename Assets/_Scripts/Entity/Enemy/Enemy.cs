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

    [Header("References")]
    public Transform target; // Usually the player

    private float lastAttackTime;
    private float chaseStartTime;
    private bool isChasing;
    private SwarmAgent swarmAgent;

    public bool IsChasing => isChasing;
    public bool CanAttack => Time.time >= lastAttackTime + attackCooldown;

    private void Awake()
        => swarmAgent = GetComponent<SwarmAgent>();

    protected override void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (!isChasing && distanceToTarget <= detectionRange)
            StartChase();

        if (isChasing && Time.time >= chaseStartTime + chaseDuration)
            StopChase();

        if (isChasing)
            if (distanceToTarget <= attackRange)
                if (CanAttack)
                    Attack();
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

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Rotatable?.Rotate(directionToTarget, RotationConfig.RotationSpeed);

        // Deal damage if target has IDamageable
        IDamageable targetDamageable = target.GetComponent<IDamageable>();
        targetDamageable?.TakeDamage(new Damage() { Amount = attackDamage });

        // Visual/audio feedback can be added here
        Debug.Log($"{gameObject.name} attacked for {attackDamage} damage!");
    }

    public void Die()
    {
        // Notify swarm manager
        if (swarmAgent != null && swarmAgent.SwarmManager != null)
        {
            swarmAgent.SwarmManager.RemoveAgent(swarmAgent);
        }

        Destroy(gameObject);
    }
}