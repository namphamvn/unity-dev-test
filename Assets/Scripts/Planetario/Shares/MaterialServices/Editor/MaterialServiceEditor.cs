#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Planetario.Shares.MaterialServices.Editor
{
	[CustomEditor(typeof(MaterialService))]
	public class MaterialServiceEditor : UnityEditor.Editor
	{
		private bool _keywordFoldout;
		private bool _materialFoldout;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var myTarget = (MaterialService)target;
			_materialFoldout =
				EditorGUILayout.BeginFoldoutHeaderGroup(_materialFoldout, "Generated Materials");
			if (_materialFoldout)
			{
				foreach (var material in myTarget.Materials)
				{
					EditorGUILayout.ObjectField(material.name, material, typeof(Material), true);
				}
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
			_keywordFoldout =
				EditorGUILayout.BeginFoldoutHeaderGroup(_keywordFoldout, "Generated Keywords");
			if (_keywordFoldout)
			{
				foreach (var keyword in myTarget.Keywords)
				{
					EditorGUILayout.LabelField(keyword.ToString());
				}
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
		}
	}
}
#endif
