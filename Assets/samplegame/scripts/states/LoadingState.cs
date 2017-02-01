using UnityEngine;

public class LoadingState : GenericUnityMenuState {
	public const string STATE_NAME = "loading";


	public LoadingState() : base(STATE_NAME, "prefabs/Loading", SampleGameUIManager.instance) {

	}
		
	override public void OnEnter() {
		base.OnEnter();
		
		Game.singleton.HideSettingsButton();
	}
	
	override public void OnExit() {
		base.OnExit();
		
		Game.singleton.ShowSettingsButton();
	}

	// no buttons to implement
	override protected void HandleButton(string title){
	}
}