using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Animation
{
    public class EmptyAnimation : IAnimation
    {
        public EmptyAnimation()
        {
        }

        public void UpdateAnimation()
        {
        }

        public bool WeAreDuringAnimation()
        {
            return false;
        }

        public void StartAnimation()
        {
        }
    }
}