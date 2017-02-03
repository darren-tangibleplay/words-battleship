using System;
using System.Collections;
using System.Linq;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;
using Tangible.Shared;
using TMPro;

namespace Tangible.WordsBattleship {
    public class GuessWordView : MonoBehaviour {
        // PRAGMA MARK - Public Interface
        public void Init(GuessWordState guessWordState) {
            guessWordState_ = guessWordState;
            Refresh();
        }


        // PRAGMA MARK - Internal
        [SerializeField] private TMP_Text text_;

        private GuessWordState guessWordState_;

        void Update() {
            Refresh();
        }

        private void Refresh() {
            text_.text = (Math.Max((int)guessWordState_.TimeLeft, 0)).ToString();
            text_.color = (guessWordState_.TimeLeft <= 3.0f) ? Color.red : Color.green;
        }
    }
}