﻿using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UFZ.Rendering;

namespace UFZ.Interaction
{
	public abstract class ObjectVisibility : SerializedMonoBehaviour
	{
		/// <summary>
		/// Struct for storing transition data for one GameObject.
		/// </summary>
		public struct ObjectVisibilityInfo
		{
			/// <summary>
			/// The game object which will be fade in / out.
			/// </summary>
			[SceneObjectsOnly]
			public GameObject GameObject;
			/// <summary>
			/// The target opacity.
			/// </summary>
			[HorizontalGroup("Group 1")]
			public float Opacity;
			/// <summary>
			/// The transition duration
			/// </summary>
			[HorizontalGroup("Group 1")]
			public float Duration;
		}

		[Button, GUIColor(0.4f, 1f, 0.4f), PropertyOrder(-1)]
		public void SetVisibilitiesInScene()
		{
			Do(true);
		}

		/// <summary>
		/// An array of object transition data.
		/// </summary>
		[OdinSerialize]
		public ObjectVisibilityInfo[] Entries = new ObjectVisibilityInfo[0];

		[BoxGroup("Global settings"), HorizontalGroup("Global settings/Group 1")]
		public float Opacity;
		[BoxGroup("Global settings"), HorizontalGroup("Global settings/Group 1")]
		public float Duration = 5f;

		[Button, BoxGroup("Global settings"), GUIColor(1, 0.6f, 0.4f)]
		public void OverwriteWithGlobalSettings()
		{
			for (var index = 0; index < Entries.Length; index++)
			{
				Entries[index].Opacity = Opacity;
				Entries[index].Duration = Duration;
			}
		}

		public void Do(bool immediate = false)
		{
			foreach (var entry in Entries)
			{
				if(entry.GameObject == null)
					continue;

				var matProps = entry.GameObject.GetComponentsInChildren<MaterialProperties>();
				foreach (var matProp in matProps)
				{
					// This assignment is necessary due to the old version of NET that Unity uses,
					// where the target of a foreach is not considered unique as it should
					// From http://dotween.demigiant.com/support.php
					var thisProp = matProp;
					if (entry.Duration > 0f && !immediate && !Mathf.Approximately(thisProp.Opacity, entry.Opacity))
						DOTween.To(() => thisProp.Opacity, x => thisProp.Opacity = x, entry.Opacity, entry.Duration);
						//LeanTween.value(thisProp.gameObject, thisProp.SetOpacity, thisProp.Opacity, entry.Opacity, entry.Duration);
					else
						thisProp.Opacity = entry.Opacity;
				}

				var hideables = entry.GameObject.GetComponentsInChildren<IHideable>();
				foreach (var hideable in hideables)
				{
					var thisHideable = hideable;
					thisHideable.Enabled = !(entry.Opacity < 1.0);
				}
			}
		}
	}
}
