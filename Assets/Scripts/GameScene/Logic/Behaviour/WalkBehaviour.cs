using GameScene.Character;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace GameScene.Logic.Behaviour
{
	public sealed class WalkBehaviour : BehaviourBase
	{
		private readonly NavMeshAgent _navMeshAgent;
		private float? _waitTimesLeft;

		private const float WainMinTimeSec = 1f;
		private const float WainMaxTimeSec = 5f;
		private const float WanderRadius = 10f;

		public WalkBehaviour(SceneContext sceneContext, EnemyCharacterController characterController)
			: base(sceneContext, characterController)
		{
			_navMeshAgent = characterController.GetComponent<NavMeshAgent>();
			Assert.IsTrue(_navMeshAgent);

			characterController.IsRun = false;
			characterController.Move = false;
		}

		public override void Update(Vector3 toPlayerVector)
		{
			if (_waitTimesLeft.HasValue)
			{
				_waitTimesLeft -= Time.deltaTime;
				if (_waitTimesLeft <= 0f)
				{
					_waitTimesLeft = null;
					MoveToNewPoint();
				}

				return;
			}

			if (!_navMeshAgent.pathPending &&
			    (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance ||
			     _navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete))
			{
				_characterController.Move = false;
				_waitTimesLeft = Random.Range(WainMinTimeSec, WainMaxTimeSec);
			}
		}

		private void MoveToNewPoint()
		{
			var randomDirection = Random.insideUnitSphere * WanderRadius;
			randomDirection += _navMeshAgent.transform.position;
			randomDirection.y = 0f;

			const int maxTryMove = 10;
			for (var i = 0; i < maxTryMove; ++i)
			{
				if (NavMesh.SamplePosition(randomDirection, out var hit, WanderRadius, NavMesh.AllAreas))
				{
					_navMeshAgent.SetDestination(hit.position);
					_characterController.Move = true;
					return;
				}
			}

			_waitTimesLeft = 1f;
		}

		public override void Dispose()
		{
		}
	}
}