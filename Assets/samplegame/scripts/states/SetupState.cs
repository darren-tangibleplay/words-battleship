using UnityEngine;

public class SetupState : GameState {
	public const string STATE_NAME = "normal_setup";

	public enum ExitType {
		OK,
		SKIP,
	};


	private SetupController setup_flow_;

	public SetupState () : base(STATE_NAME, true) {

	}

    void CleanUp() {
		GameObject.Destroy(setup_flow_.gameObject);
	}

	override public void OnPush() {
		base.OnPush();

		setup_flow_ = (GameObject.Instantiate(Resources.Load("prefabs/Setup") as GameObject, parent: SampleGameUIManager.instance.root, worldPositionStays: false) as GameObject).GetComponent<SetupController>();
		setup_flow_.Init(OnAllDone, true, SampleGameUIManager.instance, "prefabs/have_special_tiles", SetupController.PlaySurfaceDetection.BoardOrPaper);
	}

	public void OnAllDone(SetupQuality setupQuality) {
        OnSetupComplete(ExitType.OK);
    }

	override public void OnEnter() {
		base.OnEnter();
	}

	override public void OnExit() {
		base.OnExit();
	}

	override public void OnPop() {
        CleanUp();

		base.OnPop();
	}

	public override void Update(float delta) {
		base.Update(delta);
	}

	public override void TangibleUpdate() {
		base.TangibleUpdate();
	}

	void OnSetupComplete(ExitType exit_type) {
		ServerFeedDownloader.instance.Init();
        Game.singleton.GoToMainMenuState();
	}
}