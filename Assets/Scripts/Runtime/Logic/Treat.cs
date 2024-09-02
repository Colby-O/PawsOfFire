using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PawsOfFire.Logic
{
    internal class Treat : MonoBehaviour
    {
        private float _lifespan;
        private bool _isDestoryed;
        private int _loc;

        private Color _orig;
        private Color _blink = Color.clear;

        private float _blinkTime = 0f;

        public float GetLifespan()
        {
            return _lifespan;
        }

        public bool IsDestoryed()
        {
            return _isDestoryed;
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        public void SetLocationID(int id)
        {
            _loc = id;
        }

        public int GetLocationID()
        {
            return _loc;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && !_isDestoryed)
            {
                PawsOfFireGameManager.player.AddScore();
                _isDestoryed = true;
            }
        }

        private void Awake()
        {
            _lifespan = 0f;
            _blinkTime = 0;
            _isDestoryed = false;
            _orig = GetComponent<MeshRenderer>().material.color;
        }

        private void Update()
        {
            _lifespan += Time.deltaTime;

            if (_lifespan > 25)
            {
                if (_blinkTime > 0.1f)
                {
                    if (GetComponent<MeshRenderer>().material.color == _orig) GetComponent<MeshRenderer>().material.color = _blink;
                    else if (GetComponent<MeshRenderer>().material.color == _blink) GetComponent<MeshRenderer>().material.color = _orig;
                    _blinkTime = 0;
                }
                _blinkTime += Time.deltaTime;
            }
        }
    }
}
