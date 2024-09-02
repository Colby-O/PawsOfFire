using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PawsOfFire.Player
{
    [RequireComponent(typeof(PlayerInput))]
    internal sealed class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _target;

        [Header("Camera Settings")]
        [SerializeField] private Vector3 _cameraOffset = new Vector3(0.0f, 5.0f, -20.0f);
        [SerializeField] private PlayerSettings _settings;
        [SerializeField] private Vector2 _zoomLimits;
        [SerializeField][Range(-90, 0)] private float _minViewY;
        [SerializeField][Range(0, 90)] private float _maxViewY;
        [SerializeField] private LayerMask _ignoreCameraCollisionMask;
        [SerializeField] private LayerMask _groundMask;

        private Camera _cam;
        [SerializeField] private Vector2 _cameraAngle;
        private float _cameraOffsetZ;
        private float _cameraOffsetY;
        private float _mouseScrollY;

        private PlayerInput _playerInput;
        private InputAction _lookAction;
        private InputAction _zoomAction;

        private Vector2 _rawMousePosition;

        private Vector2 _previousCameraAngle;

        /// <summary>
        /// Rotates around the player
        /// </summary>
        private void RotateAroundTarget(Vector3 axis, float angle)
        {
            transform.RotateAround(_target.position, axis, angle);
        }

        /// <summary>
        /// Makes the camera follow the player
        /// </summary>
        private void FollowPlayer()
        {
            Vector3 cameraPosition = new(_target.position.x, _target.position.y + _cameraOffsetY, _target.position.z + _cameraOffsetZ);
            transform.position = cameraPosition;
            transform.rotation = Quaternion.identity;
        }

        private Vector3 CalculatePointOutsideTarget(Vector3 pt)
        {
            Collider collider = _target.GetComponent<Collider>();

            float radius = Mathf.Max(collider.bounds.size.x, collider.bounds.size.y, collider.bounds.size.z);
            Vector3 center = collider.bounds.center;

            if (Mathf.Pow(pt.x - center.x, 2) + Mathf.Pow(pt.y - center.y, 2) + Mathf.Pow(pt.z - center.z, 2) <= Mathf.Pow(radius, 2))
            {
                Debug.Log("Here!");
                Vector3 newPoint =  center + (radius / (pt - center).magnitude) * (pt - center);
                return new Vector3(newPoint.x, pt.y, newPoint.z);
            }

            return pt;
        }

        /// <summary>
        /// calculates the raycast to the camera with an offset
        /// and returns the distance and the resulting position
        /// of the camera if there was a collision
        /// </summary>
        private (float distance, Vector3 newPos, LayerMask layer) CalculateCameraCollisionWithOffset(Vector3 offset)
        {
            Vector3 direction = transform.position + offset - _target.position;
            Ray ray = new(_target.position, direction.normalized);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, direction.magnitude, ~_ignoreCameraCollisionMask))
            {
                return (hitInfo.distance, hitInfo.point - offset, hitInfo.transform.gameObject.layer);  
            }

            return (float.MaxValue, Vector3.zero, 0);
        }

        /// <summary>
        /// Check if there is any object in the way of the cameras view of the player
        /// </summary>
        private void CheckCameriaCollision()
        {
            // calculate the size of the viewport in world space
            float viewHeight = 2.0f * Mathf.Tan(Mathf.Deg2Rad * _cam.fieldOfView * 0.5f) * _cam.nearClipPlane;
            float viewWidth = viewHeight / _cam.pixelHeight * _cam.pixelWidth;

            // check all 4 corners of the camera viewport in world space
            // to insure they are not intersecting any objects
            (float distance, Vector3 newPos, LayerMask layer)[] corners = {
                CalculateCameraCollisionWithOffset(
                    -_cam.transform.right * viewWidth / 2.0f + _cam.transform.up * viewHeight / 2.0f
                ),
                CalculateCameraCollisionWithOffset(
                    _cam.transform.right * viewWidth / 2.0f + _cam.transform.up * viewHeight / 2.0f
                ),
                CalculateCameraCollisionWithOffset(
                    -_cam.transform.right * viewWidth / 2.0f - _cam.transform.up * viewHeight / 2.0f
                ),
                CalculateCameraCollisionWithOffset(
                    _cam.transform.right * viewWidth / 2.0f - _cam.transform.up * viewHeight / 2.0f
                ),
            };

            if (corners.Any(c => c.distance != float.MaxValue))
            {
                // set camera pos to the closest intersection
                (float distance, Vector3 newPos, LayerMask layer) obj = corners.Aggregate(
                    (m, n) => m.distance < n.distance ? m : n
                );

                if (((1 << obj.layer) & _groundMask) != 0) 
                {
                    _previousCameraAngle.y -= 1.0f;
                    _cameraAngle = _previousCameraAngle;
                    ProcessCamera();
                } else
                {
                    transform.position = obj.newPos;
                }
            }
        }

        /// <summary>
        /// Updates the angle of the camera
        /// </summary>
        private void UpdateCameraAngle()
        {
            _cameraAngle += _rawMousePosition * _settings.sensitivity;
            _cameraAngle.y = Mathf.Clamp(_cameraAngle.y, _minViewY, _maxViewY);
        }

        /// <summary>
        /// Process how far the camera should orbit the player
        /// </summary>
        private void ProcessCameraZoom()
        {
            if (_mouseScrollY > 0 && _cameraOffsetZ < -_zoomLimits.x) _cameraOffsetZ += 1;
            else if (_mouseScrollY < 0 && _cameraOffsetZ > -_zoomLimits.y) _cameraOffsetZ -= 1;
        }

        /// <summary>
        /// Process all camera movement
        /// </summary>
        private void ProcessCamera()
        {
            _previousCameraAngle = _cameraAngle;
            ProcessCameraZoom();
            FollowPlayer();
            UpdateCameraAngle();
            RotateAroundTarget(transform.up, _cameraAngle.x);
            RotateAroundTarget(-transform.right, _cameraAngle.y);
            CheckCameriaCollision();
        }

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            _cameraOffsetZ = _cameraOffset.z;
            _cameraOffsetY = _cameraOffset.y;
            _cameraAngle = new(0, 0);

            _lookAction = _playerInput.actions["View"];
            _zoomAction = _playerInput.actions["Zoom"];

            _lookAction.performed += e => _rawMousePosition = e.ReadValue<Vector2>();
            _zoomAction.performed += e => _mouseScrollY = e.ReadValue<float>();
        }

        private void LateUpdate()
        {
            if (!PawsOfFireGameManager.allowInput) return;
            ProcessCamera();
        }
    }
}
