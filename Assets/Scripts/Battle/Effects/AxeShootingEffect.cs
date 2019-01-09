using Assets.Scripts.Units;

namespace Assets.Scripts.Battle.Effects
{
    public class AxeShootingEffect : ProjectileShootingEffect
    {
        public AxeShootingEffect() : base(2, ProjectileType.Axe, EffectActivationCircumstances.DirectorBattle)
        {
        }
    }
}
