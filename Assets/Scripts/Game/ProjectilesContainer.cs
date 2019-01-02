using System.Linq;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class ProjectilesContainer : PawnsContainer<ProjectileModel>
    {
        public ProjectileModel AddProjectile(MyHexPosition position, Orientation orientation, GameObject projectilePrefab)
        {
            var projectile = Instantiate(projectilePrefab, transform);
            projectile.GetComponent<ProjectileModel>().Orientation = orientation;
            projectile.GetComponent<ProjectileModel>().Position = position;

            var model = projectile.GetComponent<ProjectileModel>();
            base.AddPawn(position, model);
            return model;
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