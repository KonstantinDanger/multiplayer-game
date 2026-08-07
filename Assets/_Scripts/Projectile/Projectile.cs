using Mirror;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(NetworkTransformReliable))]
public class Projectile : NetworkBehaviour
{
    public ProjectileData Data;

    [SerializeField] private Collider _collider;
    [SerializeField] private float _destroyTime = 10;

    [Header("References")]
    [SerializeReference, SubclassSelector]
    private ProjectileMovementMethod _movementMethod;

    [SerializeReference, SubclassSelector]
    private ProjectileCollisionReaction _collisionReaction;

    public Collider Collider => _collider;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (TryGetComponent(out _collider) && !_collider.isTrigger)
        {
            UnityEngine.Debug.LogError("Collider should be a trigger!");
            _collider.isTrigger = true;
        }
    }
#endif
    public void Initialize(ProjectileData data)
    {
        Data = data;

        _collider.excludeLayers |= (1 << data.Sender.layer);
    }

    private void Start()
        => Invoke(nameof(Destroy), _destroyTime);

    private void Destroy()
        => NetworkServer.Destroy(gameObject);

    private void Update()
    {
        if (_movementMethod.UpdateMethod == MovementUpdate.Common)
            MoveProjectile(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (_movementMethod.UpdateMethod == MovementUpdate.Fixed)
            MoveProjectile(Time.fixedDeltaTime);
    }

    private void MoveProjectile(float deltaTime)
        => _movementMethod.Move(this, Data.Speed * deltaTime * Data.Direction);

    private void OnTriggerEnter(Collider other)
    {
        if (MatchesTeam(other.gameObject))
            return;

        _collisionReaction.Collide(other, this);
    }

    private void OnTriggerStay(Collider other)
    {
        if (MatchesTeam(other.gameObject))
            return;

        _collisionReaction.ContinuousCollide(other, this);
    }

    private bool MatchesTeam(GameObject gameObject)
        => gameObject.TryGetComponent(out Entity entity)
        && entity.TeamId == Data.Damage.TeamId;
}

