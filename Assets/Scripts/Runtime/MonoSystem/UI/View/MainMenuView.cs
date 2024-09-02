using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PawsOfFire.MonoSystem
{
    internal class MainMenuView : View
    {
        [SerializeField] RawImage _menuView;

        [SerializeField] private Button _play;
        [SerializeField] private Button _leaderboards;
        [SerializeField] private Button _settings;
        [SerializeField] private Button _exit;
        [SerializeField] private Button _logout;
        [SerializeField] private TMP_Text _welcome;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _click;
        [SerializeField] private Camera _cam;
        private void Play()
        {
            _audioSource.PlayOneShot(_click);
            _menuView.gameObject.SetActive(false);
            _cam.gameObject.SetActive(false);
            PawsOfFireGameManager.StartGame();
        }

        private void Leaderbarods()
        {
            _audioSource.PlayOneShot(_click);
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<LeaderboardView>();
        }

        private void Settings()
        {
            _audioSource.PlayOneShot(_click);
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<SettingView>();
        }

        private void Exit()
        {
            _audioSource.PlayOneShot(_click);
            Application.Quit();
        }

        private void Logout()
        {
            _audioSource.PlayOneShot(_click);
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<LoginView>();
        }

        public override void Show()
        {
            base.Show();
            if (PawsOfFireGameManager.isOnline) _welcome.text = $"Welcome {PawsOfFireGameManager.session.name}!\nYour best score is {PawsOfFireGameManager.session.score}.";
            else _welcome.text = "Offline.";

            if (PawsOfFireGameManager.isOnline) _logout.transform.GetChild(0).GetComponent<TMP_Text>().text = "Logout";
            else _logout.transform.GetChild(0).GetComponent<TMP_Text>().text = "Login";

            _cam.gameObject.SetActive(true);
            _menuView.gameObject.SetActive(true);
        }

        public override void Init()
        {
            _play.onClick.AddListener(Play);
            _settings.onClick.AddListener(Settings);
            _leaderboards.onClick.AddListener(Leaderbarods);
            _logout.onClick.AddListener(Logout);
            _exit.onClick.AddListener(Exit);
        }
    }
}
