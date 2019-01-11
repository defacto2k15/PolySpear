using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class CompositeAnimation : IAnimation
    {
        private readonly List<IAnimation> _innerAnimations;

        public CompositeAnimation(List<IAnimation> innerAnimations)
        {
            _innerAnimations = innerAnimations;
        }

        public void UpdateAnimation()
        {
            _innerAnimations.ForEach(c => c.UpdateAnimation());
        }

        public bool WeAreDuringAnimation()
        {
            return _innerAnimations.Any(c => c.WeAreDuringAnimation());
        }

        public void StartAnimation()
        {
            _innerAnimations.ForEach(c => c.StartAnimation());
        }
    }
}