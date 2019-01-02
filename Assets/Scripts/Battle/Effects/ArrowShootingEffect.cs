using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Units;

namespace Assets.Scripts.Battle.Effects
{
    public class ArrowShootingEffect : ProjectileShootingEffect
    {
        public ArrowShootingEffect() : base(8, ProjectileType.Arrow)
        {
        }
    }
}
