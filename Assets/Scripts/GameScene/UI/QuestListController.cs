using System.Collections.Generic;
using GameScene.Quest.Controller;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;

namespace GameScene.UI
{
	[DisallowMultipleComponent]
	public class QuestListController : MonoBehaviour
	{
		[Inject] private readonly IReadOnlyList<QuestControllerBase> _questList;

		[SerializeField] private RectTransform _container;
		[SerializeField] private QuestViewController _questViewPrefab;

		private void Start()
		{
			foreach (var questController in _questList)
			{
				var view = Instantiate(_questViewPrefab, _container);
				view.Init(questController);
			}
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_container, "_container != null");
			Assert.IsNotNull(_questViewPrefab, "_questViewPrefab != null");
		}
	}
}