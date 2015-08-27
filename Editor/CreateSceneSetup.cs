using UFZ.Interaction;
using UFZ.Misc;
using UnityEngine;
using UnityEditor;

namespace UFZ.Initialization
{
	public class CreateSceneSetup : MonoBehaviour
	{
		[MenuItem("UFZ/Create scene setup")]
		static void CreateSetup()
		{
			var sceneSetupGo = GameObject.Find("Scene Setup");
			if (sceneSetupGo == null)
				sceneSetupGo = new GameObject("Scene Setup");

			var cameraPathsGo = GetChildGameObject(sceneSetupGo, "CameraPaths");
			var viewpointsGo = GetChildGameObject(sceneSetupGo, "Viewpoints");
			var visibilitiesGo = GetChildGameObject(sceneSetupGo, "Visibilities");
			AddComponent<GameObjectList>(visibilitiesGo);

			Selection.activeGameObject = sceneSetupGo;
		}

		[MenuItem("UFZ/Add viewpoint")]
		static void AddViewpoint()
		{
			var viewpointsGo = GameObject.Find("Viewpoints");
			if (viewpointsGo == null)
			{
				Debug.LogError("Viewpoints object not found. Run menu item " +
					"UFZ/Create scene setup!");
				return;
			}
			var viewpointGo = new GameObject("New Viewpoint");
			Undo.RegisterCreatedObjectUndo(viewpointGo, "Created new viewpoint");
			viewpointGo.transform.SetParent(viewpointsGo.transform, false);
			viewpointGo.AddComponent<Viewpoint>();

			Selection.activeGameObject = viewpointGo;
		}

		[MenuItem("UFZ/Add camera path")]
		public static void CreateNewCameraPath()
		{
			var pathsGo = GameObject.Find("CameraPaths");
			if (pathsGo == null)
			{
				Debug.LogError("CameraPaths object not found. Run menu item " +
					"UFZ/Create scene setup!");
				return;
			}
			var newCameraPath = new GameObject("New Camera Path");
			Undo.RegisterCreatedObjectUndo(newCameraPath, "Added new camera path");
			newCameraPath.transform.SetParent(pathsGo.transform, false);
			newCameraPath.AddComponent<CameraPath>();
			var animator = newCameraPath.AddComponent<CameraPathAnimator>();
			//if (Camera.main != null)
			//	animator.animationObject = Camera.main.transform;
			Selection.activeGameObject = newCameraPath;
			SceneView.lastActiveSceneView.FrameSelected();
		}

		private static GameObject GetChildGameObject(GameObject parentGo, string childName)
		{
			GameObject childGo;
			var childTr = parentGo.transform.FindChild(childName);
			if (childTr == null)
			{
				childGo = new GameObject(childName);
				childGo.transform.SetParent(parentGo.transform, false);
			}
			else
			{
				childGo = childTr.gameObject;
			}
			return childGo;
		}

		private static T AddComponent<T>(GameObject go) where T : MonoBehaviour
		{
			return go.GetComponent<T>() ?? go.AddComponent<T>();
		}
	}
}
