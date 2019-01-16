using Assets.Scripts.Units;

namespace Assets.Scripts.Battle.Effects
{
    public class LaserShootingEffect : ProjectileShootingEffect
    {
        public LaserShootingEffect() : base(3, ProjectileType.Laser, EffectActivationCircumstances.DirectorBattle)
        {
        }
    }
}