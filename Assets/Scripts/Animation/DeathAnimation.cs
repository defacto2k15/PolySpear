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

        public DeathAnimation(UnitModel targetUnit) : base(targetUnit)
        {
            _renderers = new List<SpriteRenderer>();
            _renderers.Add(targetUnit.GetComponent<SpriteRenderer>());
            _renderers.AddRange(targetUnit.GetComponentsInChildren<SpriteRenderer>());
            _startTime = Time.time;
        }

        protected override void Update()
        {
            Debug.Log("W13 UPDATE!!");
            bool flip = (Mathf.Repeat(Time.time, Constants.DeathAnimationLoopLength) < Constants.DeathAnimationLoopLength / 2f);
            _renderers.ForEach(c => c.flipY = flip);
        }

        protected override bool Finished => _startTime + Constants.DeathAnimationLength < Time.time;
    }
}