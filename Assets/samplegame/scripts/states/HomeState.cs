using System;
using Tangible.Shared;
﻿using UnityEngine;

public class HomeState : GenericUnityMenuState {
	public const string STATE_NAME = "home";

	string levelName_;
	Action onComplete_;
	public HomeState (Action complete) : base(STATE_NAME, "prefabs/main_menu", SampleGameUIManager.instance) {
		onComplete_ = complete;
	}

	override protected void HandleButton(string title){
		switch (title) {
		case "play":
			Game.StateMachine.PopState(HomeState.STATE_NAME);
			Game.StateMachine.PushState(new LevelSelectState(onComplete_, Game.DisplayData.LevelSelected));
			break;
		default:
			Debug.LogError("unknown button title: " + title);
			break;
		}
	}

	override public void OnPush() {
		SampleGameSoundManager.instance.SelectAndPlayBackgroundMusic (STATE_NAME);
		SampleGameSoundManager.instance.PreloadSoundEffects(STATE_NAME);

		Game.enableHome = false;
		base.OnPush();
	}

	override public void OnEnter() {
		base.OnEnter();
		AccountProfileWidget.Show();
		PromotionController.Show();
		SampleGameUIManager.instance.BuildVersionButton.gameObject.SetActive (true);
	}

	override public void OnExit() {
		PromotionController.Hide();
		SampleGameUIManager.instance.BuildVersionButton.gameObject.SetActive (false);
		base.OnExit();
	}

	override public void OnPop() {
		base.OnPop();
		Game.enableHome = true;
	}
}
