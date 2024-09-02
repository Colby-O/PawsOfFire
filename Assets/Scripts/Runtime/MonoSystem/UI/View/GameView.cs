using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PawsOfFire.MonoSystem
{
    internal class GameView : View
    {
        [SerializeField] private TMP_Text _scoreDisplay;
        [SerializeField] private Slider _jumpGague;

        private int _score;

        public void UpdateGague(float val)
        {
            _jumpGague.value = val;
        }

        public void UpdateScore(int score)
        {
            _score = score;
            _scoreDisplay.text = $"Treats Collected: {_score}";
        }

        public override void Hide()
        {
            base.Hide();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public override void Show()
        {
            base.Show();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public override void Init()
        {
            
        }
    }
}
