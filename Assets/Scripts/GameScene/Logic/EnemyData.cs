using System;
using Character;

namespace GameScene.Logic
{
	[Serializable]
	public enum EnemyType
	{
		Undefined,
		Red,
		Blue
	}

	public class EnemyData
	{
		private EnemyCharacterController _characterController;

		public string Name { get; }
		public EnemyType EnemyType { get; }
		public bool IsDead { get; private set; }
		public event Action OnDieEvent;

		public EnemyData(string name, EnemyType enemyType, EnemyCharacterController characterController)
		{
			Name = name;
			EnemyType = enemyType;
			_characterController = characterController;
		}

		public void SetIsDead()
		{
			if (IsDead)
			{
				return;
			}

			IsDead = true;
			_characterController = null;
			OnDieEvent?.Invoke();
		}
	}
}