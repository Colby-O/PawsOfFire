using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PawsOfFire.MonoSystem
{
    internal class SettingView : View
    {
        [SerializeField] private Slider _overall;
        [SerializeField] private Slider _music;
        [SerializeField] private Slider _sfx;
        [SerializeField] private Slider _sensivity;
        [SerializeField] private Button _back;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _click;

        private void Back()
        {
            _audioSource.PlayOneShot(_click);
            GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
        }

        private void Overall(float val)
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().SetOverallVolume(val);
        }

        private void Music(float val)
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().SetMusicVolume(val);
        }
        private void SfX(float val)
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().SetSfXVolume(val);
        }

        private void Sensivity(float val)
        {
            float sensitivity = Mathf.Lerp(PawsOfFireGameManager.player.GetSettings().sensitivityLimits.x, PawsOfFireGameManager.player.GetSettings().sensitivityLimits.y, val);
            PawsOfFireGameManager.player.GetSettings().sensitivity = new Vector2(sensitivity, sensitivity);
        }

        public override void Init()
        {
            _back.onClick.AddListener(Back);
            _overall.onValueChanged.AddListener(Overall);
            _music.onValueChanged.AddListener(Music);
            _sfx.onValueChanged.AddListener(SfX);
            _sensivity.onValueChanged.AddListener(Sensivity);

            _overall.value = 1;
            _music.value = 0.5f;
            _sfx.value = 1;

            Overall(_overall.value);
            Music(_music.value);
            SfX(_sfx.value);

            _sensivity.value = Mathf.InverseLerp(PawsOfFireGameManager.player.GetSettings().sensitivityLimits.x, PawsOfFireGameManager.player.GetSettings().sensitivityLimits.y, PawsOfFireGameManager.player.GetSettings().sensitivity.x);
        }
    }
}
