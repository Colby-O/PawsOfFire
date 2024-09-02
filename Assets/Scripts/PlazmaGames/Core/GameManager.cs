using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlazmaGames.Core.Message;
using PlazmaGames.Core.MonoSystem;
using PlazmaGames.Core.Network;
using PlazmaGames.Settings;

namespace PlazmaGames.Core
{
	public abstract class GameManager : MonoBehaviour
	{
		protected static GameManager _instance;

		private readonly NetworkRequestEmitter _networkEmitter = new NetworkRequestEmitter();
		private readonly MessageManager _messageManager = new MessageManager();
		private readonly MonoSystemManager _monoSystemManager = new MonoSystemManager();

		/// <summary>
		/// Adds a listener for TMessage to the scene.
		/// </summary>
		public static void AddListener<TMessage>(Action<TMessage> listener) where TMessage : IMessage => _instance._messageManager.AddListener(listener);

		/// <summary>
		/// Removes a listener for TMessage to the scene.
		/// </summary>
		public static void RemoveListener<TMessage>(Action<TMessage> listener) where TMessage : IMessage => _instance._messageManager.RemoveListener(listener);

		/// <summary>
		/// Emits an message to the GameManager.
		/// </summary>
		public static void Emit<TMessage>(TMessage msg) where TMessage : IMessage => _instance._messageManager.Emit(msg);

		/// <summary>
		/// Add a MonoSystems to the GameManager. A MonoSystem takes the place of other singleton classes.
		/// </summary>
		public static void AddMonoSystem<TMonoSystem, TBindTo>(TMonoSystem monoSystem) where TMonoSystem : IMonoSystem, TBindTo => _instance._monoSystemManager.AddMonoSystem<TMonoSystem, TBindTo>(monoSystem);

		/// <summary>
		/// Fetches an attached MonoSystem of type TMonoSystem.
		/// </summary>
		public static TMonoSystem GetMonoSystem<TMonoSystem>() => _instance._monoSystemManager.GetMonoSystem<TMonoSystem>();

		/// <summary>
		/// Emit a network request of type TRequest.
		/// </summary>
		public static void EmitNetworkRequest<TRequest>(TRequest type, PacketReader packet, int fromID = -1) where TRequest : System.Enum => _instance._networkEmitter.Emit(Convert.ToInt32(type), packet, fromID);

		/// <summary>
		/// Attaches a callback linked to a network request of type TRequest.
		/// </summary>
		public static void AddNetworkRequest<TRequest>(TRequest type, Action<PacketReader, int> callback) where TRequest : System.Enum => _instance._networkEmitter.Subscribe(Convert.ToInt32(type), callback);

		/// <summary>
		/// Removes a callback linked to a network request of type TRequest.
		/// </summary>
		public static void RemoveNetworkRequest<TRequest>(TRequest type, Action<PacketReader, int> callback) where TRequest : System.Enum => _instance._networkEmitter.Unsubscribe(Convert.ToInt32(type), callback);
		
		/// <summary>
		/// Initialzes the GameManager automatically on scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			if (_instance) return;

//            PlazmaGamesSettings settings = PlazmaGamesSettings.GetSettings();
			string prefabPath = "GameManager";//settings.GetSceneGameManagerNameOrDefault(SceneManager.GetActiveScene().name);

			GameManager gameManagerPrefab = Resources.Load<GameManager>(prefabPath);
			GameManager gameManager = Instantiate(gameManagerPrefab);

			gameManager.name = gameManager.GetApplicationName();

			DontDestroyOnLoad(gameManager);

			_instance = gameManager;

			gameManager.OnInitalized();
		}

		/// <summary>
		/// Fetches the name of the application.
		/// </summary>
		protected abstract string GetApplicationName();
		
		/// <summary>
		/// Function to be ran after the GameManager is Initalized.
		/// </summary>
		protected abstract void OnInitalized();

		protected virtual void Update()
		{
			_networkEmitter.CheckForRequest();
		}
	}
}
