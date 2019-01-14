using Assets.Scripts.Animation;
using Assets.Scripts.Battle.Effects;
using Assets.Scripts.Game;
using Assets.Scripts.Symbols;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class ProjectileModel : PawnModel
    {
        public IEffect HitEffect => new ArrowStrikeEffect();

        public ProjectileModel Clone()
        {
            return new ProjectileModel()
            {
                Position = Position,
                Orientation = Orientation,
                IsUnitAlive = IsUnitAlive,
                Owner = Owner,
            };
        }
    }
}