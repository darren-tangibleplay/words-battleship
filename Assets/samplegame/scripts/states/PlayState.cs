using UnityEngine;
using System;

public class PlayState : GameState {
	public const string STATE_NAME = "play";

	public PlayState(Action complete) : base(STATE_NAME, true) {
	}

	override public void OnPush() {
		base.OnPush();
        Game.enableRestartLevel = true;
        Game.LevelManager.StartLevel(false);
        Game.singleton.OnLevelLoaded();
	}
	
	override public void OnEnter() {
        Game.LevelManager.SetPauseCurrentLevel(false);
		base.OnEnter();
	}
	
	override public void OnExit() {
		Game.LevelManager.HideDialogsForCurrentLevel ();
        Game.LevelManager.SetPauseCurrentLevel(true);
		base.OnExit();
	}
	
	override public void OnPop() {
		base.OnPop();
        Game.enableRestartLevel = false;
        Game.LevelManager.ExitGame();
        Game.singleton.OnLevelUnloaded();
    }
	
	public override void Update(float delta) {
		base.Update(delta);
	}
	
	public override void TangibleUpdate() {
		base.TangibleUpdate();
	}

    public void DebugSolve() {
        if (Game.Level != null) Game.Level.DebugSolve();
    }
}
