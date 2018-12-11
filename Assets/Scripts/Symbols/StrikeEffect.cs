using Assets.Scripts.Units;

namespace Assets.Scripts.Symbols
{
    public class StrikeEffect : IEffect
    {
        public void Execute(EffectReciever effectReciever)
        {
            effectReciever.WasStruck = true;
        }
    }
}