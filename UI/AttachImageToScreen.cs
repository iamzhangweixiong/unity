using UnityEngine;

namespace UFZ.UI
{
	public class AttachImageToScreen : MonoBehaviour
	{
		public enum ScreenPosition
		{
			TopRight,
			BottomRight
		}

		public Texture2D Image;
		public string ScreenName = "CenterScreen";
		public ScreenPosition Position = ScreenPosition.TopRight;

		private bool _attached;
		private bool _searched;

		void Update()
		{
			if (_attached) return;

			if (ScreenName.Length == 0)
			{
				Core.Error("AttachImageToScreen: Please specify a valid ScreenName.");
				enabled = false;
				return;
			}

			var screen = MiddleVR.VRDisplayMgr.GetScreen(ScreenName);
			if (screen == null)
			{
				Core.Error("AttachImageToScreen: Screen could not be found.");
				enabled = false;
				return;
			}

			var node = GameObject.Find(ScreenName);
			if (node != null)
			{
				var imageGo = new GameObject("Screen Attached Image");

				// Setting new parent
				imageGo.transform.SetParent(node.transform, false);
				
				Core.Info("AttachToNode: " + name + " attached to : " + node.name);
				_attached = true;

				enabled = false;
			}
			else
			{
				if (_searched) return;
				Core.Error("[X] AttachToNode: Failed to find Game object '" + ScreenName + "'");
				_searched = true;

				enabled = false;
			}
		}
	}
}
