using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockroachSpawner : MonoBehaviour {

    [SerializeField]
    private GameObject EnemyPrefab;

    [SerializeField]
    private float SpawnInterval;

    [SerializeField]
    private Transform Parent;

    private List<GameObject> Enemies;

    private static CockroachSpawner Inst;

    private CockroachSpawner() {
        Enemies = new List<GameObject>();
    }

    protected void Awake() {
        Inst = this;
    }

    public static CockroachSpawner GetCockroachSpawner() {
        return Inst;
    }

    public void Spawn(int count, Vector3 location) {
        for (int i = 0; i != count; i++) {
            Spawn(location);
        }
    }

    public void Spawn(int count) {
        for (int i = 0; i != count; i++) {
            Spawn(new Vector3(
                Random.Range(-10.0f, 10.0f),
                1.15f,
                Random.Range(-10.0f, 10.0f)
            ));
        }
    }

    public void Spawn(Vector3 location) {
        GameObject enemy = Instantiate(
            EnemyPrefab,
            location,
            Quaternion.identity,
            Parent
        );
        Enemies.Add(enemy);
    }

    public void SetSpawnInterval(float SpawnInterval) {
        this.SpawnInterval = SpawnInterval;
    }

    public float GetSpawnInterval() {
        return SpawnInterval;
    }

    public void SetSpawnIntervalMultiplyBy(float SpawnIntervalModifier) {
        SpawnInterval *= SpawnIntervalModifier;
    }

    public void StartSpawning() {
        StartCoroutine("SpawnCockroach");
    }

    public void StopSpawining() {
        StopCoroutine("SpawnCockroach");
    }

    protected IEnumerator SpawnCockroach() {
        while (isActiveAndEnabled) {
            yield return new WaitForSeconds(SpawnInterval);
            Spawn(1);
        }
    }

    public void RemoveAllEnemies() {
        foreach (GameObject enemy in Enemies) {
            Destroy(enemy);
        }
        Enemies = new List<GameObject>();
    }
}
