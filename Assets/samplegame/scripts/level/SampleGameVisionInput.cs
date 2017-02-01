using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tangible;

public class SampleGameVisionInput : LevelVisionInput {

	[SerializeField]
	TextMesh text_;

	[SerializeField]
	Color text_highlight_color_;
	Color initial_text_color_;

	private long total_ = 0;
	private GoTweenFlow turn_flow_;

	private int turn_value_;


	// for this example game, computing a simple value from the clusters to use to see if the data has changed
	// and provide something to use in the simple example gameplay
	override protected void UpdateCurrentState(List<List<ClusterHelper.Card>> clusters) {
		long old_total = total_;
		total_ = 0;
		foreach (List<ClusterHelper.Card> cluster in clusters) {
			int clusterVal = 1;
			foreach (ClusterHelper.Card card in cluster) {
				clusterVal *= card.value;
			};

			total_ += clusterVal;          
		}

		if (old_total != total_) {
			ResetConfidenceCheck();
		} else {
			IncrementConfidence ();
		}
	}
	
	override protected void UpdateCurrentInputDisplay(List<List<ClusterHelper.Card>> clusters) {
		string textToDisplay = "";

        if (clusters.Count >= 1 && clusters[0].Count >= 1) {

            for (int i = 0; i < clusters.Count; i++) {
				List<ClusterHelper.Card> cluster = clusters[i];
				for (int j = 0 ; j < cluster.Count; j++) {
					textToDisplay += cluster[j].id.ToString();
					if (j == cluster.Count - 1) {
						textToDisplay += "\n";
					} else {
						textToDisplay += " ";
					}
                }
            }
			textToDisplay += "TOTAL: " + total_;
        }
		text_.text = textToDisplay;
    }

	override protected void HandleButtonPress(ClusterHelper.Card card) {
		FloatyText.Create (null, "button pressed: " + card.id);
	}
		
    override protected void Start() {
		base.Start ();
       
		initial_text_color_ = text_.color;
    }
		
	override protected void CheckForTurn() {
		// for sample just checking to see if there is any input that's been the same long enough to have confidence in and using that
		// to take a turn
		// should actually check to see if they're doing something valid that we want to do something with
		float confidence = ComputeConfidence();
		if (last_clusters_ != null && last_clusters_.Count > 0 && confidence >= 1.0f) {
			turn_value_ = (int)total_;
			SetState(State.ANIMATE_TURN);
		}
	}
		
	override protected void CompleteAllAnimations() {
		if (turn_flow_ != null) {
			turn_flow_.destroy();
			turn_flow_ = null;
		}
		if (text_ != null) {
			text_.color = initial_text_color_;	
		}
	}

	override protected void AnimateTurn() {
		base.AnimateTurn ();

		Game.Level.CreateChildScore (null, turn_value_);
	
		turn_flow_ = new GoTweenFlow();
		turn_flow_.autoRemoveOnComplete = true;
		turn_flow_.insert(0.0f, Go.to(text_, 0.3f, new GoTweenConfig().colorProp("color", text_highlight_color_).setEaseType(GoEaseType.QuadOut).setIterations(5, GoLoopType.PingPong)));
		turn_flow_.insert (0.0f, new GoTween (this, 5.0f, new GoTweenConfig ()));

		turn_flow_.setOnCompleteHandler(thisTween => AnimateTurnComplete());
		turn_flow_.play();
	}


	override protected void AnimateTurnComplete() {
		base.AnimateTurnComplete ();

		turn_flow_ = null;
	}
}
