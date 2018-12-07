using Assets.Scripts.Units;

namespace Assets.Scripts.Symbols
{
    public class SpearSymbolActiveEffect : IEffect
    {
        public void Execute(EffectReciever effectReciever)
        {
            effectReciever.Kill();
        }
    }
}