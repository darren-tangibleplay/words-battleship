using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class SpriteAnimationTool : SpriteAnimationToolBase {

	static List<string> ignoreList = new List<string>{  };

	[MenuItem("Tools/Generate Sprite Anims")]
	public static void GenerateSpriteAnims() {
		GenerateAnimsForDirectory ("/samplegame/sprites/generate_keyframes/", "samplegame/Resources/animations/", ignoreList);
	}
}