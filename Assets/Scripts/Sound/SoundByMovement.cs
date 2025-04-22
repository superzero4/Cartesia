using System;
using UnityEngine;

namespace Sound
{
    public class SoundByMovement : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Vector3 _multiplier;
        [SerializeField] private AnimationCurve _z;
        private Vector3 _status;

        void Awake()
        {
            _audioSource.loop = true;
            _audioSource.playOnAwake = false;
            _audioSource.mute = false;
            _audioSource.Play();
        }

        private void Update()
        {
            //Debug
            RawSet((Vector3.one + new Vector3(Mathf.Sin(Time.time),
                Mathf.Sin(Time.time),
                Mathf.Sin(Time.time))));
        }

        public void Play()
        {
            RawSet(Vector3.one / 2f);
            _audioSource.Play();
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public void Offset(Vector3 offset)
        {
            RawSet(_status + offset);
        }

        public void RawSet(Vector3 value)
        {
            _status = value;
            _audioSource.panStereo = (_status.x - 1) * 2 * _multiplier.x;
            _audioSource.volume = _status.y / 2f * _multiplier.y;
            _audioSource.pitch = _z.Evaluate(_status.z) * _multiplier.z;
        }
    }
}