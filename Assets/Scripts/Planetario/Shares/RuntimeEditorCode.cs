using System.Diagnostics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Planetario.Shares
{
	public static class RuntimeEditorCode
	{
		[Conditional("UNITY_EDITOR")]
		public static void SetDirty(Object obj)
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty(obj);
#endif
		}

		[Conditional("UNITY_EDITOR")]
		public static void SaveAllAssets()
		{
#if UNITY_EDITOR
			AssetDatabase.SaveAssets();
#endif
		}

		[Conditional("UNITY_EDITOR")]
		public static void SetSceneDirty(GameObject obj)
		{
#if UNITY_EDITOR
			EditorSceneManager.MarkSceneDirty(obj.scene);
#endif
		}

		public static void SetMesh(MeshFilter meshFilter, Mesh mesh)
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				meshFilter.mesh = mesh;
			}
			else
			{
				meshFilter.sharedMesh = mesh;
			}
#else
			meshFilter.mesh = mesh;
#endif
		}
	}
}
