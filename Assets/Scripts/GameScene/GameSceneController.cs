using System;
using System.Collections.Generic;
using System.Linq;
using Windows;
using GameScene.Character;
using GameScene.Quest.Controller;
using NUnit.Framework;
using Plugins.vcow.ScreenLocker;
using Plugins.vcow.WindowManager;
using R3;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using Random = UnityEngine.Random;

namespace GameScene
{
	[DisallowMultipleComponent]
	public class GameSceneController : MonoBehaviour
	{
		[Serializable]
		public struct EnemyPrefabRecord
		{
			public EnemyCharacterController _enemyPrefab;
			public int _countFrom;
			public int _countTo;
		}

		[Inject] private readonly IScreenLockerManager _screenLockerManager;
		[Inject] private readonly IObjectResolver _container;
		[Inject] private readonly IReadOnlyList<QuestControllerBase> _questList;
		[Inject] private readonly IWindowManager _windowManager;

		[SerializeField, Header("Enemy spawn")] private Vector2 _fieldSize;
		[SerializeField] private EnemyPrefabRecord[] _enemyPrefabs;

		private readonly CompositeDisposable _disposables = new();

		private Vector3[] _gizmoPoints = new Vector3[4];

		private void Start()
		{
			SpawnEnemies();

			if (_screenLockerManager.IsLocked)
			{
				_screenLockerManager.Unlock(completeCallback: _ => OnStart());
			}
			else
			{
				OnStart();
			}
		}

		private void OnDestroy()
		{
			_disposables.Dispose();
		}

		private void OnDrawGizmos()
		{
			var color = Gizmos.color;
			Gizmos.color = Color.magenta;
			var halfSize = _fieldSize * 0.5f;
			const float yDistance = 0.3f;
			_gizmoPoints[0] = new Vector3(halfSize.x, yDistance, halfSize.y);
			_gizmoPoints[1] = new Vector3(halfSize.x, yDistance, -halfSize.y);
			_gizmoPoints[2] = new Vector3(-halfSize.x, yDistance, -halfSize.y);
			_gizmoPoints[3] = new Vector3(-halfSize.x, yDistance, halfSize.y);
			Gizmos.DrawLineStrip(_gizmoPoints, true);
			Gizmos.color = color;
		}

		private void OnStart()
		{
			if (_disposables.IsDisposed)
			{
				return;
			}

			Observable.CombineLatest(_questList.Select(quest => quest.IsCompleted))
				.Where(completes => completes.All(b => b))
				.Take(1)
				.Subscribe(_ => OnWin())
				.AddTo(_disposables);
		}

		private void OnWin()
		{
			_windowManager.ShowWindow(WinWindow.Id);
		}

		private void SpawnEnemies()
		{
			var halfSize = _fieldSize * 0.5f;
			Assert.IsTrue(halfSize is { x: > 0f, y: > 0f }, "Field size can't be zero or negative.");

			var layerMask = LayerMask.NameToLayer("Ground");
			foreach (var enemyPrefabRecord in _enemyPrefabs)
			{
				var min = Mathf.Max(0, Mathf.Min(enemyPrefabRecord._countFrom, enemyPrefabRecord._countTo));
				var max = Mathf.Max(0, enemyPrefabRecord._countFrom, enemyPrefabRecord._countTo);
				if (max == 0)
				{
					continue;
				}

				var count = min + Random.Range(0, max - min + 1);
				for (var i = 0; i < count; ++i)
				{
					Transform instance = null;
					const int maxTrySpawn = 10;
					const float enemyYPosition = 0f;
					const float safeArea = 2f;
					for (var j = 0; j < maxTrySpawn; ++j)
					{
						var pos = new Vector3(Random.Range(-halfSize.x, halfSize.x), enemyYPosition, Random.Range(-halfSize.y, halfSize.y));
						if (NavMesh.SamplePosition(pos, out var hit, safeArea, NavMesh.AllAreas) &&
						    !Physics.CheckSphere(hit.position, safeArea, layerMask))
						{
							var enemy = Instantiate(enemyPrefabRecord._enemyPrefab);
							instance = enemy.transform;
							_container.Inject(enemy);
							instance.position = pos;
							break;
						}
					}

					if (!instance)
					{
						Debug.LogError("Can't spawn enemies: no more free space.");
						return;
					}

					instance.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
				}
			}
		}
	}
}