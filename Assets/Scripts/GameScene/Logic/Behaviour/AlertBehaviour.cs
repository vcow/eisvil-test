using GameScene.Character;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace GameScene.Logic.Behaviour
{
	public sealed class AlertBehaviour : BehaviourBase
	{
		private const float RotationSpeed = 5f;

		public AlertBehaviour(SceneContext sceneContext, EnemyCharacterController characterController)
			: base(sceneContext, characterController)
		{
			var navMeshAgent = characterController.GetComponent<NavMeshAgent>();
			Assert.IsTrue(navMeshAgent);

			navMeshAgent.isStopped = true;
			navMeshAgent.ResetPath();
			characterController.Move = false;
		}

		public override void Update(Vector3 toPlayerVector)
		{
			var lookAtPlayerVector = Quaternion.AngleAxis(180f, Vector3.up) * toPlayerVector.normalized;
			var targetRotation = Quaternion.LookRotation(lookAtPlayerVector);
			var t = _characterController.transform;
			t.rotation = Quaternion.Slerp(t.rotation, targetRotation, Time.deltaTime * RotationSpeed);
		}
  
		public override void Dispose()
		{
		}
	}
}