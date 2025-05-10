using GameScene.Logic;
using GameScene.Logic.Behaviour;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace GameScene.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(NavMeshAgent),
		 typeof(HumanoidCharacterController), typeof(Collider))]
	public class EnemyCharacterController : MonoBehaviour
	{
		[SerializeField] private EnemyType _enemyType;
		[SerializeField] private float _hitForce = 5f;
		[SerializeField] private NavMeshObstacle _corpseObstacle;
		[SerializeField, Header("Behaviour")] private float _alertCharacterDistance;
		[SerializeField] private float _runAwayCharacterDistance;

		[Inject] private readonly SceneContext _sceneContext;

		private NavMeshAgent _navMeshAgent;
		private HumanoidCharacterController _humanoid;
		private EnemyData _enemyData;

		private BehaviourBase _currentBehaviour;

		private void Awake()
		{
			_navMeshAgent = GetComponent<NavMeshAgent>();
			_humanoid = GetComponent<HumanoidCharacterController>();
		}

		private void Start()
		{
			_enemyData = new EnemyData(gameObject.name, _enemyType, this);
			_sceneContext.RegisterEnemy(_enemyData);
		}

		private void OnDestroy()
		{
			_currentBehaviour?.Dispose();
			_currentBehaviour = null;
		}

		private void Update()
		{
			if (_humanoid.IsDead)
			{
				return;
			}

			var toPlayerVector = transform.position - _sceneContext.PlayerPosition;
			toPlayerVector.y = 0f;

			var sqrMagnitude = toPlayerVector.sqrMagnitude;
			if (sqrMagnitude <= _runAwayCharacterDistance * _runAwayCharacterDistance)
			{
				if (_currentBehaviour is RunAwayBehaviour runAwayBehaviour)
				{
					runAwayBehaviour.Update(toPlayerVector);
					return;
				}

				_currentBehaviour?.Dispose();
				_currentBehaviour = new RunAwayBehaviour(_sceneContext, this);
			}
			else if (sqrMagnitude <= _alertCharacterDistance * _alertCharacterDistance)
			{
				if (_currentBehaviour is AlertBehaviour alertBehaviour)
				{
					alertBehaviour.Update(toPlayerVector);
					return;
				}

				_currentBehaviour?.Dispose();
				_currentBehaviour = new AlertBehaviour(_sceneContext, this);
			}
			else
			{
				if (_currentBehaviour is WalkBehaviour walkBehaviour)
				{
					walkBehaviour.Update(toPlayerVector);
					return;
				}

				_currentBehaviour?.Dispose();
				_currentBehaviour = new WalkBehaviour(_sceneContext, this);
			}

			_currentBehaviour.Update(toPlayerVector);
		}

		private void OnCollisionEnter(Collision other)
		{
			_humanoid.Kill(other.GetContact(0).point, _hitForce);
		}

		public bool IsRun
		{
			get => _humanoid.IsRun;
			set => _humanoid.IsRun = value;
		}

		public bool Move
		{
			get => _humanoid.Move;
			set => _humanoid.Move = value;
		}

		public void OnDie()
		{
			_currentBehaviour?.Dispose();
			_currentBehaviour = null;

			_enemyData.SetIsDead();

			_navMeshAgent.enabled = false;
			if (_corpseObstacle)
			{
				_corpseObstacle.enabled = true;
			}
		}
	}
}