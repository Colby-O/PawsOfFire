using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PawsOfFire.MonoSystem
{
    internal class MenuCamera : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _menuCam;
        [SerializeField] private RawImage _view;

        private RenderTexture _rt;

        private RenderTexture CreateRenderTexture()
        {
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            rt.Create();
            return rt;
        }

        private void OnRectTransformDimensionsChange()
        {
            _rt = CreateRenderTexture();
            _menuCam.targetTexture = _rt;
            _view.texture = _rt;
        }

        private void Awake()
        {
            _rt = CreateRenderTexture();
            _menuCam.targetTexture = _rt;
            _view.texture = _rt;
        }
    }
}
