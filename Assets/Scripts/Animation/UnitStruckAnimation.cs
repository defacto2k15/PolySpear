using System.Collections.Generic;
using Assets.Scripts.Game;
using Assets.Scripts.Sound;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    public class UnitStruckAnimation : MyAnimation
    {
        private readonly SpriteRenderer _renderer;
        private float _startTime;
        private bool _finished;

        public UnitStruckAnimation(UnitModelComponent targetUnit) : base(targetUnit)
        {
            foreach (Transform childTransform in targetUnit.transform)
            {
                if (childTransform.name.Equals("UnitSprite")) //todo TL3
                {
                    _renderer = (childTransform.GetComponent<SpriteRenderer>());
                }
            }
            _startTime = Time.time;
        }

        protected override void Update()
        {
            Color color = Color.white;
            if (CurrentTime > Constants.UnitStruckRedStart && CurrentTime < (Constants.UnitStruckRedStart + Constants.UnitStruckRedDuration) )
            {
                color = Color.red;
            }

            if (_renderer != null)
            {
                _renderer.color = color;
            }
        }

        protected override void MyFinish()
        {
            if (_renderer != null)
            {
                _renderer.color = Color.white;
            }
        }


        protected override bool Finished => CurrentTime > Constants.UnitStruckAnimationDuration;

        private float CurrentTime => Time.time - _startTime;
    }

    public class DefenseAnimation : MyAnimation
    {
        private readonly MasterSound _masterSound;
        private readonly SpriteRenderer _renderer;
        private float _startTime;
        private bool _finished;
        private bool _audioStarted = false;

        public DefenseAnimation(PawnModelComponent targetUnit, MasterSound masterSound) : base(targetUnit)
        {
            _masterSound = masterSound;
            foreach (Transform childTransform in targetUnit.transform)
            {
                if (childTransform.name.Equals("UnitSprite")) //todo TL3
                {
                    _renderer = (childTransform.GetComponent<SpriteRenderer>());
                }
            }
            _startTime = Time.time;
        }

        protected override void Update()
        {
            Color color = Color.white;
            if (CurrentTime > Constants.UnitDefenseGoldStart&& CurrentTime < (Constants.UnitDefenseGoldStart+ Constants.UnitDefenseGoldDuration) )
            {
                color = Color.green;
                if (!_audioStarted)
                {
                    _masterSound.PlayDefenseSound();
                    _audioStarted = true;
                }
            }

            if (_renderer != null)
            {
                _renderer.color = color;
            }
        }

        protected override void MyFinish()
        {
            if (_renderer != null)
            {
                _renderer.color = Color.white;
            }
        }


        protected override bool Finished => CurrentTime > Constants.UnitDefenseGoldDuration;

        private float CurrentTime => Time.time - _startTime;
    }

}