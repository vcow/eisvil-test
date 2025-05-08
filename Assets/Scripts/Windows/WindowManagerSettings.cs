using Plugins.vcow.WindowManager;
using UnityEngine;

namespace Windows
{
	[CreateAssetMenu(menuName = "Settings/Windows Manager Settings", fileName = "WindowsManagerSettings")]
	public class WindowManagerSettings : ScriptableObject
	{
		[SerializeField] private string[] _groupHierarchy;
		[SerializeField] private WindowsPrefabLibrary[] _windowLibraries;
		[SerializeField] private int _startCanvasSortingOrder;

		public Plugins.vcow.WindowManager.WindowManagerSettings GetSettings() =>
			new()
			{
				GroupHierarchy = _groupHierarchy,
				WindowLibraries = _windowLibraries,
				StartCanvasSortingOrder = _startCanvasSortingOrder
			};
	}
}