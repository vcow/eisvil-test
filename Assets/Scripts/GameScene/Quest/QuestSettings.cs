using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameScene.Quest
{
	[CreateAssetMenu(menuName = "Settings/Quest Settings", fileName = "QuestSettings")]
	public class QuestSettings : ScriptableObject, IQuestSettings
	{
		[Serializable]
		public class QuestRecord
		{
			public QuestTriggerType _triggerType;
			public float _targetValue;
		}

		private readonly Lazy<IReadOnlyDictionary<QuestTriggerType, float>> _questData;

		[SerializeField] private QuestRecord[] _quests;

		public QuestSettings()
		{
			_questData = new Lazy<IReadOnlyDictionary<QuestTriggerType, float>>(() =>
				_quests?.GroupBy(record => record._triggerType)
					.Select(records =>
					{
						Assert.IsFalse(records.Count() > 1, $"More than one quest of type {records.Key} detected.");
						return (key: records.Key, value: records.First()._targetValue);
					})
					.ToDictionary(tuple => tuple.key, tuple => tuple.value)
			);
		}

		public IReadOnlyDictionary<QuestTriggerType, float> QuestData => _questData.Value;
	}
}