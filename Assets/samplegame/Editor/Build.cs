using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEditor.Callbacks;

public class Build : BuildBase {

	private static string[] scenes = new string[1] {"Assets/main.unity"};
	
	private static BuildConfig release = new BuildConfig(
		"com.tangibleplay.osmo.samplegame",
		"Assets/samplegame/icon/default",
		"",
		"builds/ios/release"
	);
	
	private static BuildConfig qa = new BuildConfig(
		"com.tangibleplay.osmo.samplegame",
		"Assets/samplegame/icon/default",
		"QA",
		"builds/ios/qa"
	);
	
	private static BuildConfig beta = new BuildConfig(
		"com.tangibleplay.osmo2.samplegame",
		"Assets/samplegame/icon/beta",
		"BETA",
		"builds/ios/beta"
	);

	public static void PreBuildQA(){
		PreBuild(qa);
	}
	
	public static void PreBuildBeta(){
		PreBuild(beta);
	}
	
	public static void PreBuildRelease(){
		PreBuild(release);
	}
	
	[MenuItem ("Build/Setup/Reset")]
	public static void Reset() {
		// this function exists to clear any weird state set by non-release build functions!
		PreBuildRelease();
	}
	
	[MenuItem ("Build/Setup/QA")]
	public static void SetQA() {
		PreBuildQA();
	}
	
	[MenuItem ("Build/Setup/Beta")]
	public static void SetBeta() {
		PreBuildBeta();
	}
	
	[MenuItem ("Build/Setup/Release")]
	public static void SetRelease() {
		PreBuildRelease();
	}
	
	[MenuItem ("Build/iOS/QA")]
	public static void BuildQA() {
		PreBuildQA();
		BuildTo (scenes, qa.buildPath);
	}
	
	[MenuItem ("Build/iOS/Beta")]
	public static void BuildBeta() {
		PreBuildBeta();
		BuildTo(scenes, beta.buildPath);
	}
	
	[MenuItem ("Build/iOS/Release")]
	public static void BuildRelease() {
		PreBuildRelease();
		BuildTo(scenes, release.buildPath);
	}
}