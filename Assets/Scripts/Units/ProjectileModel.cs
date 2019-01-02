using Assets.Scripts.Symbols;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class ProjectileModel : PawnModel
    {
        //[SerializeField]
        //public SerializableIEffect ProjectileSerializableEffect;
        //public IEffect Effect => ProjectileSerializableEffect.Value;

        public ProjectileModel Clone()
        {
            return new ProjectileModel()
            {
                Position = Position,
                Orientation = Orientation,
                IsUnitAlive = IsUnitAlive,
                //ProjectileSerializableEffect = ProjectileSerializableEffect
            };
        }
    }
}