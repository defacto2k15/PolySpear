using Assets.Scripts.Units;

namespace Assets.Scripts.Symbols
{
    public class ShieldEffectContainer : IEffectContainer
    {
        public ShieldEffectContainer()
        {
            PassiveEffect = new EmptyEffect();
            ActiveEffect = new EmptyEffect();
            ReactEffect = new DefenseEffect();
        }

        public IEffect PassiveEffect { get; }
        public IEffect ActiveEffect { get; }
        public IEffect ReactEffect { get; }
    }
}