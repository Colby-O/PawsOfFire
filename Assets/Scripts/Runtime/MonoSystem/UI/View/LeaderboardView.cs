using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PawsOfFire.MonoSystem
{
    internal class LeaderboardView : View
    {
        [SerializeField] private Button _back;
        [SerializeField] private LeaderBoards _leaderboards;
        [SerializeField] private float _leaderboardsRefreshRate = 10;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _click;

        private Coroutine _refresh;

        private void Back()
        {
            _audioSource.PlayOneShot(_click);
            GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
        }

        private IEnumerator RefeshLeaderboards()
        {
            while (true)
            {
                _leaderboards.Reload();
                yield return new WaitForSeconds(_leaderboardsRefreshRate);
            }
        }

        public override void Hide()
        {
            base.Hide();
            if (_refresh != null) StopCoroutine(_refresh);
        }

        public override void Show()
        {
            base.Show();
            _refresh = StartCoroutine(RefeshLeaderboards());
        }

        public override void Init()
        {
            _back.onClick.AddListener(Back);
        }
    }
}
