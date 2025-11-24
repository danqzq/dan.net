using UnityEngine;

namespace Dan.Net
{
    [RequireComponent(typeof(SyncObject))]
    public class TransformSync : MonoBehaviourDanNet, ISyncData
    {
        private enum UpdateMode
        {
            Normal,
            Fixed,
            Late
        }
        
        [SerializeField] private UpdateMode _updateMode = UpdateMode.Normal;
        
        [Header("Sync Settings")]
        [SerializeField] private bool _syncPosition = true;
        [SerializeField] private bool _syncRotation = true;
        
        [Header("Smoothing Settings")]
        [SerializeField] private bool _smoothing = true;
        [SerializeField] private float _lagCompensationFactor = 10f;

        private Vector3 _targetPosition, _lastPosition;
        private Quaternion _targetRotation, _lastRotation;

        private Vector3 _velocity;
        
        private double _currentTime;
        private double _lastSendTime, _currentSendTime;

        public Vector3 TargetPosition => _targetPosition;
        public Quaternion TargetRotation => _targetRotation;

        private void Start()
        {
            _targetPosition = transform.position;
            _targetRotation = transform.rotation;
        }

        private Vector3 _lastSentPosition;
        private Quaternion _lastSentRotation;
        
        private const float POSITION_THRESHOLD = 0.01f;
        private const float ROTATION_THRESHOLD = 1f;

        public void OnDataRead(in SyncDataStream stream)
        {
            var data = stream.ReceiveTransform();
            
            if (!data.hasPosition && !data.hasRotation)
            {
                return; // No data for this object
            }
            
            _currentTime = 0f;

            _lastSendTime = _currentSendTime;
            _currentSendTime = stream.serverSentTime;
            
            if (data.hasPosition && _syncPosition)
            {
                _lastPosition = transform.position;
                _targetPosition = data.position;
            }
            
            if (data.hasRotation && _syncRotation)
            {
                _lastRotation = transform.rotation;
                _targetRotation = data.rotation;
            }
        }

        public void OnDataSend(in SyncDataStream stream)
        {
            Vector3? pos = null;
            Quaternion? rot = null;
            
            if (_syncPosition && Vector3.Distance(_lastSentPosition, transform.position) > POSITION_THRESHOLD)
            {
                pos = transform.position;
                _lastSentPosition = pos.Value;
            }

            if (_syncRotation && Quaternion.Angle(_lastSentRotation, transform.rotation) > ROTATION_THRESHOLD)
            {
                rot = transform.rotation;
                _lastSentRotation = rot.Value;
            }
            
            if (pos.HasValue || rot.HasValue)
            {
                stream.SendTransform(pos, rot);
            }
        }

        private void Move(UpdateMode updateMode)
        {
            if (syncObject.IsMine || updateMode != _updateMode)
                return;

            var deltaTime = updateMode == UpdateMode.Fixed ? Time.fixedDeltaTime : Time.deltaTime;
            _currentTime += deltaTime;
            
            var updateInterval = _currentSendTime - _lastSendTime;
            if (updateInterval <= 0)
            {
                if (_syncPosition)
                {
                    transform.position = _targetPosition;
                }
                
                if (_syncRotation)
                {
                    transform.rotation = _targetRotation;
                }

                return;
            }
            
            var t = (float)(_currentTime / updateInterval);
            
            var extrapolationAmount = _lagCompensationFactor * 0.1f;
            t = Mathf.Clamp(t + extrapolationAmount, 0f, 1.5f);
            
            if (_syncPosition)
            {
                if (_smoothing)
                {
                    var targetPos = Vector3.Lerp(_lastPosition, _targetPosition, t);
                    transform.position = Vector3.SmoothDamp(
                        transform.position, 
                        targetPos, 
                        ref _velocity, 
                        0.1f,
                        Mathf.Infinity,
                        deltaTime
                    );
                }
                else
                {
                    transform.position = Vector3.Lerp(_lastPosition, _targetPosition, t);
                }
            }
            
            if (_syncRotation)
            {
                if (_smoothing)
                {
                    var targetRot = Quaternion.Slerp(_lastRotation, _targetRotation, t);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRot,
                        deltaTime * 10f
                    );
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(_lastRotation, _targetRotation, t);
                }
            }
        }

        private void Update() => Move(UpdateMode.Normal);
        private void FixedUpdate() => Move(UpdateMode.Fixed);
        private void LateUpdate() => Move(UpdateMode.Late);
    }
}