using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerManager : MonoBehaviour {

    private float Terror;

    private Transform LookTarget;

    private static PlayerManager inst;

    private FirstPersonController fpsController;

    private SprayController sprayController;

    private PlayerManager() {
        LookTarget = null;
    }

    public void Awake() {
        fpsController = GetComponent<FirstPersonController>();
        sprayController = GetComponentInChildren<SprayController>(true);
        inst = this;
    }

    public void Start () {
    }

    public void Update () {
        if (LookTarget != null) {
            transform.LookAt(LookTarget);
            LookTarget = null;
        }
	}

    public void LookAt(Transform LookTarget) {
        this.LookTarget = LookTarget;
    }

    public void StopLookingAt() {
        LookTarget = null;
    }

    private void OnTriggerEnter(Collider other) {

        switch (GameManager.GetGameManager().GetGameState()) {

            case GameManager.GameState.PLAYING:
                if (other.tag == "Enemy") {
                    if (other.GetComponent<CockroachBehaviour>().IsAlive()) {
                        Terror++;
                    }
                }
                else if (other.tag == "PowerUp") {
                    float bonus = other.GetComponent<PowerUpInfo>().GetBonus();
                    sprayController.LoadAmmo(bonus);
                    UIManager.GetUIManager().ShowGameplayMessage("Power up!", 2);
                    Destroy(other.gameObject);
                }
                break;
        }
    }

    public int GetTerror() {
        return (int) Terror;
    }

    public static PlayerManager GetPlayerManager() {
        return inst;
    }

    public void ReadyPlayerOne() {
        Terror = 0.0f;
        fpsController.enabled = true;
    }

    public void StopPlaying() {
        fpsController.enabled = false;
    }

    public void ReduceTerrorBy(int TerrorBonus) {
        Terror -= TerrorBonus;
        if (Terror < 0.0f) {
            Terror = 0.0f;
        }
    }
}
