using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class LevelComplete : MonoBehaviour {

    public delegate void OnContinue();
    public delegate void OnReplay();

    [SerializeField]
    protected Button continue_;
    
    [SerializeField]
    protected Button replay_;

    protected OnContinue on_continue_;
    protected OnReplay on_replay_;

    public virtual void Init(OnContinue on_continue, OnReplay on_replay) {
        on_continue_ = on_continue;
        on_replay_ = on_replay;

		if (continue_ != null) {
			continue_.onClick.AddListener (OnExit);
		}
		if (replay_ != null) {
			replay_.onClick.AddListener (OnRestart);
		}
    }

    public abstract void DisplayButtons();

    public void OnRestart() {
        SampleGameSoundManager.instance.OnButtonClick();
        AnimateOut(false);
    }
    
    public void OnExit() {
        SampleGameSoundManager.instance.OnButtonClick();
        AnimateOut(true);
    }
    
    void AnimateOut(bool exit) {
        if (exit) { 
            on_continue_();
        } else {
            on_replay_();
        }
    }   

}
