using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CockroachBehaviour : MonoBehaviour {

    public enum CockroachState {
        INITIALIZING,
        NORMAL,
        DEAD
    }

    private CockroachState State;

    private Vector3 target;

    private NavMeshAgent nav;
    private AudioSource aud;
    private Rigidbody rb;

    private bool IsGettingDamage;

    [SerializeField]
    private float HP = 10.0f;

    [SerializeField]
    private float DPS = 10.0f;

    [SerializeField]
    private AudioClip[] RoachSounds;

    [SerializeField]
    private AudioClip DeathSound;

    [SerializeField]
    private float RandomizeTargetEverySeconds = 5.0f;

    [SerializeField]
    private float PlaybackSoundEverySeconds = 0.9f;

    [SerializeField]
    private float SoundPlaybackProbability = 0.5f;

    [Tooltip("If true then the cockroach is moving around as expected." +
        "If false, it does not. Useful for debugging purposes.")]
    [SerializeField]
    private bool IsMoving = true;

    protected void Start () {

        State = CockroachState.INITIALIZING;

        IsGettingDamage = false;

        nav = GetComponent<NavMeshAgent>();
        aud = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        SelectRandomTarget();

        StartCoroutine(RandomizeTarget());
        StartCoroutine(PlaybackRandomSound());

        State = CockroachState.NORMAL;
    }

    protected void Update () {

        switch (State) {

            case CockroachState.INITIALIZING:
                break;

            case CockroachState.NORMAL:

                switch (GameManager.GetGameManager().GetGameState()) {

                    case GameManager.GameState.PLAYING:
                    case GameManager.GameState.INTRO:

                        if (Vector3.Distance(transform.position, target) < 0.5f) {
                            SelectRandomTarget();
                        }
                        else {
                            if (IsMoving) {
                                nav.SetDestination(target);
                            }
                        }

                        if (IsGettingDamage) {
                            HP -= DPS * Time.deltaTime;
                            if (HP <= 0) {
                                TransitionToDeadState();
                            }
                        }
                        break;
                }
                break;

            case CockroachState.DEAD:
                break;
        }
    }

    protected void SelectRandomTarget() {
        target = new Vector3(
            Random.Range(-10.0f, 10.0f),
            1.15f,
            Random.Range(-10.0f, 10.0f)
        );
    }

    protected void OnGUI() {

        if (GameManager.GetGameManager().GetGameState() == GameManager.GameState.PLAYING) {

        switch (State) {

            case CockroachState.NORMAL:
                Camera cam = Camera.current;

                if (cam != null) {

                    Vector3 p = cam.WorldToScreenPoint(transform.position);

                    /*
                    Texture2D tex = new Texture2D(100, 1);
                    for (int i = 0; i != 50; i++) {
                        tex.SetPixel(i, 1, Color.red);
                        tex.SetPixel(i, 2, Color.red);
                        tex.SetPixel(i, 3, Color.red);
                        tex.SetPixel(i, 4, Color.red);
                    }
                    */

                    if (p.z < 5 && p.z > 0 && !Physics.Linecast(transform.position, cam.transform.position)) {
                        /*
                        GUI.DrawTexture(new Rect(p.x - 50, Screen.height - p.y - 5 - 10, 100, 5), tex, ScaleMode.StretchToFill);
                        */
                        GUI.Label(new Rect(p.x - 5, Screen.height - p.y - 20, 30, 20), HP.ToString());
                    }
                }
                break;
        }
        }
    }

    protected IEnumerator RandomizeTarget() {
        while (this.isActiveAndEnabled) {
            yield return new WaitForSeconds(RandomizeTargetEverySeconds);
            SelectRandomTarget();
        }
    }

    protected IEnumerator PlaybackRandomSound() {
        while (this.isActiveAndEnabled) {
            yield return new WaitForSeconds(PlaybackSoundEverySeconds);
            if (Random.value < SoundPlaybackProbability) {
                int i = Random.Range(0, RoachSounds.Length - 1);
                aud.PlayOneShot(RoachSounds[i]);
            }
        }
    }

    protected void TransitionToDeadState() {
        StopAllCoroutines();
        HP = 0;
        // TODO: Play additional death effects...
        nav.isStopped = true;
        nav.enabled = false;
        aud.PlayOneShot(DeathSound);
        rb.AddRelativeForce(Vector3.up * 3.33f, ForceMode.Impulse);
        rb.AddTorque(Vector3.left * 6.67f);
        // notification-based approach to updating GameManager...
        GameManager.GetGameManager().EnemyKilled();
        State = CockroachState.DEAD;
    }

    public void StartGettingDamage() {
        IsGettingDamage = true;
    }

    public void StopGettingDamage() {
        IsGettingDamage = false;
    }

    public bool IsAlive() {
        return State != CockroachState.DEAD;
    }
}
