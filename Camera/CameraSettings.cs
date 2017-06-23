﻿using UnityEngine;
using System.Collections.Generic;
using FullInspector;

namespace UFZ.Rendering
{
	public class CameraSettings : BaseBehavior
	{
		[HideInInspector]
		public List<vrCamera> _cameras;


		private void Start()
		{
			var clusterNode = MiddleVR.VRClusterMgr.GetMyClusterNode();
			if (clusterNode == null)
			{
				for (uint i = 0; i < MiddleVR.VRDisplayMgr.GetCamerasNb(); i++)
				{
					var cam = MiddleVR.VRDisplayMgr.GetCamera(i);
					if (cam != null)
						_cameras.Add(cam);
				}
			}
			else
			{
				for (uint i = 0; i < clusterNode.GetViewportsNb(); i++)
				{
					var vp = clusterNode.GetViewport(i);
					var camName = vp.GetCamera().GetName();
					var cam = MiddleVR.VRDisplayMgr.GetCameraStereo(camName) as vrCamera;
					if (cam != null)
						_cameras.Add(cam);
				}
			}
			// Has to be called twice! Don't ask why ...
			Set();
			Set();
		}

		[InspectorHeader("Clipping planes")]
		[SerializeField]
		public float Near
		{
			get { return _near; }
			set
			{
				_near = value;
				Set();
				Set();
			}
		}
		private float _near = 0.01f;

		[SerializeField]
		public float Far
		{
			get { return _far; }
			set
			{
				_far = value;
				Set();
				Set();
			}
		}
		private float _far = 1000f;

		private void Set()
		{
			if (_cameras == null)
				return;
			foreach (var cam in _cameras)
			{
				cam.SetFrustumNear(_near);
				cam.SetFrustumFar(_far);
				cam.SetDirty();
			}
		}
	}
}