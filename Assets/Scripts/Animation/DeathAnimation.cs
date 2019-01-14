using System.Collections.Generic;
using Assets.Scripts.Game;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    public class DeathAnimation : MyAnimation
    {
        private readonly List<SpriteRenderer> _renderers;
        private float _startTime;
        private bool _finished;

        public DeathAnimation(UnitModelComponent targetUnit) : base(targetUnit)
        {
            _renderers = new List<SpriteRenderer>();
            _renderers.AddRange(targetUnit.GetComponentsInChildren<SpriteRenderer>());
            _startTime = Time.time;
        }

        protected override void Update()
        {
            Color clr = Color.red;
            bool flip = (Mathf.Repeat(Time.time, Constants.DeathAnimationLoopLength) < Constants.DeathAnimationLoopLength / 1.6f);
            _renderers.ForEach(c => c.flipY = flip);
            _renderers.ForEach(c => c.color = new Color((flip?0.5f:0.75f),0,0,(flip ? 0.995f : 0.99f)));
        }

        protected override bool Finished => _startTime + Constants.DeathAnimationLength < Time.time;
    }
}