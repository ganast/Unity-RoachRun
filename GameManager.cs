using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	// the score...
    private float Score;

	// the number of points gained during the current level...
    private int PointsThisLevel;
    
	// the game state...
	private GameState State;

	// the game can, at any time, be in one of the
	// following states...
    public enum GameState {
        INITIALIZING,
        INTRO,
        MENUS,
        PLAYING,
        GAMEOVER,
        WIN
    }

	// singleton instance...
    private static GameManager inst;

	// reference to the spray controller script...
    [SerializeField]
    private SprayController Spray;

	// number of score points to be awarded to the player per kill...
    [SerializeField]
    private int PointsPerKill = 10;

	// number of terror points to be subtracted from the player per kill...
    [SerializeField]
    private int TerrorBonusPerKill = 10;

	// number of ammo bonus to be awarded to the player per kill...
    [SerializeField]
    private int SprayAmmoBonusPerKill = 5;

	// the level changes every this number of score points gained...
    [SerializeField]
    private int PointsPerLevel = 100;

	// the number of seconds between successive enemy spawns when the game
	// starts...
    [SerializeField]
    private float InitialEnemySpawnInterval = 10.0f;

	// the spawn interval will be modified by this value when the level changes...
    [SerializeField]
    private float EnemySpawnIntervalModifierPerLevel = 0.5f;

	/*** Initialization *******************************************************/
	
	/**
	 * Private constructor, performs basic initialization (including state).
	 */
    private GameManager() {
        inst = null;
        PointsThisLevel = 0;
        State = GameState.INITIALIZING;
    }

	/**
	 * Start is called on the frame when a script is enabled just before any of
	 * the Update methods are called the first time.
	 */
	protected void Start () {
        UIManager.GetUIManager().HideUI();
        // start the game in the intro state...
        Intro();
    }

	/*** Per-frame logic ******************************************************/
	
	/**
	 * Update is called every frame, if the MonoBehaviour is enabled.
	 */
    protected void Update () {

		// as most aspects of the game's implementation, the per-frame behaviour
		// of the game manager depends on game state...
        switch (State) {
        
			// the GameManager is only responsible for the game over mechanic
			// which only applies to the gameplay state
			case GameState.PLAYING:

				// polling-based approach to updating GameManager, check if the
				// player's terror level is above 100...
				if (PlayerManager.GetPlayerManager().GetTerror() >= 100) {
                    
					// in which case, transition to the game over state...
					GameOver();
                }
                break;
        }
	}

	/**** Singleton pattern implementation ************************************/

	/**
	 * Awake is called when the script instance is being loaded.
	 */
    protected void Awake() {
		// initialization of the singleton instance according to Unity's runtime
		// model...
        inst = this;
    }

	/**
	 * Client code access to the singleton instance.
	 */
    public static GameManager GetGameManager() {
		// simply return the singleton instance as it has already been
		// initialized...
        return inst;
    }

	/**** Public API **********************************************************/
	
    /**
	 * Returns the current game state.
	 */
	public GameState GetGameState() {
        return State;
    }

	/*** Event handlers *******************************************************/

	/**
	 * Handles the enemy killed event.
	 */
    public void EnemyKilled() {

		// increase the score according to configuration...
		Score += PointsPerKill;

		// increase the number of points gained during the current level...
        PointsThisLevel += PointsPerKill;

		// award the ammo bonus...
        Spray.LoadAmmo(SprayAmmoBonusPerKill);

		// reduce terror...
        PlayerManager.GetPlayerManager().ReduceTerrorBy(TerrorBonusPerKill);

		// update the player score on the UI...
        UIManager.GetUIManager().UpdateScore((int) Score);

		// check if the level must be increased according to configuration...
        if (PointsThisLevel >= PointsPerLevel) {

			// if so, restart counting points gained during the current level...
            PointsThisLevel = 0;

			// calculate the level to advance to...
            int level = (int) (Score / PointsPerLevel) + 1;

			// update the user interface with a message for the new level...
            UIManager.GetUIManager().ShowGameplayMessage("Level " + level, 3);

			// reconfigure the enemy spawner for the new level...
            CockroachSpawner.GetCockroachSpawner().SetSpawnIntervalMultiplyBy(EnemySpawnIntervalModifierPerLevel);
        }
    }

	/**** State transitions ***************************************************/

    /**
     * Transition to intro state.
     */
    public void Intro() {
        
        // set the state...
        State = GameState.INTRO;
        
        // make player unplayable...
        PlayerManager.GetPlayerManager().StopPlaying();

        // start playing the radio background music...
        EnvironmentManager.GetEnvironmentManager().StartRadio();

        // show the intro UI...
        UIManager.GetUIManager().ShowIntroUI();

        // start the intro sequence...
        UIManager.GetUIManager().PlayIntroSequence();
    }

	/**
	 * Transition to the menus state...
	 */
    public void MainMenu() {

        // set the state...
        State = GameState.MENUS;
        
        // make player unplayable...
        PlayerManager.GetPlayerManager().StopPlaying();

        // stop playing the radio background music...
        EnvironmentManager.GetEnvironmentManager().StopRadio();

		// show the main menu user interface...
        UIManager.GetUIManager().ShowMainMenuUI();
    }

	/**
	 * Transition to the about state...
	 */
    public void About() {

        // set the state...
        State = GameState.MENUS;
        
        // make player unplayable...
        PlayerManager.GetPlayerManager().StopPlaying();

        // stop playing the radio background music...
        EnvironmentManager.GetEnvironmentManager().StopRadio();

		// show the about screen user interface...
        UIManager.GetUIManager().ShowAboutUI();
    }

	/**
	 * Transition to the gameplay state...
	 */
    public void StartGame() {

        // set the state...
        State = GameState.PLAYING;

		// reset the score...
        Score = 0;
        
		// reload ammo...
		Spray.Reload();

		// clear all enemies that may already be in the world (e.g., as part of
		// the intro sequence)...
        CockroachSpawner.GetCockroachSpawner().RemoveAllEnemies();

        // start playing the radio background music...
        EnvironmentManager.GetEnvironmentManager().StartRadio();

		// make the player playable...
        PlayerManager.GetPlayerManager().ReadyPlayerOne();

		// update the player score on the UI...
        UIManager.GetUIManager().UpdateScore((int)Score);

		// show the gameplay user interface...
        UIManager.GetUIManager().ShowGameplayUI();

		// reconfigure the enemy spawner for the new level...
        CockroachSpawner.GetCockroachSpawner().SetSpawnInterval(InitialEnemySpawnInterval);

		// make sure the enemy spawner is indeed spawning enemies...
        CockroachSpawner.GetCockroachSpawner().StartSpawning();
    }

	/**
	 * Transition to the game over state...
	 */
    public void GameOver() {

        // set the state...
        State = GameState.GAMEOVER;

		// show the game over user interface...
        UIManager.GetUIManager().ShowGameOverUI();

        // stop playing the radio background music...
        EnvironmentManager.GetEnvironmentManager().StopRadio();

        // stop spraying (in case the player lost while attacking)...
        Spray.StopSpraying();
        
        // make player unplayable...
        PlayerManager.GetPlayerManager().StopPlaying();

		// stop spawning enemies...
        CockroachSpawner.GetCockroachSpawner().StopSpawining();
    }
}
