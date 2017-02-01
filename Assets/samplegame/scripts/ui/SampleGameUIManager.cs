using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Tangible.Shared;

public class SampleGameUIManager : UIManager {
	
	public static SampleGameUIManager instance { private set; get; }

	[SerializeField]
	protected BuildVersionButton buildVersionButton_;

	public BuildVersionButton BuildVersionButton {
		get { return buildVersionButton_; }
	}

	[SerializeField]
	protected RectTransform gameRoot_;
	
	public RectTransform gameRoot {
		get { return gameRoot_; }
	}

	// holder for things that need to be above debug solve and below scrim
	[SerializeField]
	protected RectTransform higherGameRoot_;
	
	public RectTransform higherGameRoot {
		get { return higherGameRoot_; }
	}
	
	private void Awake() {
		instance = this;
	
		HideScrim ();

		base.Init();
	}
	
	private void OnDestroy() {
		if (instance == this) {
			instance = null;
		}
	}
		
	// =================================================== SCRIM
	
	[SerializeField]
	private Image scrim;

	[SerializeField]
	private Color scrim_color_ = new Color(1.0f, 1.0f, 1.0f, 0.8f);

	[SerializeField]
	private Color scrim_opaque_color_ = Color.white;

	private bool scrimVisible = false;
	private bool scrimOpaque = false;

	public bool input_blocked {
		get { 
			return scrimVisible || (curDialog != null && curDialog.dialogInfo.blockInput) || AccountProfileWidget.WidgetOpen;
		}
	}

	public void UpdateScrim(bool visible, bool opaque = false) {
		if (visible != scrimVisible || opaque != scrimOpaque) {
			
			Go.killAllTweensWithTarget(scrim);
			
			// if opaque make the color change immediately since we want to make sure to completely hide what's underneath
			// and we don't want what's added above to look weird with what we're trying to cover up during the transition
			if (visible) {
				if (opaque) {
					scrim.color = scrim_opaque_color_;
				} else {
					Go.to(scrim, 0.5f, new GoTweenConfig().colorProp("color", scrim_color_));
				}
				scrimOpaque = opaque;
			} else {
				Color hideColor = TweenHelper.FadedColor(scrim_color_, 0.0f);
				if (scrimOpaque) {
					scrim.color = hideColor;
				} else {
					Go.to(scrim, 0.5f, new GoTweenConfig().colorProp("color", hideColor));
				}
			}

            scrimVisible = visible;

			CanvasGroup group = scrim.GetComponent<CanvasGroup>();
			if (group != null) {
				group.blocksRaycasts = visible;
			}
		}
	}
	
	public void ShowScrim(bool opaque = false) {
		UpdateScrim(true, opaque);
	}
	
	public void HideScrim() {
		UpdateScrim(false);
	}
}
