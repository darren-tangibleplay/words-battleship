using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransitionManager : MonoBehaviour {

    public delegate void OnComplete();

    // Enum order by priority of animation to play (highest at the top)
    public enum Moment {
        LEVEL_SOLVED,
        LEVEL_UNLOCKED,
		LEVEL_SELECTED
    }

    public class Transition {
        readonly public Moment moment;
        readonly public string level_name;
        readonly public string creature_id;
        readonly public string from_level_name;
        private OnComplete on_complete_;

        public Transition(OnComplete on_complete, Moment _moment, string _level_name, string _creature_id = null, string _from_level_name = null) {
            moment = _moment;
            level_name = _level_name;
            creature_id = _creature_id;
            from_level_name = _from_level_name;
            on_complete_ = on_complete;
        }

        public override string ToString() {
            return level_name + "|" + (creature_id == null ? "" : creature_id) + "@" + moment.ToString();
        }

        public void OnComplete() {
            if (on_complete_ != null) on_complete_();
        }
    }

    List<Transition> transitions_ = new List<Transition>();
    HashSet<string> transitions_unique_ = new HashSet<string>();

    public void Add(Transition transition) {
        if (transitions_unique_.Contains(transition.ToString())) return;
        transitions_.Add(transition);
        transitions_unique_.Add(transition.ToString());
    }

    public Transition PopNext() {
        if (transitions_.Count == 0) return null;
        transitions_.Sort((a, b) => ((int) a.moment).CompareTo((int) b.moment));
        Transition next = transitions_[0];
        transitions_.RemoveAt(0);
        transitions_unique_.Remove(next.ToString());
        return next;
    }

    public bool HasNext() {
        return transitions_.Count > 0;
    }
}
