using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class SwarmManager : MonoBehaviour
{
    [Header("Swarm Settings")]
    public int maxSwarmSize = 15;
    public Transform target; // Player reference

    [Header("Spawning")]
    public GameObject enemyPrefab;
    public float spawnRadius = 10f;
    public bool spawnOnStart = true;
    public int initialSpawnCount = 10;

    private List<SwarmAgent> agents = new List<SwarmAgent>();

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnSwarm(initialSpawnCount);
        }
    }

    public void SpawnSwarm(int count)
    {
        count = Mathf.Min(count, maxSwarmSize - agents.Count);

        for (int i = 0; i < count; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = transform.position.y; // Keep on same Y plane

            Vector3 spawnPosition = transform.position + randomOffset;
            GameObject enemyObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
            NetworkServer.Spawn(enemyObj);

            Enemy enemy = enemyObj.GetComponent<Enemy>();
            SwarmAgent agent = enemyObj.GetComponent<SwarmAgent>();

            if (agent != null)
            {
                agent.Initialize(this);
                agents.Add(agent);
            }
        }
    }

    public void RemoveAgent(SwarmAgent agent) => agents.Remove(agent);

    public List<SwarmAgent> GetAgents() => agents;

    public int GetSwarmSize() => agents.Count;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}