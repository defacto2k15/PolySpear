using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Units
{
    public class BattleResults
    {
        private List<UnitModel> _struckUnits = new List<UnitModel>();
        private List<UnitDisplacements> _displacements = new List<UnitDisplacements>();
        private List<ProjectileCreations> _projectiles = new List<ProjectileCreations>();

        public List<UnitModel> StruckUnits => _struckUnits;
        public List<UnitDisplacements> Displacements => _displacements;
        public List<ProjectileCreations> Projectiles => _projectiles;

        public List<UnitModel> KilledUnits => StruckUnits;

        public void AddStruckUnit(UnitModel unit)
        {
            _struckUnits.Add(unit);
        }

        public bool UnitWasStruck(UnitModel unit)
        {
            return _struckUnits.Contains(unit);
        }

        public void UnStrike(UnitModel unit)
        {
            Assert.IsTrue(UnitWasStruck(unit), "Unit was not stuck, cannot unstuck");
            _struckUnits.Remove(unit);
        }

        public void DisplaceUnit(UnitModel unit, MyHexPosition newPosition)
        {
            _displacements.Add(new UnitDisplacements()
            {
                Unit = unit,
                DisplacementStart = unit.Position,
                DisplacementEnd = newPosition
            });
        }

        public bool UnitIncapaciated(UnitModel unit)
        {
            return _struckUnits.Contains(unit) || _displacements.Any(c => c.Unit == unit);
        }

        public void Add(BattleResults otherResults)
        {
            _struckUnits.AddRange(otherResults.StruckUnits);
            _struckUnits = _struckUnits.Distinct().ToList();

            _displacements.AddRange(otherResults.Displacements);
            _displacements = _displacements.Where(c => !_struckUnits.Contains(c.Unit)).Distinct().ToList();

            _projectiles.AddRange(otherResults.Projectiles);
        }

        public bool PositionWasFreed(MyHexPosition target)
        {
            return _struckUnits.Any(c => c.Position.Equals(target)) || _displacements.Any(c => c.DisplacementStart.Equals(target));
        }

        public static BattleResults Empty => new BattleResults();

        public void AddProjectile(MyHexPosition startPosition, Orientation orientation, MyHexPosition endPosition, ProjectileType type)
        {
            _projectiles.Add(new ProjectileCreations()
            {
                StartPosition = startPosition,
                EndPosition = endPosition,
                Orientation = orientation,
                Type = type
            });
        }
    }

    public class UnitDisplacements
    {
        public UnitModel Unit;
        public MyHexPosition DisplacementStart;
        public MyHexPosition DisplacementEnd;
    }

    public class ProjectileCreations
    {
        public MyHexPosition StartPosition;
        public MyHexPosition EndPosition;
        public Orientation Orientation;
        public ProjectileType Type;
    }

    public enum ProjectileType
    {
        Arrow, Laser, Axe
    }
}
