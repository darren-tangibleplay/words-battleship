// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
//
// // A controller that rules them all
// public class SelectorController : MonoBehaviour, Controller {
//
// 	public enum Source {
// 		NONE,
// 		PHYSICAL,
// 		PHYSICAL_DEBUG,
// 		PHYSICAL_RECORDED,
// 		ON_SCREEN
// 	};
//
//     [SerializeField]
//     private PhysicalController physical_controller_prefab_;
//     private PhysicalController physical_controller_;
//
//     [SerializeField]
//     private OnScreenController on_screen_controller_prefab_;
//     private OnScreenController on_screen_controller_;
//
//     [SerializeField]
//     private string debug_recorded_input_;
//
//     [SerializeField]
//     private GameObject deck_prefab_;
//     private Deck deck_;
//
// 	private Source source = Source.NONE;
// 	private Controller controller = null;
//
// 	private int userId = Tangible.UserId.INVALID;
//     private Tangible.Config config = null;
//     private Tangible.EventDelegate processEvent = null;
// 	private int dumpInterval = 0;
//
//     bool color_coin_enabled_ = true;
//     public bool ColorCoinEnabled {
//         get { return color_coin_enabled_; }
//     }
//
// 	public Deck Deck {
// 		get {
// 			return deck_;
// 		}
// 	}
//
//     [SerializeField]
//     private Image reflector_marker_;
//
//     private bool muted_ = true;
//
// 	public Source GetSource() {
// 		return source;
// 	}
//
// 	public void AnimateToOriginal() {
// 		if (source == Source.ON_SCREEN) {
// 			on_screen_controller_.AnimateToOriginal();
// 		}
// 	}
//
//     void Controller.Mute(bool mute) {
//         // Use Muted instead
//         throw new UnityException();
//     }
//
//     public bool Muted {
//         get { return muted_; }
//         set { if (muted_ != value) {
//                 muted_ = value;
//                 OnMuteChanged();
//             }
//         }
//     }
//
//     private void OnMuteChanged() {
// 		if (reflector_marker_ != null) reflector_marker_.color = muted_ ? new Color(1, 1, 1, 0.7f) : new Color(1, 1, 1, 1);
//         if (physical_controller_ != null) (physical_controller_ as Controller).Mute(muted_);
//         if (on_screen_controller_ != null) (on_screen_controller_ as Controller).Mute(muted_);
//     }
//
// 	public bool ToggleSource() {
// 		if (debug_recorded_input_ != null && debug_recorded_input_.Length > 0 && !PhysicalController.HardwareSupported()) {
// 			return RegisterWith(Source.PHYSICAL_RECORDED);
// 		}
// 		switch(source) {
// 			case Source.NONE:
// 			case Source.ON_SCREEN:
// 				if (RegisterWith(Source.PHYSICAL)) return true;
// 				goto case Source.PHYSICAL;
// 			case Source.PHYSICAL:
// 				if (RegisterWith(Source.PHYSICAL_DEBUG)) return true;
// 				goto case Source.PHYSICAL_DEBUG;
// 			case Source.PHYSICAL_DEBUG:
// 				if (RegisterWith(Source.ON_SCREEN)) return true;
// 				goto default;
// 			default: break;
// 		}
// 		return false;
// 	}
//
// 	void CreateDeck() {
// 		if (deck_ == null) {
// 			deck_ = (GameObject.Instantiate (deck_prefab_) as GameObject).GetComponent<Deck>();
// 		}
// 	}
//
// 	public bool RegisterWith(Source _source) {
//         Debug.Log("SelectorController Register " + source.ToString() + " -> " + _source.ToString());
// 		if (_source == source) return true;
// 		if (source == Source.ON_SCREEN || _source == Source.ON_SCREEN) DestroyController();
// 		CreateDeck ();
// 		switch(_source) {
// 			case Source.PHYSICAL:
// 			case Source.PHYSICAL_DEBUG:
// 			case Source.PHYSICAL_RECORDED:
// 			{
// 				if (physical_controller_ == null) {
// 					physical_controller_ = Instantiate(physical_controller_prefab_, Vector3.zero, Quaternion.identity) as PhysicalController;
//                     physical_controller_.transform.parent = transform;
// 					physical_controller_.Init(deck_);
//                     (physical_controller_ as Controller).Mute(Muted);
// 				}
// 				controller = physical_controller_ as Controller;
//                 physical_controller_.SetDebug(Source.PHYSICAL_DEBUG == _source || Game.enableVisionEventLog);
// 				physical_controller_.SetRecorded(Source.PHYSICAL_RECORDED == _source, debug_recorded_input_);
//                 physical_controller_.EnableColorCoin(color_coin_enabled_);
// 			} break;
// 			case Source.ON_SCREEN:
// 			default:
// 			{
// 				if (on_screen_controller_ == null) {
// 					on_screen_controller_ = Instantiate(on_screen_controller_prefab_, Vector3.zero, Quaternion.identity) as OnScreenController;
//                     physical_controller_.transform.parent = transform;
//                     on_screen_controller_.Init(deck_);
//                     (on_screen_controller_ as Controller).Mute(Muted);
// 				}
// 				controller = on_screen_controller_ as Controller;
// 			} break;
// 		}
//
// 		if (controller.Register(userId, config, processEvent, dumpInterval)) {
// 			source = _source;
// 		} else {
// 			DestroyController();
// 		}
// 		return _source == source;
//     }
//
// 	void DestroyController() {
// 		if (controller != null) {
// 			controller.Unregister(userId);
// 			controller = null;
// 		}
// 		source = Source.NONE;
// 	}
//
// 	void Update() {
// 		if (debug_recorded_input_ != null && debug_recorded_input_.Length > 0) {
// 			ToggleSource();
// 			debug_recorded_input_ = null;
// 		}
// 	}
//
//     public void SetDebug(bool debug) {
//         physical_controller_.SetDebug(debug);
//     }
//
//     public void EnableColorCoin(bool enable) {
//         color_coin_enabled_ = enable;
//
//         if (physical_controller_ != null) {
//             physical_controller_.EnableColorCoin(color_coin_enabled_);
//         }
//     }
//
// 	public bool Register(int _userId, Tangible.EventDelegate _processEvent, int automaticDumpInterval = 0) {
// 		CreateDeck ();
// 		return (this as Controller).Register (_userId, new Tangible.Config (deck_.GetRecognitionMode ()), _processEvent, automaticDumpInterval);
// 	}
//
// 	//----------------------
//     // Controller
//     //----------------------
//
// 	bool Controller.Register(int _userId, Tangible.Config _config, Tangible.EventDelegate _processEvent, int automaticDumpInterval) {
// 		(this as Controller).Unregister(userId);
// 		userId = _userId;
// 		config = _config;
// 		processEvent = _processEvent;
// 		dumpInterval = automaticDumpInterval;
// 		return ToggleSource();
//     }
//
//     void Controller.Unregister(int _userId) {
// 		userId = Tangible.UserId.INVALID;
//     	config = null;
//     	processEvent = null;
// 		DestroyController();
//     }
// }
