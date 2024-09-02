using PawsOfFire.Logic;
using PawsOfFire.MonoSystem;
using PawsOfFire.Player;
using PlazmaGames.Core;
using PlazmaGames.Core.MonoSystem;
using PlazmaGames.MonoSystems.Animation;
using System.Collections;
using UnityEngine;

namespace PawsOfFire
{
    internal class PawsOfFireGameManager : GameManager
    {
        [Header("Holders")]
        [SerializeField] private GameObject _monoSystemParnet;

        [Header("MonoSystems")]
        [SerializeField] private AnimationMonoSystem _animationMonoSystem;
        [SerializeField] private UIMonoSystem _uiMonoSystem;
        [SerializeField] private AudioMonoSystem _audioMonoSystem;

        public static bool allowInput = false;
        public static bool hasStarted = false;
        public static bool isOnline = false;

        public static PlayerController player;
        public static TreatController treatController;

        public static PlayerInfo session;

        public static PawsOfFireGameManager Instance => (PawsOfFireGameManager)_instance;

        public static void SetSession(PlayerInfo info)
        {
            session = info;
        }

        public static void ResetGame()
        {
            Debug.Log("Resetting Game!");
        }

        public static void StartGame()
        {
            Debug.Log("Starting Game!");
            player.ToggleFreeze(false);
            hasStarted = true;
            treatController.StartSpawn();
            GetMonoSystem<IUIMonoSystem>().Show<GameView>();
            player.ResetScore();
        }

        /// <summary>
        /// Adds all events listeners
        /// </summary>
        private void AddListeners()
        {

        }

        /// <summary>
        /// Removes all events listeners
        /// </summary>
        private void RemoveListeners()
        {

        }

        /// <summary>
        /// Attaches all MonoSystems to the GameManager
        /// </summary>
        private void AttachMonoSystems()
        {
            AddMonoSystem<AnimationMonoSystem, IAnimationMonoSystem>(_animationMonoSystem);
            AddMonoSystem<UIMonoSystem, IUIMonoSystem>(_uiMonoSystem);
            AddMonoSystem<AudioMonoSystem, IAudioMonoSystem>(_audioMonoSystem);
        }

        protected override string GetApplicationName()
        {
            return nameof(PawsOfFireGameManager);
        }

        protected override void OnInitalized()
        {
            // Ataches all MonoSystems to the GameManager
            AttachMonoSystems();

            // Adds Event Listeners
            AddListeners();

            // Ensures all MonoSystems call Awake at the same time.
            _monoSystemParnet.SetActive(true);
        }

        private void Awake()
        {
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }

        private void Start()
        {
            player = FindAnyObjectByType<PlayerController>();
            treatController = FindObjectOfType<TreatController>();
        }
    }
}
