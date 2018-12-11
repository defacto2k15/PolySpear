using Assets.Scripts.Units;

namespace Assets.Scripts.Symbols
{
    public interface ISymbolModelTODODELETE
    {
        Orientation LocalOrientation { get;}
        IEffect PassiveEffect { get; }
        IEffect ActiveEffect { get;}
    }
}