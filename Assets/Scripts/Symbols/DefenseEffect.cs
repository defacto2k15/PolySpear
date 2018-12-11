using Assets.Scripts.Units;

namespace Assets.Scripts.Symbols
{
    public class DefenseEffect : IEffect
    {
        public void Execute(EffectReciever effectReciever)
        {
            effectReciever.WasStruck = false;
        }
    }
}