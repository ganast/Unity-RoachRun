using System.Collections.Generic;
using UnityEngine;

public class SprayController : MonoBehaviour {

    private ParticleSystem spray;
    private AudioSource aud;
    private CapsuleCollider coll;

    [SerializeField]
    private float APS = 1.0f;

    [SerializeField]
    private float Ammo = 100.0f;

    private bool IsFiring;

    private List<CockroachBehaviour> RoachesUnderAttack;

    public SprayController() {
        RoachesUnderAttack = new List<CockroachBehaviour>();
    }

    protected void Start () {

        IsFiring = false;

        spray = GetComponent<ParticleSystem>();
        spray.Stop();

        aud = GetComponent<AudioSource>();

        coll = GetComponent<CapsuleCollider>();
        coll.enabled = false;
    }

    protected void Update () {

        switch (GameManager.GetGameManager().GetGameState()) {

            case GameManager.GameState.PLAYING:

                if (Input.GetMouseButtonDown(0) && Ammo > 0.0f) {
                    StartSpraying();
                }

                else if (Input.GetMouseButtonUp(0)) {
                    StopSpraying();
                }

                if (IsFiring) {
                    Ammo -= APS * Time.deltaTime;
                    if (Ammo < 0.0f) {
                        Ammo = 0.0f;
                        StopSpraying();
                    }
                }

                break;
        }
    }

    protected void OnTriggerEnter(Collider other) {

        switch (GameManager.GetGameManager().GetGameState()) {

            case GameManager.GameState.PLAYING:

                if (other.tag.Equals("Enemy")) {

                    CockroachBehaviour roach = other.gameObject.GetComponent<CockroachBehaviour>();

                    roach.StartGettingDamage();

                    RoachesUnderAttack.Add(roach);
                }
                break;
        }
    }

    protected void OnTriggerExit(Collider other) {

        switch (GameManager.GetGameManager().GetGameState()) {

            case GameManager.GameState.PLAYING:
                if (other.tag.Equals("Enemy")) {

                    CockroachBehaviour roach = other.gameObject.GetComponent<CockroachBehaviour>();

                    roach.StopGettingDamage();
                }
                break;
        }
    }

    public float GetAmmo() {
        return Ammo;
    }

    public void StartSpraying() {
        coll.enabled = true;
        spray.Play(false);
        aud.Play();
        IsFiring = true;
    }

    public void StopSpraying() {
        IsFiring = false;
        spray.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        aud.Stop();
        coll.enabled = false;
        foreach (CockroachBehaviour roach in RoachesUnderAttack) {
            roach.StopGettingDamage();
        }
        RoachesUnderAttack = new List<CockroachBehaviour>();
    }

    public void Reload() {
        Ammo = 100.0f;
    }

    public void LoadAmmo(float AmmoToLoad) {
        Ammo += AmmoToLoad;
        if (Ammo > 100.0f) {
            Ammo = 100.0f;
        }
    }
}
