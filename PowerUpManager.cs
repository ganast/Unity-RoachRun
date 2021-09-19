using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {

    [SerializeField]
    private Transform[] Locations;

    [SerializeField]
    private float SpawnInterval = 60.0f;

    [SerializeField]
    private GameObject PickupPrefab;

    private float Elapsed;


	protected void Start () {
        Elapsed = 0.0f;
    }

    protected void Update () {
        Elapsed += Time.deltaTime;
        if (Elapsed >= SpawnInterval) {
            Elapsed = 0.0f;
            SpawnPowerUp();
        }
	}

    protected void SpawnPowerUp() {

        if (Locations.Length == 0) {
            return;
        }

        List<Transform> validLocations = new List<Transform>();
        for (int i = 0; i != Locations.Length; i++) {
            if (Locations[i].childCount == 0) {
                validLocations.Add(Locations[i]);
            }
        }
        if (validLocations.Count != 0) {
            Transform parent = validLocations[Random.Range(0, validLocations.Count)];
            Instantiate(PickupPrefab, parent);
        }
    }
}
