using System.Collections.Generic;

namespace GameScene.Quest
{
	public interface IQuestSettings
	{
		IReadOnlyDictionary<QuestTriggerType, float> QuestData { get; }
	}
}