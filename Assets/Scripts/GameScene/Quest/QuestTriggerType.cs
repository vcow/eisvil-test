using System;

namespace GameScene.Quest
{
	[Serializable]
	public enum QuestTriggerType
	{
		MinTimeInGame,
		RedEnemiesKilled,
		BlueEnemiesKilled,
		TotalEnemiesKilled
	}
}