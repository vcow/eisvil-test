using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
		private readonly Dictionary<EnemyType, ReadOnlyReactiveProperty<int>> _deadEnemiesObservable = new();

		public Vector3 PlayerPosition { get; private set; }

		public void SetPlayerPosition(Vector3 position)
		{
			PlayerPosition = position;
		}

		public ReadOnlyReactiveProperty<int> GetDeadEnemiesObservable(EnemyType enemyType = EnemyType.Undefined)
		{
			if (_deadEnemiesObservable.TryGetValue(enemyType, out var observable))
			{
				return observable;
			}

			observable = Observable.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
					handler => (_, args) => handler(args),
					handler => _deadEnemies.CollectionChanged += handler,
					handler => _deadEnemies.CollectionChanged -= handler)
				.Select(_ => enemyType == EnemyType.Undefined
					? _deadEnemies.Count
					: _deadEnemies.Count(data => data.EnemyType == enemyType))
				.ToReadOnlyReactiveProperty(enemyType == EnemyType.Undefined
					? _deadEnemies.Count
					: _deadEnemies.Count(data => data.EnemyType == enemyType))
				.AddTo(_disposables);

			_deadEnemiesObservable.Add(enemyType, observable);
			return observable;
		}

		public void RegisterEnemy(EnemyData enemyData)
		{
			Assert.IsFalse(_livingEnemies.Contains(enemyData) || _deadEnemies.Contains(enemyData),
				$"Enemy {enemyData.Name} already registered.");
			if (enemyData.IsDead)
			{
				_deadEnemies.Add(enemyData);
			}
			else
			{
				_livingEnemies.Add(enemyData);
				IDisposable h = null;
				h = Observable.FromEvent(
						handler => enemyData.OnDieEvent += handler,
						handler => enemyData.OnDieEvent -= handler)
					.Subscribe(_ =>
					{
						_disposables.Remove(h);

						if (_livingEnemies.Remove(enemyData))
						{
							_deadEnemies.Add(enemyData);
						}
					})
					.AddTo(_disposables);
			}
		}

		void IDisposable.Dispose()
		{
			_disposables.Dispose();
		}
	}
}