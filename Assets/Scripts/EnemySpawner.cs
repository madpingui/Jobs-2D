using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; 
    [SerializeField] private int numberToSpawn; 
    [SerializeField] private float spawnRadius = 15f;


    [SerializeField] float moveSpeed = 2.5f;
    [SerializeField] float distanceThreshold = 30f;

    private Transform playerTransform;
    private TransformAccessArray transformAccessArray;
    private EnemyMovementJob enemyMovementJob;
    private JobHandle jobHandle;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        transformAccessArray = new TransformAccessArray(numberToSpawn);

        for (int i = 0; i < numberToSpawn; i++)
        {
            var spawnPosition = Random.insideUnitCircle * spawnRadius;
            var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            transformAccessArray.Add(enemy.transform);
        }
    }

    private void Update()
    {
        enemyMovementJob = new EnemyMovementJob
        {
            playerPosition = playerTransform.position,
            moveSpeed = moveSpeed,
            distanceThreshold = distanceThreshold,
            deltaTime = Time.deltaTime
        };

        jobHandle = enemyMovementJob.Schedule(transformAccessArray);
    }

    private void LateUpdate()
    {
        jobHandle.Complete();
    }

    private void OnDestroy()
    {
        transformAccessArray.Dispose();
    }

    [BurstCompile]
    private struct EnemyMovementJob : IJobParallelForTransform
    {
        public Vector3 playerPosition;
        public float moveSpeed;
        public float distanceThreshold;
        public float deltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 direction = (playerPosition - transform.position).normalized;
            float distance = Vector3.Distance(playerPosition, transform.position);

            if (distance <= distanceThreshold)
            {
                transform.position += -direction * moveSpeed * deltaTime;
            }

            //Uncomment if you want rotation to the player
            //Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
            //transform.rotation = lookRotation;
        }
    }
}

