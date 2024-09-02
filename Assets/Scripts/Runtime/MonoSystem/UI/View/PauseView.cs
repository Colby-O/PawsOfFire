using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace PawsOfFire.MonoSystem
{
    internal class PauseView : View
    {
        [SerializeField] private Button _resume;
        [SerializeField] private Button _settings;
        [SerializeField] private Button _leave;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _click;

        private void Resume()
        {
            _audioSource.PlayOneShot(_click);
            GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
            PawsOfFireGameManager.allowInput = true;
        }

        private void Settings()
        {
            _audioSource.PlayOneShot(_click);
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<SettingView>();
        }

        private void Leave()
        {
            _audioSource.PlayOneShot(_click);
            PawsOfFireGameManager.player.Death();
        }

        public override void Show()
        {
            base.Show();
            PawsOfFireGameManager.allowInput = false;
        }

        public override void Init()
        {
            _resume.onClick.AddListener(Resume);
            _settings.onClick.AddListener(Settings);
            _leave.onClick.AddListener(Leave);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Resume();
        }
    }
}
