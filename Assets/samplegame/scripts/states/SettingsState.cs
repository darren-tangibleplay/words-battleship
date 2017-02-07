using UnityEngine;
using System.Collections;

public class SettingsState : GenericUnityMenuState {
	public const string STATE_NAME = "settings";

	public SettingsState () : base(STATE_NAME, "prefabs/settings_ui", SampleGameUIManager.instance) {
		// nothing?
	}

	override protected void HandleButton(string title){
		// // switch (title) {
		// // case "back":
		// // 	animateBack_ = true;
		// // 	Game.singleton.HandleBackButton();
		// // 	break;
		// // case "home":
		// // 	if (Game.Level != null && Game.Level.have_taken_turn && !Game.Level.level_ended) {
		// // 		Game.StateMachine.PushState (new HomeConfirmationState ());
		// // 	} else {
		// // 		Game.singleton.GoToMainMenuState();
		// // 	}
		// // 	break;
        // // case "restart":
		// // 	Game.StateMachine.PopState(STATE_NAME);
		// // 	if (Game.Level != null) {
		// // 		Game.Level.Restart();
		// // 	}
        // //     break;
        // // case "achieve":
		// // 	Game.singleton.OpenAchievements();
		// // 	break;
        // // case "reset_game_save":
		// // 	Game.StateMachine.PushState (new ResetConfirmationState ());
		// // 	break;
        // // case "report_issue":
		// // 	(prefab_ as SettingsUI).HandleReport();
		// // 	break;
		// // default:
		// // 	Debug.LogError("unknown button title: " + title);
		// // 	break;
		// }
	}

	override public void OnEnter() {
		base.OnEnter();

		Game.singleton.HideSettingsButton();
		SampleGameUIManager.instance.UpdateScrim(true);
	}

	override public void OnExit() {
		base.OnExit();

		Game.singleton.ShowSettingsButton();
		SampleGameUIManager.instance.UpdateScrim(false);
	}

}
