using System;
using UnityEngine;

public class ResetConfirmationState : GenericUnityMenuState {
	public const string STATE_NAME = "reset_confirm";

	public ResetConfirmationState () : base(STATE_NAME, "prefabs/reset_confirmation", SampleGameUIManager.instance) {

	}
	
	override protected void HandleButton(string title){
		switch (title) {
		case "erase":
			Game.StateMachine.PopState(ResetConfirmationState.STATE_NAME);
			Game.StateMachine.PopState(SettingsState.STATE_NAME);
			Game.singleton.OnResetSaveButton();
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
