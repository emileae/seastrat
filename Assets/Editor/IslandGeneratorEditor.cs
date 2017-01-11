//using UnityEngine;
//using System.Collections;
//using UnityEditor;
//
//[CustomEditor (typeof (IslandGenerator))]
//public class MapGeneratorEditor : Editor {
//
//	public GameObject platform1;
//	public GameObject platform2;
//	public GameObject platform3;
//
//	public override void OnInspectorGUI() {
//		
//		IslandGenerator islandGen = (IslandGenerator)target;
//
//		// if a change happens in IslandGenerator's settings then its as if a button was pressed
////		if (DrawDefaultInspector ()) {
////			if (mapGen.autoUpdate) {
////				mapGen.GenerateMap ();
////			}
////		}
//
//		platform1 = (GameObject) EditorGUI.ObjectField(new Rect(3, 3, 10, 20), "Platform 1", platform1, typeof(GameObject));
//
//		if (GUILayout.Button ("Generate")) {
//			islandGen.GenerateIsland ();
//		}
//	}
//}