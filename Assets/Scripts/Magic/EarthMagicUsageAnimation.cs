using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game;
using Assets.Scripts.Sound;
using UnityEngine;

namespace Assets.Scripts.Magic
{
    public class EarthMagicUsageAnimation : IAnimation
    {
        private GameObject _tile;
        private readonly CameraShake _shake;
        private readonly MasterSound _masterSound;

        private float _startTime;
        private Vector3 _oldScale;
        private Vector3 _targetScale;

        private bool _animationEnded = false;

        public EarthMagicUsageAnimation(GameObject tile, CameraShake shake, MasterSound masterSound)
        {
            _tile = tile;
            _shake = shake;
            _masterSound = masterSound;
        }

        public void UpdateAnimation()
        {
            if (_animationEnded)
            {
                return;
            }
            var a = (Time.time - _startTime) * Constants.EarthMagicAnimationFactor;
            if (a > 1)
            {
                _animationEnded = true;
                return;
            }
            _tile.transform.localScale = Vector3.Lerp(_oldScale, _targetScale, (float)a);

        }

        public bool WeAreDuringAnimation()
        {
            return !_animationEnded;
        }

        public void StartAnimation()
        {
            _shake.StartShake(2f);
            _startTime = Time.time;
            _oldScale = _tile.transform.localScale;
            _targetScale = new Vector3(0,0,0);
            _masterSound.PlayEarthMagicApplySound();
        }
    }

    public class WindMagicUsageAnimation : IAnimation
    {
        private GameObject _tile;
        private readonly CameraShake _shake;
        private readonly MasterSound _masterSound;

        private float _startTime;
        private Vector3 _oldScale;
        private Vector3 _targetScale;

        private bool _animationEnded = true;

        public WindMagicUsageAnimation(GameObject tile, CameraShake shake, MasterSound masterSound)
        {
            _tile = tile;
            _shake = shake;
            _masterSound = masterSound;
        }

        public void UpdateAnimation()
        {
            //if (_animationEnded)
            //{
            //    return;
            //}
            //var a = (Time.time - _startTime) * Constants.EarthMagicAnimationFactor;
            //if (a > 1)
            //{
            //    _animationEnded = true;
            //    return;
            //}
            //_tile.transform.localScale = Vector3.Lerp(_oldScale, _targetScale, (float)a);

        }

        public bool WeAreDuringAnimation()
        {
            return !_animationEnded;
        }

        public void StartAnimation()
        {
            _masterSound.PlayWindMagicApplySound();
            //_shake.StartShake(2f);
            //_startTime = Time.time;
            //_oldScale = _tile.transform.localScale;
            //_targetScale = new Vector3(0,0,0);
        }
    }
}
