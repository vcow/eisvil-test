using GameScene.Quest.Controller;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GameScene.UI
{
	[DisallowMultipleComponent]
	public class QuestViewController : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _description;
		[SerializeField, Header("Progress")] private RectMask2D _progressBarContainer;
		[SerializeField] private GameObject _doneMarker;
		[SerializeField] private TextMeshProUGUI _label;

		private readonly CompositeDisposable _disposables = new();

		public void Init(QuestControllerBase questController)
		{
			var progressBarTransform = (RectTransform)_progressBarContainer.transform;
			questController.Progress
				.Subscribe(f =>
				{
					var barSize = progressBarTransform.rect.width;
					_progressBarContainer.padding = new Vector4(0f, 0f,barSize * (1f - f), 0f);
				})
				.AddTo(_disposables);
			questController.Description
				.Subscribe(s => _description.text = s)
				.AddTo(_disposables);
			questController.IsCompleted.Subscribe(b =>
				{
					_progressBarContainer.gameObject.SetActive(!b);
					_doneMarker.SetActive(b);
				})
				.AddTo(_disposables);
			questController.Label
				.Subscribe(s => _label.text = s)
				.AddTo(_disposables);
		}

		private void OnDestroy()
		{
			_disposables.Dispose();
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_description, "_description != null");
			Assert.IsNotNull(_progressBarContainer, "_progressBarContainer != null");
			Assert.IsNotNull(_doneMarker, "_doneMarker != null");
			Assert.IsNotNull(_label, "_label != null");
		}
	}
}