using System.Linq;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class ProjectilesContainer : PawnsContainer<ProjectileModel>
    {
        public ProjectileModel AddProjectile(MyHexPosition position, Orientation orientation)
        {
            var projectile = new ProjectileModel
            {
                Orientation = orientation,
                Position = position
            };

            base.AddPawn(position, projectile);
            return projectile;
        }

        public ProjectilesContainer Clone()
        {
            return new ProjectilesContainer()
            {
                _pawns = _pawns.ToDictionary(c => c.Key, c => c.Value.Clone())
            };
        }
    }
}