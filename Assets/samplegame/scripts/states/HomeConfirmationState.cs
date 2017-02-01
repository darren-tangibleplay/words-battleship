using System;
using UnityEngine;

public class HomeConfirmationState : GenericUnityMenuState {
	public const string STATE_NAME = "home_confirm";

	public HomeConfirmationState () : base(STATE_NAME, "prefabs/home_confirmation", SampleGameUIManager.instance) {

	}
	
	override protected void HandleButton(string title){
		switch (title) {
		case "home":
			Game.singleton.GoToMainMenuState();
			break;
		case "cancel":
		case "back":
			animateBack_ = true;
			Game.singleton.HandleBackButton();
			break;
		default:
			Debug.LogError("unknown button title: " + title);
			break;
		}
	}	

	override public void OnEnter () {
		base.OnEnter ();

		Game.singleton.HideSettingsButton();
		SampleGameUIManager.instance.UpdateScrim(true, true);
	}
		
	override public void OnExit () {
		base.OnExit ();

		Game.singleton.ShowSettingsButton();
		SampleGameUIManager.instance.UpdateScrim(false);
	}
}
