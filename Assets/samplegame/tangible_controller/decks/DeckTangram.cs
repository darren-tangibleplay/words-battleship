// using UnityEngine;
// using System.Collections;
// using Tangible;
//
// public class DeckTangram : Deck {
//
// 	public OnScreenObject blockPrefab;
//
// 	private float sizeScreen = 320;
// 	private float sizeMm = 110.0f;
// 	private Shape[] shapes;
// 	private TangramIdConfig idConfig;
//
// 	void Awake() {
// 		idConfig = gameObject.GetComponent<TangramIdConfig> ();
// 		if (idConfig == null) {
// 			Debug.LogError ("TangramIdConfig component must be set on DeckTangram");
// 		}
// 	}
//
// 	public class Shape {
// 		public readonly Mesh mesh = new Mesh();
// 	 	public readonly TangibleObject.Shape shape;
//
// 		public Shape(int rank) {
// 			mesh = CreateMesh(rank);
// 			shape = TangramHelper.GetShape(rank);
// 		}
//
// 		public static Vector3 GetCenter(TangibleObject.Shape shape) {
// 			Vector2[] ps = TangramHelper.GetPoints(shape);
// 			Vector2 p = Vector3.zero;
// 			for (int i = 0; i < ps.Length; i++) {
// 				p += ps[i];
// 			}
// 			p *= 1.0f / ps.Length;
// 			return new Vector3(p.x, p.y, 0);
// 		}
//
// 		public static Mesh CreateMesh(int rank) {
// 			TangibleObject.Shape shape = TangramHelper.GetShape(rank);
// 			Mesh mesh = new Mesh();
// 			Vector2[] points2d = TangramHelper.GetPoints(shape);
// 			Vector2[] uvs = TangramHelper.GetUVs(rank);
// 			int[] tris = points2d.Length == 3 ? TangramHelper.triangle_index : TangramHelper.quad_index;
// 			Vector3[] points = new Vector3[points2d.Length];
// 			for (int i = 0; i < points2d.Length; i++) {
// 				points[i] = new Vector3(points2d[i].x, points2d[i].y, 0);
// 			}
// 			mesh.vertices = points;
// 			mesh.triangles = tris;
// 			mesh.uv = uvs;
// 			return mesh;
// 		}
//
// 		public static Mesh CreateInnerMesh(int rank, float thickness) {
// 			TangibleObject.Shape shape = TangramHelper.GetShape(rank);
// 			Mesh mesh = new Mesh();
// 			Vector2[] points2d = TangramHelper.GetInnerPoints(TangramHelper.GetPoints(shape), thickness);
// 			Vector2[] uvs = TangramHelper.GetUVs(rank);
// 			int[] tris = points2d.Length == 3 ? TangramHelper.triangle_index : TangramHelper.quad_index;
// 			Vector3[] points = new Vector3[points2d.Length];
// 			for (int i = 0; i < points2d.Length; i++) {
// 				points[i] = new Vector3(points2d[i].x, points2d[i].y, 0);
// 			}
// 			mesh.vertices = points;
// 			mesh.triangles = tris;
// 			mesh.uv = uvs;
// 			return mesh;
// 		}
//
// 		public static Mesh CreateOutlineMesh(int rank, float thickness) {
// 			TangibleObject.Shape shape = TangramHelper.GetShape(rank);
// 			Mesh mesh = new Mesh();
// 			Vector2[] points2dOut = TangramHelper.GetPoints(shape);
// 			Vector2[] points2dIn = TangramHelper.GetInnerPoints(points2dOut, thickness);
// 			Vector3[] points = new Vector3[points2dOut.Length + points2dIn.Length];
// 			Vector2[] uvs = new Vector2[points.Length];
// 			for (int i = 0; i < points2dOut.Length; i++) {
// 				points[2 * i] = new Vector3(points2dOut[i].x, points2dOut[i].y, 0);
// 				points[2 * i + 1] = new Vector3(points2dIn[i].x, points2dIn[i].y, 0);
// 				uvs[2 * i] = points2dOut[i];
// 				uvs[2 * i + 1] = points2dIn[i];
// 			}
// 			int[] tris = points2dOut.Length == 3 ? TangramHelper.triangle_outline_index : TangramHelper.quad_outline_index;
// 			mesh.vertices = points;
// 			mesh.triangles = tris;
// 			mesh.uv = uvs;
// 			return mesh;
// 		}
// 	}
//
// 	void Init() {
// 		if (shapes != null) return;
// 		int cardCount = GetCount ();
// 		shapes = new Shape[cardCount];
// 		for (int i= 0; i<cardCount; i++) {
// 			shapes[i] = new Shape(i);
// 		}
// 	}
//
// 	void Start() {
// 		Init();
// 	}
//
// 	override public OnScreenObject GetPrefab() {
// 		return blockPrefab;
// 	}
//
// 	override public TangibleObject.Shape GetShape(int index) {
// 		return TangramHelper.GetShape(index);
// 	}
//
// 	override public float GetWidthMillimeters(int id) {
// 		return sizeMm;
// 	}
//
// 	override public float GetHeightMillimeters(int id) {
// 		return sizeMm;
// 	}
//
// 	override public float GetMillimeterToScreen() {
// 		return sizeScreen / sizeMm;
// 	}
//
// 	override protected void AssignGraphicsToMeshRenderer(int index, MeshRenderer renderer) {
// 		Init();
// 		MeshFilter mf = renderer.GetComponent<MeshFilter>() as MeshFilter;
// 		mf.mesh = shapes[index].mesh;
// 		//Destroy(renderer.GetComponent<MeshCollider>());
// 		MeshCollider mc = renderer.gameObject.AddComponent<MeshCollider>() as MeshCollider;
//         //mc.convex = true;
// 		mc.sharedMesh = shapes[index].mesh;
// 		renderer.material.SetColor("_Color", GetColor(index));
//     }
//
// 	override public IdConfig GetIdConfig() {
// 		return idConfig;
// 	}
//
// 	override public Tangible.Config.RecognitionMode GetRecognitionMode() {
// 		return Tangible.Config.RecognitionMode.TANGRAMS;
// 	}
//
// }
