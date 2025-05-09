using Character;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace GameScene.Logic.Behaviour
{
	public sealed class RunAwayBehaviour : BehaviourBase
	{
		private readonly NavMeshAgent _navMeshAgent;
		private float _checkDirectionTimestamp;

		private const float CheckDirectionStepTimeSec = 1f;
		private const float FleeDistance = 10f;

		public RunAwayBehaviour(SceneContext sceneContext, EnemyCharacterController characterController)
			: base(sceneContext, characterController)
		{
			_navMeshAgent = characterController.GetComponent<NavMeshAgent>();
			Assert.IsTrue(_navMeshAgent);

			_navMeshAgent.isStopped = true;
			_navMeshAgent.ResetPath();

			_characterController.IsRun = true;
			CheckDirectionAndRun((characterController.transform.position - sceneContext.PlayerPosition).normalized);
		}

		public override void Update(Vector3 toPlayerVector)
		{
			if (Time.time - _checkDirectionTimestamp >= CheckDirectionStepTimeSec)
			{
				CheckDirectionAndRun(toPlayerVector.normalized);
			}
		}

		private void CheckDirectionAndRun(Vector3 toPlayerVector)
		{
			_checkDirectionTimestamp = Time.time;

			for (var ang = 0f; ang < 360f; ang += 30f)
			{
				var rotatedVector = Quaternion.AngleAxis(ang, Vector3.up) * toPlayerVector;
				var targetPosition = rotatedVector * FleeDistance + _characterController.transform.position;
				if (NavMesh.SamplePosition(targetPosition, out var hit, 2f, NavMesh.AllAreas))
				{
					_navMeshAgent.SetDestination(hit.position);
					_characterController.Move = true;
					return;
				}
			}

			_navMeshAgent.isStopped = true;
			_navMeshAgent.ResetPath();
			_characterController.Move = false;
		}

		public override void Dispose()
		{
		}
	}
}