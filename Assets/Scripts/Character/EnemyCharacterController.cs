using System;
using UnityEngine;
using UnityEngine.AI;

namespace Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(NavMeshAgent),
		 typeof(HumanoidCharacterController), typeof(Collider))]
	public class EnemyCharacterController : MonoBehaviour
	{
		[SerializeField] private float _hitForce = 5f;
		[SerializeField] private NavMeshObstacle _corpseObstacle;

		private NavMeshAgent _navMeshAgent;
		private HumanoidCharacterController _humanoid;

		private void Awake()
		{
			_navMeshAgent = GetComponent<NavMeshAgent>();
			_humanoid = GetComponent<HumanoidCharacterController>();
		}

		private void OnCollisionEnter(Collision other)
		{
			_humanoid.Kill(other.GetContact(0).point, _hitForce);
		}

		public void OnDie()
		{
			_navMeshAgent.enabled = false;
			if (_corpseObstacle)
			{
				_corpseObstacle.enabled = true;
			}
		}
	}
}