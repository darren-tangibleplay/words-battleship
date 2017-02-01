using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SettingsUI : GenericUnityMenu {
	
	private ReportControllerIssueUI report_controller_issue_;
	
	Vector3 top_spacing_;
	Vector3 bottom_spacing_;

    private enum SettingsOption {
		kSettingsOption_Back = 0,
		kSettingsOption_Home,
		kSettingsOption_Restart,
        kSettingsOption_Achievements,
		kSettingsOption_Report,
        kSettingsOption_Reset,
	}

    private HashSet<SettingsOption> top_ = new HashSet<SettingsOption>();
	
	[SerializeField]
	private List<Button> buttons;
	
	[SerializeField]
	private List<Text> labels;
	
	public override void Init() {
		top_spacing_ = buttons[1].transform.localPosition - buttons[0].transform.localPosition;
        bottom_spacing_ = buttons[buttons.Count - 2].transform.localPosition - buttons[buttons.Count - 1].transform.localPosition;

        top_.Add(SettingsOption.kSettingsOption_Back);
        top_.Add(SettingsOption.kSettingsOption_Home);
		top_.Add(SettingsOption.kSettingsOption_Restart);
        top_.Add(SettingsOption.kSettingsOption_Achievements);
	}
	
	public override AbstractGoTween Show() {
		GoTweenFlow flow = (base.Show() as GoTweenFlow);
		
		if (flow != null) {
			
			foreach (Text label in labels) {
                if (label == null) continue;
				Go.killAllTweensWithTarget(label);
			}
			
			foreach (Text label in labels) {
                if (label == null) continue;
                Color target_color = TweenHelper.FadedColor(label.color,1);
                flow.insert(0f, Go.to(label, 1.5f, new GoTweenConfig().colorProp("color", target_color).setEaseType(GoEaseType.QuartOut)));
			}
		}
		
		return flow;
	}
	
	public override AbstractGoTween Hide(bool back = false) {
		GoTweenFlow flow = (base.Hide(back) as GoTweenFlow);
		
		if (flow != null) {
			
			foreach (Text label in labels) {
                if (label == null) continue;
                Go.killAllTweensWithTarget(label);
			}
			
			foreach (Text label in labels) {
                if (label == null) continue;
                Color target_color = TweenHelper.FadedColor(label.color,0);
                flow.insert(0, Go.to(label, 0.2f, new GoTweenConfig().colorProp("color", target_color).setEaseType(GoEaseType.QuartOut)));
            } 
		}
		
		return flow;
	}
	
	override protected void Start () {
		base.Start();

		foreach (Text label in labels) {
            if (label == null) continue;
            label.color = TweenHelper.FadedColor(label.color,0);
		}

		(buttons [(int)SettingsOption.kSettingsOption_Back].gameObject.GetComponent<LongPressButton>()).onLongPress.AddListener(EnterDebug);
	}

	private void EnterDebug() {
		// handle debug like it was a normal button press:
		HandleButtonGeneric (buttons[(int)SettingsOption.kSettingsOption_Back].gameObject);
		
		Game.singleton.HandleDebugMode();
		CheckOptions();
	}
	
	public void HandleReport() {
		if (report_controller_issue_ != null) {
			Destroy(report_controller_issue_.gameObject);
			report_controller_issue_ = null;
		}
		
		report_controller_issue_ = (Instantiate(Resources.Load("prefabs/report_issue_ui") as GameObject) as GameObject).GetComponent<ReportControllerIssueUI>();
		report_controller_issue_.transform.parent = transform;
		report_controller_issue_.transform.position = new Vector3(0, 0, transform.position.z - 100);
		
		report_controller_issue_.items.Add(new ReportControllerIssueUI.ReportItem("Simple Capture", SimpleCapture));
		report_controller_issue_.items.Add(new ReportControllerIssueUI.ReportItem("Expect 1 of each", OnExpectTwoOfEach));
		report_controller_issue_.items.Add(new ReportControllerIssueUI.ReportItem("Expect no card", OnExpectNone));
        report_controller_issue_.items.Add(new ReportControllerIssueUI.ReportItem("Expect no change", OnExpectNoChange));
    }

	private void SimpleCapture() {
		report_controller_issue_.SimpleCapture("samplegame-capture");
	}
	
    private void OnExpectTwoOfEach() {
		int[] must_have = new int[] {
			0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
			10, 11, 12
		};
		report_controller_issue_.DumpFramesAssertStart("Capture if missing one type is missing", "samplegame-expect-1-each", null, null, must_have);
	}
	
	private void OnExpectNone() {
		report_controller_issue_.DumpFramesAssertStart("Capture if detecting anything", "samplegame-expect-none", null, null, null);
	}

    private void OnExpectNoChange() {
        List<int> forbidden = new List<int>();
        List<int> must_have = new List<int>(Game.EventProcessor.Ids);
        int[] all = new int[] {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
            10, 11, 12
        };
        for (int i=0 ; i<all.Length; i++) {
            if (!Game.EventProcessor.Ids.Contains(all[i])) {
                forbidden.Add(all[i]);
            }
        }
		report_controller_issue_.DumpFramesAssertStart("Capture if anything changes", "samplegame-expect-no-change", null, forbidden.ToArray(), must_have.ToArray());
    }

	private void CheckOptions() {
		SetButtonEnabled (SettingsOption.kSettingsOption_Home, Game.enableHome);
		SetButtonEnabled (SettingsOption.kSettingsOption_Restart, Game.enableRestartLevel);
        SetButtonEnabled (SettingsOption.kSettingsOption_Report, Game.enableVisionReportUI);

		// the position could be different based on current state of the transition, so getting the initial position here 
		// instead of at initialization so that the buttons won't end up in the wrong place.  the first button should 
		// never be moved and always have the correct value
		Vector3 top_initial_position = buttons[0].transform.localPosition;
        Vector3 bottom_initial_position = buttons[buttons.Count - 1].transform.localPosition;

		// adjust positioning to remove gaps
		int top_active_count = 0;
        int bottom_active_count = 0;
        for (int i = 0; i < buttons.Count; i++) {
            if (!top_.Contains((SettingsOption) i)) continue;
            Button button = buttons[i]; 
			if (button.gameObject.activeSelf) {
				Go.killAllTweensWithTarget(button);
                Text label = labels[i];
                if (label != null) Go.killAllTweensWithTarget(label);
                button.transform.localPosition = top_initial_position + top_active_count * top_spacing_;
                top_active_count++;
			}
		}

        for (int i = buttons.Count - 1; i >= 0; i--) {
            if (top_.Contains((SettingsOption) i)) continue;
            Button button = buttons[i]; 
            if (button.gameObject.activeSelf) {
                Go.killAllTweensWithTarget(button);
                Text label = labels[i];
                if (label != null) Go.killAllTweensWithTarget(label);                
                button.transform.localPosition = bottom_initial_position + bottom_active_count * bottom_spacing_;
                bottom_active_count++;
            }
        }
		
		SlideBackTransition transition = this.GetComponent<SlideBackTransition> ();
		if (transition != null) {
			transition.UpdateTargetPositions ();
		}
	}

	public override void Reset(){
		base.Reset ();

		CheckOptions ();
	}
	
	void SetButtonEnabled(SettingsOption option, bool enabled) {
		buttons[(int)option].gameObject.SetActive(enabled);
        if (labels[(int)option] != null) labels[(int)option].gameObject.SetActive(enabled);
	}
}