using Assets.Scripts.Units;

namespace Assets.Scripts.Symbols
{
    public class PushEffect : IEffect
    {
        public void Execute(EffectReciever effectReciever)
        {
            effectReciever.WasPushed = true;
        }
    }
}