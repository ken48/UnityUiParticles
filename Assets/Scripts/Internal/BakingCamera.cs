using System;
using UnityEngine;

namespace UnityUiParticles.Internal
{
    internal class BakingCamera : MonoBehaviour
    {
        static BakingCamera _instance;
        Camera _camera;
        int _refCount;

        public static void RegisterConsumer()
        {
            if (_instance == null)
            {
                var gameObject = new GameObject(typeof(BakingCamera).Name);

                // This camera object is just for internal use
                gameObject.hideFlags = HideFlags.HideAndDontSave;

                _instance = gameObject.AddComponent<BakingCamera>();
                _instance._camera = gameObject.AddComponent<Camera>();

                // Turn camera off because particle mesh baker will use only camera matrix
                gameObject.SetActive(false);
            }
            _instance._refCount++;
        }

        public static void UnregisterConsumer()
        {
            if (_instance == null)
                throw new Exception("BakingCamera has no instance");

            _instance._refCount--;
            if (_instance._refCount == 0)
            {
                if (Application.isPlaying)
                    Destroy(_instance.gameObject);
                else
                    DestroyImmediate(_instance.gameObject);
                _instance = null;
            }
        }

        public static Camera GetCamera(Canvas canvas)
        {
            Camera currentCamera = _instance._camera;
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                var canvasWorldCamera = canvas.worldCamera;
                if (canvasWorldCamera != null)
                    currentCamera = canvasWorldCamera;
            }

            // Adjust camera orthographic size to canvas size
            // for canvas-based coordinates of particles' size and speed.
            Vector2 size = ((RectTransform)canvas.transform).rect.size;
            currentCamera.orthographic = true;
            currentCamera.orthographicSize = Mathf.Max(size.x, size.y) * canvas.scaleFactor;
            return currentCamera;
        }
    }
}
