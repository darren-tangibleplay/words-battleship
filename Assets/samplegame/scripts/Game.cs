using System;
using System.Collections;
using System.Collections.Generic;
using Tangible;
using Tangible.Game;
﻿using UnityEngine;
using UnityEngine.UI;

public class Game : GameController {
	private const float SAVE_INTERVAL_SECONDS = 60f;

	public static Game singleton { get;private set;}

    #if UNITY_EDITOR
    public static bool saveLoad = true;
    public static bool cheat = true;
    public static bool enableVisionReportUI = false;
    public static bool enableVisionLineHelper = true;
    public static bool enableVisionEventLog = false;
	public static bool enableBackgroundMusic = false;
    #elif QA
    public static bool saveLoad = true;
    public static bool cheat = true;
    public static bool enableVisionReportUI = true;
    public static bool enableVisionLineHelper = false;
    public static bool enableVisionEventLog = false;
	public static bool enableBackgroundMusic = true;
    #elif BETA
    public static bool saveLoad = true;
    public static bool cheat = true;
	public static bool enableVisionReportUI = false;
    public static bool enableVisionLineHelper = false;
    public static bool enableVisionEventLog = false;
	public static bool enableBackgroundMusic = true;
    #else
    public static bool saveLoad = true;
    public static bool cheat = false;
	public static bool enableVisionReportUI = false;
    public static bool enableVisionLineHelper = false;
    public static bool enableVisionEventLog = false;
	public static bool enableBackgroundMusic = true;
    #endif

	public static bool enableHome = true;
	public static bool enableRestartLevel = false;

	[SerializeField]
	Button settings_button_;

    Color settings_button_color_;

	static public bool UseHysteresis {
		get {
			return true;
		}
	}



	enum SettingsButtonState {
		INACTIVE,
		ACTIVE,
		CLICK_TO_SHOW
	};
	SettingsButtonState settings_button_state_ = SettingsButtonState.INACTIVE;

    [SerializeField]
    private LineHelper debug_line_helper_;
    static public LineHelper DebugLineHelper {
        get { return singleton.debug_line_helper_; }
    }

	static public bool EnableLineHelper {
        get { return enableVisionLineHelper; }
	}

        private Controller controller_;

        [SerializeField]
        private OnScreenController on_screen_controller_prefab_;

        [SerializeField]
        private PhysicalController physical_controller_prefab_;

        [SerializeField]
    	public Deck deck;

	private StateMachine state_machine_;
    static public StateMachine StateMachine {
        get {
			if (singleton == null || singleton.state_machine_ == null) {
				return null;
			}
			return singleton.state_machine_;
		}
    }

	SampleGamePlayerState player_state_ = new SampleGamePlayerState();
	SaveData display_data_;

    static public SaveData SaveData { get { return singleton.player_state_.SaveData; } }
    static public SaveData DisplayData { get { return singleton.display_data_; } }

    private TransitionManager transition_manager_;
    static public TransitionManager TransitionManager {
        get { return singleton.transition_manager_; }
    }

    private float initial_orthographic_size_; // stored after initial calculations to get the base size for determining current zoom level
    static public float CameraZoomMultiplier {
        get { return Camera.main.orthographicSize / singleton.initial_orthographic_size_; }
    }

    public int debug_frame_game = 0;
    public int debug_frame_vision = 0;

	private string currentProfileId;
	// store what we've loaded as a sanity check so that automatically saving doesn't somehow manage
	// to save empty data if it tries to save automatically when we haven't loaded first
	private bool playerStateLoaded;
	private string loadedProfileId;

	private bool is_ready_;

	void Awake() {
		singleton = this;

		REST.gameId = SectionId.samplegame;
		REST.clientVersion = OsmoAppHelper.VersionWithBuild;

		settings_button_color_ = settings_button_.image.color;
	}

	override protected void AppStart() {
		base.AppStart();

		RegisterForPush(2);
	}

	override protected void Start() {
		base.Start();

        // Disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

		// If there was per player information stored in PlayerPrefs before accounts was put in
		// we'd migrate them over here, but I don't think there are any for numbers
		// See MigratePlayerPrefs function in Tangram

		player_state_.Init();

        deck = GetComponent<DeckCard>();



        #if UNITY_EDITOR
        // We just one instance of the physical controller
        Instantiate(physical_controller_prefab_);

        OnScreenController onScreenController = Instantiate(on_screen_controller_prefab_) as OnScreenController;
        onScreenController.cardSizeScreen = 64f;
        onScreenController.Init(deck);

        controller_ = onScreenController;
        #else
        PhysicalController physicalController = Instantiate(physical_controller_prefab_) as PhysicalController;
        physicalController.Init(deck);

        controller_ = physicalController;
        #endif

        controller_.Register(processEvent);

        // level_manager_ = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        transition_manager_ = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<TransitionManager>();

        // event_processor_ = gameObject.AddComponent<EventProcessor>();

        state_machine_ = gameObject.AddComponent<StateMachine>();
        state_machine_.onStateChange = HandleStateChange;

        initial_orthographic_size_ = Camera.main.orthographicSize;

		AchievementManager.instance.Init(SectionId.samplegame);

		RemoveUnusedAchievements ();

		// level_manager_.UpdateAchievementGoals ();

		ProfileManager.instance.WhenReady( () => {
			ProfilesReady();
		});

		AddAchievementListeners ();

		UIManager.OnUIButton += OnUIButtonClick;

		RegisterURLDelegate();

		state_machine_.PushState(new StartUpState());
    }

	override protected void OnLowMemoryWarning() {
		SampleGameAssetLoader.instance.UnloadAssets();
	}

	// removing achievements for the list for functionality that is currently disabled
	private void RemoveUnusedAchievements() {

	}

	private void ProfilesReady() {
		currentProfileId = ProfileManager.instance.ActiveProfileId;

		AchievementManager.instance.LoadProgress(currentProfileId);
		LoadPlayerState();

		ProfileManager.instance.onProfileSelect += HandleProfileSelection;
	}

	public void OnUIButtonClick(GameObject obj = null) {
		SampleGameSoundManager.instance.OnButtonClick();
	}

	public void AddAchievementListeners() {
		foreach (Achievement a in AchievementManager.instance.GetAchievements()) {
			a.OnUpdated += ProcessAchievement;
		}

        if (UIManager.overlay != null) {
			AchievementNotifier notifier = UIManager.overlay.GetComponent<AchievementNotifier> ();
    		notifier.Init ();

    		// TODO: open menu, zoom to achievement
    		notifier.OnAchievementTap += JumpToAchievements;
    		notifier.OnAchievementShown += OnAchievementShown;
        }
	}

	void HandleProfileSelection(Profile selected, Profile previous) {
		string selectedId = selected != null ? selected.id : null;
		if (selectedId != currentProfileId) {
			if (currentProfileId != null) {
				Debug.Log("[Game] HandleProfileChange: from: " + currentProfileId + " to: " + selectedId);

				// save previous profile data
				AchievementManager.instance.SaveProgress(currentProfileId);
				SampleGamePlayerState.stateDirty = true;
				SavePlayerState(true);

			}

			currentProfileId = selectedId;

			// load new profile data
			AchievementManager.instance.ResetAchievements();
			AchievementManager.instance.LoadProgress(currentProfileId);

			is_ready_ = false;

			GoToMainMenuState();

			LoadPlayerState();
		}
	}

	private bool saveAchievements_ = false;

	private void ProcessAchievement(Achievement a) {
		OnAchievementsChanged ();

		if (StateMachine.currentState.name == AchievementsState.STATE_NAME) {
			(StateMachine.currentState as AchievementsState).Refresh();
		}
	}

	private void OnAchievementsChanged() {
		if (saveAchievements_ == false) {
			saveAchievements_ = true;
			StartCoroutine(SaveAchievements());
		}
	}

	private IEnumerator SaveAchievements() {
		yield return new WaitForEndOfFrame ();

		if (saveLoad) {
			AchievementManager.instance.SaveProgress (currentProfileId);
		}

		saveAchievements_ = false;
	}

	public void OnResetSaveButton() {
		player_state_.Reset (currentProfileId);
        display_data_ = player_state_.SaveData.DeepCopy();

		// Game.LevelManager.UnlockInitialCells ();

		// clear achievements
		AchievementManager.instance.DeleteProgress(ProfileManager.instance.ActiveAccount!=null?ProfileManager.instance.ActiveAccount.id:"default");
		AchievementManager.instance.ResetAchievements();
		OnAchievementsChanged ();

		GoToMainMenuState(null);
	}

	private void StateReady(string playerId) {
		playerStateLoaded = true;
		loadedProfileId = currentProfileId;

        display_data_ = player_state_.SaveData.DeepCopy();

		// unlock initial cells if they haven't been unlocked yet, need to wait for state to be loaded to do this
		// Game.LevelManager.UnlockInitialCells ();

		is_ready_ = true;

		if (StateMachine.currentState != null && StateMachine.currentState.name != StartUpState.STATE_NAME && StateMachine.currentState.name != SetupState.STATE_NAME) {
			GoToMainMenuState(null);
		}
	}

    public void CheckDisplayDataConsistency() {
        #if UNITY_EDITOR
        if (!Game.SaveData.DeepEquals(Game.DisplayData, true)) {
            Debug.LogError("SaveData is not equal to DisplayData");
        }
        #endif
    }

	public void LoadPlayerState() {
		player_state_.Load(currentProfileId, StateReady);
	}

	public void SavePlayerState(bool force) {
		// make sure we've loaded this person's data first before we save over it to sanity check
		// against automatically saving empty data or another person's data
		if (!playerStateLoaded || loadedProfileId != currentProfileId || player_state_ == null) {
			return;
		}

		if (!force) {
			TimeSpan interval = new TimeSpan(DateTime.UtcNow.Ticks - player_state_.SaveData.last_saved_timestamp);
			if (interval.TotalSeconds < SAVE_INTERVAL_SECONDS) {
				return;
			}
		}

		player_state_.CheckSave (currentProfileId);
	}

	public void HideSettingsButton() {
		settings_button_state_ = SettingsButtonState.INACTIVE;
		Go.killAllTweensWithTarget (settings_button_.image);
		Go.to(settings_button_.image, 0.15f, new GoTweenConfig().colorProp("color", TweenHelper.FadedColor(settings_button_color_, 0)).onComplete(delegate(AbstractGoTween obj) {
			settings_button_.gameObject.SetActive(false);
		}));
	}

	public void ShowSettingsButton() {
		settings_button_state_ = SettingsButtonState.ACTIVE;
		Go.killAllTweensWithTarget (settings_button_.image);
		settings_button_.gameObject.SetActive(true);
		Go.to(settings_button_.image, 0.30f, new GoTweenConfig ().colorProp ("color", settings_button_color_).onComplete(delegate(AbstractGoTween obj) {
			StartCoroutine(FadeOutSettingsButton());
		}));
	}

	IEnumerator FadeOutSettingsButton() {
		// keep settings button active for a while and then slowly fade it out
		yield return new WaitForSeconds(5.0f);

		if (settings_button_state_ == SettingsButtonState.ACTIVE) {
			Go.killAllTweensWithTarget (settings_button_.image);
			settings_button_state_ = SettingsButtonState.CLICK_TO_SHOW;
			Go.to (settings_button_.image, 5.0f, new GoTweenConfig ().colorProp ("color", TweenHelper.FadedColor (settings_button_color_, 0)));
		}
	}

	public void OnSettingsButton() {
		if (settings_button_state_ != SettingsButtonState.INACTIVE) {
			UIManager.HandleButtonClick(settings_button_.gameObject);
			OpenSettingsMenu ();
		}
	}

	public void OpenSettingsMenu () {
		if (state_machine_.currentState.name != SettingsState.STATE_NAME) {
			state_machine_.PushState (new SettingsState ());
		}
	}

	public void OnAchievementShown(Achievement which = null) {
		SampleGameSoundManager.instance.PlaySoundFile(SampleGameSoundManager.ACHIEVEMENT_NAME);
	}

	public void JumpToAchievements(Achievement which = null) {
		OpenAchievements(which);
	}

	public void OpenAchievements(Achievement which = null) {
		// removing achievements state if we're already showing it to avoid weird issues with the back
		// button if you click on an achievement notif when the achievements screen is already showing
		if (StateMachine.currentState.name == AchievementsState.STATE_NAME) {
			StateMachine.PopState(AchievementsState.STATE_NAME);
		}
		StateMachine.PushState (new AchievementsState(which));
	}

	// used for things that should happen when the user clicks anywhere on the screen, triggered
	// currently with a click on the background
	public void OnScreenClick() {
		if (settings_button_state_ == SettingsButtonState.CLICK_TO_SHOW) {
			ShowSettingsButton();
		}
	}

	public void HandleBackButton() {
		state_machine_.PopState(state_machine_.currentState.name);
	}

	public void HandleDebugMode() {
#if UNITY_EDITOR
        enableVisionLineHelper = true;
        enableVisionEventLog = true;
#elif QA
        enableVisionLineHelper = true;
        enableVisionEventLog = true;
#elif BETA
        enableVisionLineHelper = true;
        enableVisionEventLog = true;
#endif
        enableVisionReportUI = true;

        // controller_.SetDebug(enableVisionEventLog);
    }

	public void GoToMainMenuState(string level_name = null) {
        while (state_machine_.currentState != null) {
            state_machine_.PopState(state_machine_.currentState.name);
        }

		if (!is_ready_) {
			state_machine_.PushState (new LoadingState ());
			return;
		}


		state_machine_.PushState (new DependencyState());
	}

	void OnLevelSelected() {
		SampleGameSoundManager.instance.StopBackgroundMusic ();

        // controller_.AnimateToOriginal();
		state_machine_.PushState(new PlayState(OnPlayDone));
	}

	void OnPlayDone() {
		state_machine_.PopState(PlayState.STATE_NAME);
        // state_machine_.PushState(new LevelSelectState(OnLevelSelected, Level.LevelName));
	}

    // this function gets called every time we switch states
    // use it to mute/unmute whether vision is needed
    private void HandleStateChange(GameState current) {
        //Debug.Log("Game state changed to: " + current.name.ToUpper() + " (use_vision " + current.useVisionInput.ToString() + ")");
        controller_.Mute(!current.useVisionInput);

		// check for saving the game on state changes
		SavePlayerState (true);
    }

    public void DebugSolve() {
        if (cheat) {
            if (state_machine_.currentState is SetupState) {
                ((SetupState)state_machine_.currentState).OnAllDone(null);
            } else if (state_machine_.currentState is PlayState) {
                // ((PlayState)state_machine_.currentState).DebugSolve();
            }
        }
    }

	public void OnLevelLoaded() {

	}

    public void OnLevelUnloaded() {

    }

    public void processEvent(VisionEventInput e) {

        List<TangibleObject> filtered;

        #if UNITY_EDITOR
        filtered = Tangible.EventHelper.Copy(e.items);
        #else
        filtered = Tangible.EventHelper.FilterBorder(e.items);
        #endif

        // NOTE (darren): vision SDK start point
        var lettersFound = new List<char>();
		foreach (TangibleObject obj in filtered) {
            lettersFound.Add(obj.letter);
		}
        Tangible.WordsBattleship.Vision.HandleLettersFromVisionEvent(lettersFound);
        // END
    }

	override protected void OnUpdate(float delta) {
		base.OnUpdate (delta);

		SavePlayerState (false);

		debug_frame_game++;

		// trigger a screen click if the user touches the screen and the scrim is not up, in order to determine
		// if the user touches the screen for things like showing the settings button
		if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && !SampleGameUIManager.instance.input_blocked) {
			OnScreenClick();
		}
	}

	override protected void SessionEnd() {
		base.SessionEnd();

		AchievementManager.instance.SaveProgress(currentProfileId);
		AchievementManager.instance.FlushQueue();
		SavePlayerState(true);
	}
}
