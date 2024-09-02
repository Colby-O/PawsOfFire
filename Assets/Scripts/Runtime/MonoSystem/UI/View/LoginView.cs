using JetBrains.Annotations;
using PawsOfFire.Helpers;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace PawsOfFire.MonoSystem
{
    internal class ErrorMessage
    {
        public string msg;
    }

    internal class PlayerInfo
    {
        public string name;
        public int score;
        public string password;
        public long id;
    }

    internal class LoginView : View
    {
        private const string SERVER_IP = "https://plasmaclash.com:3002";

        [SerializeField] private Button _login;
        [SerializeField] private Button _create;
        [SerializeField] private Button _exit;
        [SerializeField] private Button _playOffline;
        [SerializeField] private TMP_InputField _username;
        [SerializeField] private TMP_InputField _password;
        [SerializeField] private TMP_Text _error;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _click;

        private IEnumerator LoginRequest()
        {
            if (_username.text == string.Empty || _password.text == string.Empty)
            {
                _error.text = "Error: One or more input fields are empty.";
            }
            else
            {
                WWWForm form = new WWWForm();
                form.AddField("name", _username.text);
                form.AddField("password", HashHelper.CreateMD5(_password.text));

                using (UnityWebRequest request = UnityWebRequest.Post(
                    SERVER_IP + "/login",
                    form
                ))
                {
                    yield return request.SendWebRequest();

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        _error.text = "Error: " + JsonUtility.FromJson<ErrorMessage>(request.downloadHandler.text).msg;
                    }
                    else
                    {
                        PawsOfFireGameManager.SetSession(JsonUtility.FromJson<PlayerInfo>(request.downloadHandler.text));
                        PawsOfFireGameManager.isOnline = true;
                        GameManager.GetMonoSystem<IUIMonoSystem>().Show<MainMenuView>();
                    }
                }
            }
        }

        private IEnumerator CreateAccountRequest()
        {
            if (_username.text == string.Empty || _password.text == string.Empty)
            {
                _error.text = "Error: One or more input fields are empty.";
            }
            else if (_username.text.Length > 25)
            {
                _error.text = "Error: Username must be less than or equal to 25 characters.";
            }
            else
            {
                WWWForm form = new WWWForm();
                form.AddField("name", _username.text);
                form.AddField("password", HashHelper.CreateMD5(_password.text));

                using (UnityWebRequest request = UnityWebRequest.Post(
                    SERVER_IP + "/create-account",
                    form
                ))
                {
                    yield return request.SendWebRequest();

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        _error.text = "Error: " + JsonUtility.FromJson<ErrorMessage>(request.downloadHandler.text).msg;
                    }
                    else
                    {
                        PawsOfFireGameManager.SetSession(JsonUtility.FromJson<PlayerInfo>(request.downloadHandler.text));
                        PawsOfFireGameManager.isOnline = true;
                        GameManager.GetMonoSystem<IUIMonoSystem>().Show<MainMenuView>();
                    }
                }
            }
        }

        private void PlayOffline()
        {
            _audioSource.PlayOneShot(_click);
            PawsOfFireGameManager.isOnline = false;
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<MainMenuView>();

        }

        private void Login()
        {
            _audioSource.PlayOneShot(_click);
            StartCoroutine(LoginRequest());
        }

        private void Create()
        {
            _audioSource.PlayOneShot(_click);
            StartCoroutine(CreateAccountRequest());
        }

        private void Exit()
        {
            _audioSource.PlayOneShot(_click);
            Application.Quit();
        }

        public override void Show()
        {
            base.Show();
            _username.text = string.Empty;
            _password.text = string.Empty;
            _error.text = string.Empty;
        }

        public override void Init()
        {
            _exit.onClick.AddListener(Exit);
            _create.onClick.AddListener(Create);
            _login.onClick.AddListener(Login);
            _playOffline.onClick.AddListener(PlayOffline);
            _error.text = string.Empty;
        }
    }
}
