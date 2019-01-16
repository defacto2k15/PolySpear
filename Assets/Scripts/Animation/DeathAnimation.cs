using System.Collections.Generic;
using Assets.Scripts.Game;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    public class DeathAnimation : MyAnimation
    {
        private readonly List<SpriteRenderer> _renderers;
        private UnitModel _model;
        private float _startTime;
        private bool _finished;

        public DeathAnimation(UnitModelComponent targetUnit) : base(targetUnit)
        {
            _model = targetUnit.Model;
            _renderers = new List<SpriteRenderer>();
            _renderers.AddRange(targetUnit.GetComponentsInChildren<SpriteRenderer>());
            _startTime = Time.time;
        }

        protected override void Update()
        {
            bool flip = (Mathf.Repeat(Time.time, Constants.DeathAnimationLoopLength) < Constants.DeathAnimationLoopLength / 2f);
            _renderers.ForEach(c => c.flipY = flip);
        }

        protected override void MyStart()
        {
            _model.OnDeath();
        }

        protected override bool Finished => _startTime + Constants.DeathAnimationLength < Time.time;
    }
}