using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class UnitView : PawnView
    {
        private UnitModel _unitModel;
        private UnitFlagView _flagChild;

        public AudioClip AttackClip;
        public AudioClip DeathClip;
        public AudioClip MoveClip;

        protected override void MyStart()
        {
            _flagChild = GetComponentInChildren<UnitFlagView>();
            _unitModel = GetComponent<UnitModelComponent>().Model;

            var audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("W43 No audio source on unit");
            }
            else
            {
                _unitModel.OnStepEvent += () =>
                {
                    audioSource.clip = MoveClip;
                    audioSource.Play();
                };

                _unitModel.OnAttackEvent += () =>
                {
                    audioSource.clip = AttackClip;
                    audioSource.Play();
                };

                _unitModel.OnDeathEvent += () =>
                {
                    audioSource.clip = DeathClip;
                    audioSource.Play();
                };
            }
        }

        protected override void MyUpdate()
        {
            UpdateView(); // very wasteful, but works!
        }

        private void UpdateView()
        {
            if (_flagChild == null) // todo more elegant solution
            {
                _flagChild = GetComponentInChildren<UnitFlagView>();
                _unitModel = GetComponent<UnitModelComponent>().Model;
            }
            if (_flagChild != null)
            {
                _flagChild.SetFlagColor(Constants.PlayersFlagColors[_unitModel.Owner]);
            }
        }
    }
}
