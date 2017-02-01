using System;
using UnityEngine;

public class AchievementsState : GenericUnityMenuState {
	public const string STATE_NAME = "achieve";

	private Achievement zoomTo_;

	public AchievementsState (Achievement zoomTo = null) : base(STATE_NAME, "prefabs/achievements_ui", SampleGameUIManager.instance) {
		zoomTo_ = zoomTo;
	}
	
	override protected void HandleButton(string title){
		switch (title) {
		case "back":
			animateBack_ = true;
			Game.singleton.HandleBackButton();
			break;
		default:
			Debug.LogError("unknown button title: " + title);
			break;
		}
	}

	override protected GenericUnityMenu BuildMenu() {
		GenericUnityMenu menu = base.BuildMenu ();

		SampleGameAchievementsUI achievementsUI = menu.gameObject.GetComponent<SampleGameAchievementsUI> ();
		achievementsUI.Init ();
		achievementsUI.OnBack += delegate {
			HandleButton("back");
		};

		return menu;
	}

	public void Refresh() {
		SampleGameAchievementsUI achievementsUI = prefab_.gameObject.GetComponent<SampleGameAchievementsUI> ();
		achievementsUI.UpdateAchievements (zoomTo_);
		// TODO: zoom to appropriate achievement
	}

	override public void OnPush () {
		base.OnPush ();
	}

	override public void OnEnter () {
		base.OnEnter ();

		Game.singleton.HideSettingsButton ();
		SampleGameUIManager.instance.ShowScrim(true);

		Refresh ();
	}

	override public void OnExit () {
		base.OnExit ();

		Game.singleton.ShowSettingsButton();
		SampleGameUIManager.instance.HideScrim ();
	}
}
