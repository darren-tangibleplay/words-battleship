using UnityEngine;

public class StartUpState : GameState {
	public const string STATE_NAME = "start_up";

	Splash splash_;

	public StartUpState() : base(STATE_NAME, false) {

	}

	override public void OnPush() {
		base.OnPush();

		if (splash_ == null) {
			splash_ = (GameObject.Instantiate (Resources.Load("prefabs/splash_screen") as GameObject) as GameObject).GetComponent<Splash>();
			splash_.Init(OnSplashComplete);
		}
	}
	
	override public void OnEnter() {
		base.OnEnter();
	}
	
	override public void OnExit() {
		base.OnExit();
	}
	
	override public void OnPop() {
		if (splash_ != null) {
			splash_.AnimateOut(); // It will self destruct
			splash_ = null;
		}

		base.OnPop();
	}

	public void OnSplashComplete(Splash.ExitType exit_type) {
		//Game.instance.LoadFromFile();
		
		// go straight to auto-setup, skip the have hardware stuff.
		Game.StateMachine.PopState(name);
        Game.StateMachine.PushState(new SetupState());
		
		//Game.instance.SaveToFile();
	}
}