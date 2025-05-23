using GameScene.Logic;
using Plugins.vcow.TouchHelper;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using VContainer;

namespace GameScene.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(NavMeshAgent), typeof(HumanoidCharacterController))]
	public class PlayerCharacterController : MonoBehaviour
	{
		[SerializeField] private float _rotationSpeed = 10f;
		[SerializeField] private bool _alwaysRun = true;

		[Inject] private readonly SceneContext _sceneContext;

		private NavMeshAgent _navMeshAgent;
		private HumanoidCharacterController _humanoid;
		private Camera _camera;

		private Vector2? _moveInput;
		private Vector2? _lookInput;

		private void Awake()
		{
			_navMeshAgent = GetComponent<NavMeshAgent>();
			_humanoid = GetComponent<HumanoidCharacterController>();
			_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		}

		private void Start()
		{
			_humanoid.IsRun = _alwaysRun;
		}

		public void OnMove(InputAction.CallbackContext context)
		{
			var moveInput = context.ReadValue<Vector2>();
			if (moveInput.Equals(Vector2.zero))
			{
				_moveInput = null;
				_humanoid.Move = false;
			}
			else
			{
				_moveInput = moveInput;
				_humanoid.Move = true;
			}
		}

		public void OnLook(InputAction.CallbackContext context)
		{
			var lookInput = context.ReadValue<Vector2>();
			_lookInput = lookInput.Equals(Vector2.zero) ? null : lookInput;
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			if (context.performed && !TouchHelper.IsLocked)
			{
				_humanoid.Attack();
			}
		}

		private void Update()
		{
			if (TouchHelper.IsLocked)
			{
				if (_humanoid.Move)
				{
					_humanoid.Move = false;
				}

				return;
			}

			if (_moveInput.HasValue)
			{
				var moveDirection = new Vector3(_moveInput.Value.x, 0, _moveInput.Value.y);
				moveDirection = _camera.transform.TransformDirection(moveDirection);
				moveDirection.y = 0f;
				_navMeshAgent.Move(moveDirection * (_navMeshAgent.speed * Time.deltaTime));
			}

			var t = transform;
			if (_lookInput.HasValue)
			{
				var yaw = _lookInput.Value.x * _rotationSpeed * Time.deltaTime;
				t.Rotate(0f, yaw, 0f);
			}

			_sceneContext.SetPlayerPosition(t.position);
		}
	}
}