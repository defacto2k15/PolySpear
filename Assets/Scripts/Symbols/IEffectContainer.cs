using Assets.Scripts.Units;

namespace Assets.Scripts.Symbols
{
    public interface IEffectContainer {
        IEffect PassiveEffect { get; }
        IEffect ActiveEffect { get; }
        IEffect ReactEffect { get; }
    }
}