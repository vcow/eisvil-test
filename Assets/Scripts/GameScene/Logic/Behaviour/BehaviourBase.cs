using System;
using Character;
using UnityEngine;

namespace GameScene.Logic.Behaviour
{
	public abstract class BehaviourBase : IDisposable
	{
		protected readonly SceneContext _sceneContext;
		protected readonly EnemyCharacterController _characterController;

		protected BehaviourBase(SceneContext sceneContext, EnemyCharacterController characterController)
		{
			_characterController = characterController;
		}

		public abstract void Update(Vector3 toPlayerVector);

		public abstract void Dispose();
	}
}