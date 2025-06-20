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

        private Vector3 _interpolatePosition;
        private Quaternion _interpolateRotation;

        private Vector3 _velocity;
        
        private double _currentTime;
        private double _lastSendTime, _currentSendTime;
        
        private float _previousLerpValue;

        private void Start()
        {
            _interpolatePosition = _targetPosition = transform.position;
            _interpolateRotation = _targetRotation = transform.rotation;
        }

        public void OnDataRead(in SyncDataStream stream)
        {
            _currentTime = 0f;

            _lastSendTime = _currentSendTime;
            _currentSendTime = stream.serverSentTime;
            
            if (_syncPosition)
            {
                var x = (double) stream.Receive();
                var y = (double) stream.Receive();
                var z = (double) stream.Receive();
                
                _lastPosition = transform.position;
                _targetPosition = new Vector3((float) x, (float) y, (float) z);
            }
            
            if (_syncRotation)
            {
                var x = (double) stream.Receive();
                var y = (double) stream.Receive();
                var z = (double) stream.Receive();

                _lastRotation = transform.rotation;
                _targetRotation = Quaternion.Euler((float) x, (float) y, (float) z);
            }
        }

        public void OnDataSend(in SyncDataStream stream)
        {
            if (_syncPosition)
            {
                var position = transform.position;
                stream.Send((double) position.x);
                stream.Send((double) position.y);
                stream.Send((double) position.z);
            }
            
            if (_syncRotation)
            {
                var rotation = transform.eulerAngles;
                stream.Send((double) rotation.x);
                stream.Send((double) rotation.y);
                stream.Send((double) rotation.z);
            }
        }

        private void Move(UpdateMode updateMode)
        {
            const float smoothTime = 0.1f;
            
            if (syncObject.IsMine || updateMode != _updateMode)
                return;

            var timeToReachGoal = (_currentSendTime - _lastSendTime) * 0.5f;
            var deltaTime = updateMode == UpdateMode.Fixed ? Time.fixedDeltaTime : Time.deltaTime;
            _currentTime += deltaTime;
            
            var val = Mathf.LerpUnclamped(_previousLerpValue, timeToReachGoal == 0 ? 1f
                : (float) (_currentTime / timeToReachGoal), deltaTime * _lagCompensationFactor);
            
            if (_syncPosition) 
                _interpolatePosition = Vector3.Lerp(_lastPosition, _targetPosition, val);
            
            if (_syncRotation)
                _interpolateRotation = Quaternion.Lerp(_lastRotation, _targetRotation, val);

            if (_smoothing)
            {
                var pos = transform.position;
                var rot = transform.rotation;
                transform.position = Vector3.SmoothDamp(pos, _interpolatePosition, ref _velocity, smoothTime);
                transform.rotation = Quaternion.Slerp(rot, _interpolateRotation, smoothTime);
            }
            else
            {
                transform.position = _interpolatePosition;
                transform.rotation = _interpolateRotation;
            }

            _previousLerpValue = val;
        }

        private void Update() => Move(UpdateMode.Normal);
        private void FixedUpdate() => Move(UpdateMode.Fixed);
        private void LateUpdate() => Move(UpdateMode.Late);
    }
}