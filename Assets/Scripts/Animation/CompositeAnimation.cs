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

    public class SequenceAnimation : IAnimation
    {
        private readonly Queue<IAnimation> _innerAnimations;

        public SequenceAnimation(List<IAnimation> innerAnimations)
        {
            _innerAnimations = new Queue<IAnimation>(innerAnimations);
        }

        public void UpdateAnimation()
        {
            if (_innerAnimations.Any())
            {
                if (!_innerAnimations.Peek().WeAreDuringAnimation())
                {
                    _innerAnimations.Dequeue();
                    if (_innerAnimations.Any())
                    {
                        _innerAnimations.Peek().StartAnimation();
                    }
                    else
                    {
                        return;
                    }
                }
            _innerAnimations.Peek().UpdateAnimation();
            }
        }

        public bool WeAreDuringAnimation()
        {
            return  _innerAnimations.Any();
        }

        public void StartAnimation()
        {
            if (_innerAnimations.Any())
            {
                _innerAnimations.Peek().StartAnimation();
            }
        }
    }
}