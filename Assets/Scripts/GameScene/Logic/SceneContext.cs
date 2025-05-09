using System;
using System.Collections.ObjectModel;
using R3;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameScene.Logic
{
	public class SceneContext : IDisposable
	{
		private readonly CompositeDisposable _disposables = new();
		private readonly ObservableCollection<EnemyData> _livingEnemies = new();
		private readonly ObservableCollection<EnemyData> _deadEnemies = new();

		public Vector3 PlayerPosition { get; private set; }

		public void SetPlayerPosition(Vector3 position)
		{
			PlayerPosition = position;
		}

		public void RegisterEnemy(EnemyData enemyData)
		{
			Assert.IsFalse(_livingEnemies.Contains(enemyData) || _deadEnemies.Contains(enemyData),
				$"Enemy {enemyData.Name} already registered.");
			if (enemyData.IsDead)
			{
				_deadEnemies.Add(enemyData);
				Observable.FromEvent(h => enemyData.OnDieEvent += h, h => enemyData.OnDieEvent -= h)
					.Subscribe(_ =>
					{
						if (_deadEnemies.Remove(enemyData))
						{
							_deadEnemies.Add(enemyData);
						}
					})
					.AddTo(_disposables);
			}
			else
			{
				_livingEnemies.Add(enemyData);
			}
		}

		void IDisposable.Dispose()
		{
			_disposables.Dispose();
		}
	}
}