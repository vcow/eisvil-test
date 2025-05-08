using System.Collections.Generic;
using Plugins.vcow.ScreenLocker;
using UnityEngine;

namespace ScreenLocker
{
	[CreateAssetMenu(menuName = "Settings/Screen Locker Settings", fileName = "ScreenLockerSettings")]
	public class ScreenLockerSettings : ScriptableObject, IScreenLockerSettings
	{
		[SerializeField] private ScreenLockerBase[] _screenLockers;

		public IReadOnlyList<ScreenLockerBase> ScreenLockers => _screenLockers;
	}
}