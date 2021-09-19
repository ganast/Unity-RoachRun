using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private static UIManager inst;

    [SerializeField]
    private AudioSource UIAudio;

    [SerializeField]
    private AudioClip OhNoSoundEffect;

    [SerializeField]
    private AudioClip GameplayMessageSound;

    [SerializeField]
    private GameObject Spray;

    [SerializeField]
    private SprayController SprayController;

    [SerializeField]
    private Text ScoreText;

    [SerializeField]
    private Text AmmoText;

    [SerializeField]
    private Text TerrorText;

    [SerializeField]
    private Text IntroText;

    [SerializeField]
    private Text GameplayMessageText;

    [SerializeField]
    private GameObject GameplayCanvas;

    [SerializeField]
    private GameObject IntroCanvas;

    [SerializeField]
    private GameObject MainMenuCanvas;

    [SerializeField]
    private GameObject AboutCanvas;

    [SerializeField]
    private GameObject GameOverCanvas;

    private Transform MainMenuPlayerLookTarget;
    private Vector3 MainMenuPlayerOriginalPosition;

    protected void Awake() {
        inst = this;
    }

    protected void Start () {
        MainMenuPlayerLookTarget = GameObject.Find("Books_1_2").transform;
        MainMenuPlayerOriginalPosition = PlayerManager.GetPlayerManager().transform.localPosition;
    }

    // polling-based UI updates...
    protected void Update () {
        
        switch (GameManager.GetGameManager().GetGameState()) {

            case GameManager.GameState.INTRO:
                if (Input.GetKeyUp(KeyCode.Escape)) {
                    StopCoroutine("IntroSequence");
                    GameManager.GetGameManager().MainMenu();
                }
                break;

            case GameManager.GameState.MENUS:
                break;

            case GameManager.GameState.PLAYING:
                AmmoText.text = ((int)SprayController.GetAmmo()).ToString();
                TerrorText.text = PlayerManager.GetPlayerManager().GetTerror().ToString();
                break;
        }
    }

    // notification-based UI update...
    public void UpdateScore(int score) {
        ScoreText.text = score.ToString();
    }

    public static UIManager GetUIManager() {
        return inst;
    }

    public void HideUI() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UIAudio.Stop();
        Spray.SetActive(false);
        GameplayMessageText.text = "";
        IntroCanvas.SetActive(false);
        MainMenuCanvas.SetActive(false);
        AboutCanvas.SetActive(false);
        GameplayCanvas.SetActive(false);
        GameOverCanvas.SetActive(false);
    }

    public void ShowIntroUI() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UIAudio.Stop();
        Spray.SetActive(false);
        GameplayMessageText.text = "";
        IntroCanvas.SetActive(true);
        MainMenuCanvas.SetActive(false);
        AboutCanvas.SetActive(false);
        GameplayCanvas.SetActive(false);
        GameOverCanvas.SetActive(false);
    }

    public void PlayIntroSequence() {
        StartCoroutine("IntroSequence");
    }

    public void ShowMainMenuUI() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (!UIAudio.isPlaying) {
            UIAudio.Play();
        }
        PlayerManager.GetPlayerManager().transform.localPosition = MainMenuPlayerOriginalPosition;
        PlayerManager.GetPlayerManager().LookAt(MainMenuPlayerLookTarget);
        Spray.SetActive(false);
        GameplayMessageText.text = "";
        IntroCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
        AboutCanvas.SetActive(false);
        GameplayCanvas.SetActive(false);
        GameOverCanvas.SetActive(false);
    }

    public void ShowAboutUI() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (!UIAudio.isPlaying) {
            UIAudio.Play();
        }
        PlayerManager.GetPlayerManager().transform.localPosition = MainMenuPlayerOriginalPosition;
        PlayerManager.GetPlayerManager().LookAt(MainMenuPlayerLookTarget);
        Spray.SetActive(false);
        GameplayMessageText.text = "";
        IntroCanvas.SetActive(false);
        MainMenuCanvas.SetActive(false);
        AboutCanvas.SetActive(true);
        GameplayCanvas.SetActive(false);
        GameOverCanvas.SetActive(false);
    }

    public void ShowGameplayUI() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UIAudio.Stop();
        PlayerManager.GetPlayerManager().transform.localPosition = MainMenuPlayerOriginalPosition;
        Spray.SetActive(true);
        ShowGameplayMessage("Go!", 3);
        IntroCanvas.SetActive(false);
        MainMenuCanvas.SetActive(false);
        AboutCanvas.SetActive(false);
        GameplayCanvas.SetActive(true);
        GameOverCanvas.SetActive(false);
    }

    public void ShowGameOverUI() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UIAudio.Play();
        Spray.SetActive(false);
        GameplayMessageText.text = "";
        IntroCanvas.SetActive(false);
        MainMenuCanvas.SetActive(false);
        AboutCanvas.SetActive(false);
        GameplayCanvas.SetActive(false);
        GameOverCanvas.SetActive(true);
    }

    public void ShowGameplayMessage(string text, float duration) {
        UIAudio.PlayOneShot(GameplayMessageSound);
        GameplayMessageText.text = text;
        StartCoroutine(ClearGameplayMessage(duration));
    }

    private IEnumerator ClearGameplayMessage(float duration) {
        yield return new WaitForSeconds(duration);
        GameplayMessageText.text = "";
    }

    private IEnumerator IntroSequence() {

        yield return new WaitForSeconds(4.0f);

        IntroText.text = "Such a beautiful summer evening...";
        yield return new WaitForSeconds(3.0f);

        IntroText.text = "";
        yield return new WaitForSeconds(2.0f);

        IntroText.text = "So good to be able to enjoy it in the comfort of your country home...";
        yield return new WaitForSeconds(3.0f);

        IntroText.text = "";
        yield return new WaitForSeconds(2.0f);

        IntroText.text = "Everything is peaceful, quiet...";
        yield return new WaitForSeconds(3.0f);

        IntroText.text = "";
        yield return new WaitForSeconds(2.0f);

        IntroText.text = "You feel relaxed and secure...";
        yield return new WaitForSeconds(3.0f);

        IntroText.text = "";
        yield return new WaitForSeconds(3.0f);

        UIAudio.panStereo = 1.0f;
        UIAudio.PlayOneShot(OhNoSoundEffect);
        yield return new WaitForSeconds(1.5f);

        IntroText.text = "But wait !!!";
        yield return new WaitForSeconds(1.0f);

        PlayerManager.GetPlayerManager().LookAt(MainMenuPlayerLookTarget);
        yield return new WaitForSeconds(1.0f);

        IntroText.text = "";
        yield return new WaitForSeconds(0.25f);

        IntroText.text = "What was that sound ?!";
        yield return new WaitForSeconds(2.0f);

        IntroText.text = "";
        yield return new WaitForSeconds(1.0f);

        UIAudio.panStereo = 0.0f;
        UIAudio.PlayOneShot(OhNoSoundEffect);
        yield return new WaitForSeconds(1.5f);

        IntroText.text = "Is... is anybody there?";
        yield return new WaitForSeconds(2.0f);

        IntroText.text = "";
        yield return new WaitForSeconds(1.0f);

        IntroText.text = "OH NO! IT IS THE...";
        yield return new WaitForSeconds(1.0f);

        CockroachSpawner.GetCockroachSpawner().Spawn(100, new Vector3(6f, 1.15f, 3.0f));

        // GameObject.Find("Radio").GetComponent<AudioSource>().Stop();
        EnvironmentManager.GetEnvironmentManager().StopRadio();
        UIAudio.Play();

        yield return new WaitForSeconds(4.0f);

        IntroText.text = "";
        yield return new WaitForSeconds(1.0f);

        GameManager.GetGameManager().MainMenu();
    }
}
