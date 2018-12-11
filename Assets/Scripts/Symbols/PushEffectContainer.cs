using Assets.Scripts.Units;

namespace Assets.Scripts.Symbols
{
    public class PushEffectContainer : IEffectContainer
    {
        public PushEffectContainer()
        {
            PassiveEffect = new EmptyEffect();
            ActiveEffect = new PushEffect();
            ReactEffect = new EmptyEffect();
        }

        public IEffect PassiveEffect { get; }
        public IEffect ActiveEffect { get; }
        public IEffect ReactEffect { get; }
    }
}