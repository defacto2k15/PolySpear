using Assets.Scripts.Units;

namespace Assets.Scripts.Symbols
{
    public class SpearSymbolPassiveEffect : IEffect
    {
        public void Execute(EffectReciever effectReciever)
        {
            effectReciever.Kill();
        }
    }
}