using PawsOfFire.Helpers;
using PawsOfFire.MonoSystem;
using PlazmaGames.Core;
using PlazmaGames.MonoSystems.Animation;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

namespace PawsOfFire.Player
{
    [RequireComponent(typeof(PlayerInput), typeof(Rigidbody))]
    internal class PlayerController : MonoBehaviour
    {
        private const string SERVER_IP = "https://plasmaclash.com:3002";

        [Header("References")]
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Animator _anim;
        [SerializeField] private MeshRenderer _eyeL;
        [SerializeField] private MeshRenderer _eyeR;
        [SerializeField] private SkinnedMeshRenderer _body;
        [SerializeField] private PlayerSettings _settings;
        [SerializeField] private AudioSource _audioSource;

        [Header("Audio")]
        [SerializeField] private AudioClip _meowClip;
        [SerializeField] private AudioClip _eatingClip;
        [SerializeField] private AudioClip _jumpClip;

        [Header("Settimgs")]
        [SerializeField] private float _speed;
        [SerializeField] private float _rotSpeed;
        [SerializeField] private Vector2 _jumpForce;
        [SerializeField] private float _maxForce;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _groundDist;
        [SerializeField] private float _deathDuration;
        [SerializeField] private float _jumpCycleSpeed;

        private Vector2 _rawMovement;

        [SerializeField] private bool _isGrounded;
        [SerializeField] private bool _isJumpDown;

        private InputAction _moveAction;
        private InputAction _jumpAction;

        private float _lastYPosition;

        [SerializeField] private int _score;

        private Vector3 _startPos;
        private Quaternion _startRot;

        [SerializeField] private float _jumpPower;

        public PlayerSettings GetSettings()
        {
            return _settings;
        }

        public void AddScore()
        {
            _score++;
            _audioSource.PlayOneShot(_eatingClip);
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().UpdateScore(_score);
        }

        public void ResetScore()
        {
            _score = 0;
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().UpdateScore(_score);
        }

        private void Burn(float progress)
        {
            _eyeL.material.SetFloat("_DissolveAmount", progress);
            _eyeR.material.SetFloat("_DissolveAmount", progress);
            _body.material.SetFloat("_DissolveAmount", progress);
        }

        private void RotatePlayerTowardsView()
        {
            Vector3 movementDirection = new Vector3(_rawMovement.x, 0.0f, _rawMovement.y);
            Vector3 velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            if (velocity.magnitude > 0.01f)
            {
                Quaternion toRot = Quaternion.LookRotation(velocity.normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, _rotSpeed * Time.deltaTime);
            }
        }

        private void ProcessMovement()
        {
            Vector3 currentVel = _rb.velocity;
            Vector3 targetVel = new Vector3(_rawMovement.x, 0f, _rawMovement.y);
            targetVel *= _speed;
            targetVel = Camera.main.transform.TransformDirection(targetVel);
            Vector3 deltaVel = Vector3.ClampMagnitude(targetVel - currentVel, _maxForce);
            deltaVel = new Vector3(deltaVel.x, 0f, deltaVel.z);
            _rb.AddForce(deltaVel, ForceMode.VelocityChange);
        }

        private void ProcessJump(float power)
        {
            Vector3 jumpForce = Vector3.zero;

            if (_isGrounded)
            {
                jumpForce = Vector3.up * power;
            }
            _rb.AddForce(jumpForce, ForceMode.VelocityChange);
        }

        private void MovementInput(InputAction.CallbackContext e)
        {
            _rawMovement = e.ReadValue<Vector2>();
        }

        private void OnJumpPressed(InputAction.CallbackContext e)
        {
            if (!PawsOfFireGameManager.allowInput) return;
            _isJumpDown = true;
            _jumpPower = 0;
        }

        private void OnJumpRelased(InputAction.CallbackContext e)
        {
            _isJumpDown = false;
            if (!PawsOfFireGameManager.allowInput) return;
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().UpdateGague(0);
            ProcessJump(
                Mathf.Lerp(
                    _jumpForce.x, 
                    _jumpForce.y, 
                    Mathf.Clamp01(_jumpPower)
                )
            );
        }

        private void CheckIfGrounded()
        {
            _isGrounded = Physics.Raycast(transform.position, -Vector3.up, out RaycastHit info, _groundDist, _groundMask);
            _lastYPosition = transform.position.y;
        }

        private IEnumerator SendScore()
        {
            if (!PawsOfFireGameManager.isOnline || _score <= PawsOfFireGameManager.session.score)
            {
                transform.position = _startPos;
                transform.rotation = _startRot;
                _eyeL.material.SetFloat("_DissolveAmount", 0);
                _eyeR.material.SetFloat("_DissolveAmount", 0);
                _body.material.SetFloat("_DissolveAmount", 0);
                PawsOfFireGameManager.treatController.EndSpawn();
                ResetScore();
                GameManager.GetMonoSystem<IUIMonoSystem>().Show<MainMenuView>();
            }
            else
            {
                WWWForm form = new WWWForm();
                form.AddField("name", PawsOfFireGameManager.session.name);
                form.AddField("password", PawsOfFireGameManager.session.password);
                form.AddField("score", _score);

                using (UnityWebRequest request = UnityWebRequest.Post(
                    SERVER_IP + "/update-score",
                    form
                ))
                {
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        PawsOfFireGameManager.session.score = _score;
                    }

                    transform.position = _startPos;
                    transform.rotation = _startRot;
                    _eyeL.material.SetFloat("_DissolveAmount", 0);
                    _eyeR.material.SetFloat("_DissolveAmount", 0);
                    _body.material.SetFloat("_DissolveAmount", 0);
                    PawsOfFireGameManager.treatController.EndSpawn();
                    ResetScore();
                    GameManager.GetMonoSystem<IUIMonoSystem>().Show<MainMenuView>();                
                }
            }
        }

        public void Death()
        {
            Debug.Log("Player Died");
            ToggleFreeze(true);
            _anim.SetBool("IsJumping", false);
            _anim.SetBool("IsWalking", false);
            _anim.SetBool("IsPrepingJump", false);
            _audioSource.PlayOneShot(_meowClip);
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(this, _deathDuration, Burn, () => StartCoroutine(SendScore())); 
        }

        public void ToggleFreeze(bool state)
        {
            PawsOfFireGameManager.allowInput = !state;
        }

        private void Awake()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody>();
            if (_playerInput == null) _playerInput = GetComponent<PlayerInput>();
            if (_anim == null) _anim = GetComponent<Animator>();
            if (_audioSource == null) _audioSource = GetComponent<AudioSource>();

            _lastYPosition = transform.position.y;

            _moveAction = _playerInput.actions["Movement"];
            _jumpAction = _playerInput.actions["Jump"];

            _moveAction.performed += MovementInput;
            _jumpAction.started += OnJumpPressed;
            _jumpAction.canceled += OnJumpRelased;

            _startPos = transform.position;
            _startRot = transform.rotation;

            _isGrounded = true;
        }

        private void FixedUpdate()
        {
            if (!PawsOfFireGameManager.allowInput)
            {
                _rb.velocity = Vector3.zero;
                return;
            }
            ProcessMovement();
            RotatePlayerTowardsView();
        }

        private void Update()
        {
            bool isLastGrounded = _isGrounded;

            if (!PawsOfFireGameManager.allowInput)
            {
                _rb.velocity = Vector3.zero;
                _rb.isKinematic = true;
                return;
            }
            else
            {
                _rb.isKinematic = false;
            }

            if (_isJumpDown)
            {
                _jumpPower += Time.deltaTime * _jumpCycleSpeed;

                if (_jumpPower  > 1)
                {
                    _jumpPower = 0;
                }

                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().UpdateGague(_jumpPower);
            }

            if (Input.GetKeyDown(KeyCode.Escape)) GameManager.GetMonoSystem<IUIMonoSystem>().Show<PauseView>();

            CheckIfGrounded();

            _anim.SetBool("IsJumping", !_isGrounded);
            _anim.SetBool("IsWalking", new Vector3(_rb.velocity.x, 0f, _rb.velocity.z).magnitude > 0.01f);
            _anim.SetBool("IsPrepingJump", _isJumpDown && !(new Vector3(_rb.velocity.x, 0f, _rb.velocity.z).magnitude > 0.01f));

            if (isLastGrounded != _isGrounded) _audioSource.PlayOneShot(_jumpClip);
        }
    }
}
