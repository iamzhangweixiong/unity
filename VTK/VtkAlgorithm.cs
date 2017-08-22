﻿#if UNITY_STANDALONE_WIN
using UnityEngine;
using Kitware.VTK;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UFZ.VTK
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public abstract class VtkAlgorithm : VtkBase
	{
		public bool HasInput { get { return _hasInput; } }
		protected bool _hasInput;

		[ShowInInspector, HideIf("HasOutput")]
		public bool HasOutput
		{
			get
			{
				if (Algorithm == null)
					return false;
				Algorithm.Update();
				var output = Algorithm.GetOutputDataObject(0) as vtkDataSet;
				var numPoints = output.GetNumberOfPoints();
				if (output != null && numPoints > 0)
					return true;
				return false;
			}
		}

		[ShowInInspector]
		public string OutputDataType
		{
			get
			{
				if (!HasOutput)
					return "";
				var output = Algorithm.GetOutputDataObject(0) as vtkDataSet;
				return output.GetType().ToString();
			}
		}

		[ShowInInspector, ShowIf("HasInput")]
		public VtkAlgorithm InputAlgorithm
		{
			get { return _inputAlgorithm; }
			set
			{
				if (Algorithm == null)
					return;
				if (value == null)
					return;
				if (!HasInput || value.AlgorithmOutput == null)
					return;
				_inputAlgorithm = value;
				Algorithm.SetInputConnection(value.AlgorithmOutput);
				UpdateRenderer();
			}
		}
		[SerializeField, HideInInspector]
		private VtkAlgorithm _inputAlgorithm;

		[HideInInspector]
		public vtkAlgorithm Algorithm;

		[SerializeField]
		protected VtkRenderer ren;

		[OdinSerialize, HideInInspector]
		protected vtkAlgorithmOutput AlgorithmOutput;

		protected vtkTriangleFilter TriangleFilter;

		private void Reset()
		{
			Initialize();
			PostInitialize();
		}

#if !UNITY_EDITOR
		protected override void Awake()
		{
			base.Awake();
			Initialize();
			PostInitialize();
		}
#endif

		protected void OnValidate()
		{
			Initialize();
			PostInitialize();
		}

		public vtkAlgorithmOutput OutputPort()
		{
			return AlgorithmOutput;
		}

		[Button, ShowIf("ShowAddRenderer")]
		public void AddRenderer()
		{
			ren = gameObject.AddComponent<VtkRenderer>();
			ren.Algorithm = this;
			Initialize();
		}

		protected bool ShowAddRenderer()
		{
			return HasOutput && ren == null;
		}

		protected virtual void Initialize()
		{
			if (_inputAlgorithm != null)
				_inputAlgorithm.Initialize();
			if (TriangleFilter == null)
				TriangleFilter = vtkTriangleFilter.New();	
		}

		protected virtual void PostInitialize()
		{
			if (Algorithm != null && InputAlgorithm != null && Algorithm.GetInputConnection(0, 0) == null)
				Algorithm.SetInputConnection(InputAlgorithm.AlgorithmOutput);
			// This allows for editing values in inspector but breaks UpdateBuffers on recompile!	
			else
				UpdateRenderer();
		}

		protected void UpdateRenderer()
		{
			if (ren == null)
				return;
			ren.RequestRenderUpdate();
		}
	}
}
#endif