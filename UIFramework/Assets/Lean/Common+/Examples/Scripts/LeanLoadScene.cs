using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lean.Common
{
	/// <summary>This component allows you to load the specified scene when you manually call the Load method.</summary>
	[RequireComponent(typeof(Rigidbody))]
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanLoadScene")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Load Scene")]
	public class LeanLoadScene : MonoBehaviour
	{
		/// <summary>The name of the scene you want to load.</summary>
		public string SceneName;

		/// <summary>Load the scene asynchronously?</summary>
		public bool ASync;

		/// <summary>Keep the existing scene(s) loaded?</summary>
		public bool Additive;
		
		[ContextMenu("Load")]
		public void Load()
		{
			Load(SceneName);
		}

		public void Load(string sceneName)
		{
			if (ASync == true)
			{
				SceneManager.LoadSceneAsync(sceneName, Additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single);
			}
			else
			{
				SceneManager.LoadScene(sceneName, Additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Inspector
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanLoadScene))]
	public class LeanLoadScene_Inspector : LeanInspector<LeanLoadScene>
	{
		protected override void DrawInspector()
		{
			Draw("SceneName", "The name of the scene you want to load.");
			Draw("ASync", "Load the scene asynchronously?");
			Draw("Additive", "Keep the existing scene(s) loaded?");
		}
	}
}
#endif