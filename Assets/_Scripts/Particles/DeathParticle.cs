using AYellowpaper;
using UnityEngine;

public class DeathParticle : MonoBehaviour
{
    [SerializeField] private InterfaceReference<IDamageable> _damageable;

    [Header("Particle Settings")]
    [SerializeField] private int particleCount = 100;
    [SerializeField] private float particleLifetime = 2f;
    [SerializeField] private float explosionForce = 3f;
    [SerializeField] private float upwardForce = 2f;
    [SerializeField] private Color ashColorStart = new Color(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color ashColorEnd = new Color(0.8f, 0.4f, 0.1f, 0f);
    [SerializeField] private Color emberColor = new Color(1f, 0.5f, 0.1f, 1f);
    [SerializeField] private float emberChance = 0.3f;

    [Header("Particle Sizes")]
    [SerializeField] private float minParticleSize = 0.05f;
    [SerializeField] private float maxParticleSize = 0.2f;

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    private void Awake()
        => CreateParticleSystem();

    private void OnEnable()
        => _damageable.Value.OnDemise += HandleOnDemise;
    private void OnDisable()
    => _damageable.Value.OnDemise -= HandleOnDemise;

    private void HandleOnDemise(Damage damage)
        => TriggerDeathEffect();

    private void CreateParticleSystem()
    {
        GameObject psObject = new GameObject("AshParticles");
        psObject.transform.position = transform.position;
        ps = psObject.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.startLifetime = particleLifetime;
        main.startSpeed = 0;
        main.startSize = 0.1f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = particleCount;
        main.loop = false;

        var emission = ps.emission;
        emission.enabled = false;

        var renderer = psObject.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        renderer.material.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        renderer.material.SetInt("_ZWrite", 0);

        Destroy(psObject, particleLifetime + 1f);
    }

    private void TriggerDeathEffect()
    {
        particles = new ParticleSystem.Particle[particleCount];
        Vector3 center = transform.position;

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * 0.5f;
            Vector3 randomVel = Random.insideUnitSphere * explosionForce;
            randomVel.y += upwardForce;

            bool isEmber = Random.value < emberChance;

            particles[i].position = randomPos;
            particles[i].velocity = randomVel;
            particles[i].startLifetime = particleLifetime;
            particles[i].remainingLifetime = particleLifetime;
            particles[i].startSize = Random.Range(minParticleSize, maxParticleSize);
            particles[i].startColor = isEmber ? emberColor : ashColorStart;
            particles[i].rotation = Random.Range(0f, 360f);
            particles[i].angularVelocity = Random.Range(-180f, 180f);
        }

        ps.SetParticles(particles, particleCount);

        // Hide original object
        if (GetComponent<Renderer>())
            GetComponent<Renderer>().enabled = false;

        StartCoroutine(UpdateParticles());

        Destroy(gameObject, particleLifetime + 1f);
    }

    private System.Collections.IEnumerator UpdateParticles()
    {
        float elapsed = 0;

        while (elapsed < particleLifetime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / particleLifetime;

            int numAlive = ps.GetParticles(particles);

            for (int i = 0; i < numAlive; i++)
            {
                // Fade to orange/transparent
                particles[i].startColor = Color.Lerp(
                    particles[i].startColor.r > 0.8f ? emberColor : ashColorStart,
                    ashColorEnd,
                    t
                );

                // Shrink over time
                particles[i].startSize *= 0.98f;

                // Add gravity and drag
                Vector3 vel = particles[i].velocity;
                vel.y -= 9.81f * Time.deltaTime * 0.3f;
                vel *= 0.98f;
                particles[i].velocity = vel;
            }

            ps.SetParticles(particles, numAlive);

            yield return null;
        }
    }
}