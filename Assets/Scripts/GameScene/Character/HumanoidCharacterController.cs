using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GameScene.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(NavMeshAgent))]
	public class HumanoidCharacterController : MonoBehaviour
	{
		[SerializeField] private float _walkSpeed = 3.5f;
		[SerializeField] private float _runSpeed = 5f;
		[SerializeField] private LayerMask _ragDollLayerMask;
		[SerializeField] private GameObject _hitFxPrefab;
		[SerializeField] private UnityEvent _onDie;

		private Animator _animator;
		private IReadOnlyList<Rigidbody> _rigidBodies;

		private bool? _isDead;
		private bool _isRun;
		private bool _move;

		private static readonly int IsRunHash = Animator.StringToHash("IsRun");
		private static readonly int MoveHash = Animator.StringToHash("Move");
		private static readonly int AttackHash = Animator.StringToHash("Attack");

		private void Awake()
		{
			_animator = GetComponentInChildren<Animator>();
			_rigidBodies = GetComponentsInChildren<Rigidbody>();
		}

		private void Start()
		{
			if (!_isDead.HasValue)
			{
				IsDead = false;
			}
		}

		private void OnDestroy()
		{
			_onDie.RemoveAllListeners();
		}

		public bool IsRun
		{
			get => _isRun;
			set
			{
				_isRun = value;
				GetComponent<NavMeshAgent>().speed = value ? _runSpeed : _walkSpeed;
				if (_animator)
				{
					_animator.SetBool(IsRunHash, value);
				}
			}
		}

		public bool Move
		{
			get => _move;
			set
			{
				_move = value;
				if (_animator)
				{
					_animator.SetBool(MoveHash, value);
				}
			}
		}

		public void Attack()
		{
			if (_animator)
			{
				_animator.SetTrigger(AttackHash);
			}
		}

		public void Kill(Vector3 hitPoint, float hitForce)
		{
			IsDead = true;
			if (_rigidBodies is { Count: > 0 })
			{
				foreach (var rigidBody in _rigidBodies)
				{
					rigidBody.AddExplosionForce(hitForce, hitPoint, 5f, 0f, ForceMode.Impulse);
				}
			}

			if (_hitFxPrefab)
			{
				var fx = Instantiate(_hitFxPrefab);
				Destroy(fx, 3f);
				fx.transform.position = hitPoint;
			}
		}

		public bool IsDead
		{
			get => _isDead ?? false;
			set
			{
				if (value == _isDead || IsDead)
				{
					return;
				}

				_isDead = value;
				if (value)
				{
					if (_animator)
					{
						_animator.enabled = false;
					}

					if (_rigidBodies is { Count: > 0 })
					{
						foreach (var rigidBody in _rigidBodies)
						{
							var isInLayerMask = (_ragDollLayerMask.value & (1 << rigidBody.gameObject.layer)) != 0;
							if (isInLayerMask)
							{
								rigidBody.isKinematic = false;
								rigidBody.detectCollisions = true;
								rigidBody.useGravity = true;
							}
							else
							{
								rigidBody.detectCollisions = false;
							}
						}
					}

					_onDie.Invoke();
				}
				else
				{
					if (_animator)
					{
						_animator.enabled = true;
					}

					if (_rigidBodies is { Count: > 0 })
					{
						foreach (var rigidBody in _rigidBodies)
						{
							var isInLayerMask = (_ragDollLayerMask.value & (1 << rigidBody.gameObject.layer)) != 0;
							if (isInLayerMask)
							{
								rigidBody.isKinematic = true;
								rigidBody.detectCollisions = false;
								rigidBody.useGravity = false;
							}
							else
							{
								rigidBody.detectCollisions = true;
							}
						}
					}
				}
			}
		}
	}
}