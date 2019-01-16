using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game;
using Assets.Scripts.Sound;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Units
{
    public class BattleResults
    {
        private List<BattleEngagement> _engagements = new List<BattleEngagement>();

        public bool UnitIncapaciated(UnitModel unit)
        {
            return _engagements.Any(c => c.EngagementResult.UnitIncapaciated(unit));
        }

        public void Add(BattleEngagement engagement)
        {
            _engagements.Add(engagement);
        }

        public bool PositionWasFreed(MyHexPosition target)
        {
            return _engagements.Any(c => c.EngagementResult.PositionWasFreed(target));
        }

        public static BattleResults Empty => new BattleResults();
        //public List<UnitModel> KilledUnits => _engagements.SelectMany(c => c.EngagementResult.StruckUnits).Distinct().ToList();
        //public List<UnitDisplacements> Displacements => _engagements.SelectMany(c => c.EngagementResult.Displacements).Distinct().ToList();
        //public List<ProjectileCreations> Projectiles  => _engagements.SelectMany(c => c.EngagementResult.Projectiles).Distinct().ToList();
        public List<BattleEngagement> Engagements => _engagements;

        public bool UnitWasStruck(UnitModel locomotionTargetModel)
        {
            return _engagements.Any(c => c.EngagementResult.UnitWasStruck(locomotionTargetModel));
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

    public class BattleEngagement
    {
        private List<EngagementElement> _engagementElements = new List<EngagementElement>();
        private BattleEngagementResult _engagementResult;

        public BattleEngagement( BattleEngagementResult engagementResult)
        {
            _engagementResult = engagementResult;
        }

        public BattleEngagementResult EngagementResult => _engagementResult;

        public List<EngagementElement> EngagementElements => _engagementElements;

        public void AddEngagementElement(EngagementElement element)
        {
            _engagementElements.Add(element);
        }
    }

    public class BattleEngagementResult
    {
        private List<UnitModel> _struckUnits = new List<UnitModel>();
        private List<UnitDisplacements> _displacements = new List<UnitDisplacements>();
        private List<ProjectileCreations> _projectiles = new List<ProjectileCreations>();

        public List<UnitModel> StruckUnits => _struckUnits;

        public List<UnitDisplacements> Displacements => _displacements;

        public List<ProjectileCreations> Projectiles => _projectiles;

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

        public bool UnitIncapaciated(UnitModel unit)
        {
            return _struckUnits.Contains(unit) || _displacements.Any(c => c.Unit == unit);
        }

        public bool PositionWasFreed(MyHexPosition target)
        {
            return _struckUnits.Any(c => c.Position.Equals(target)) || _displacements.Any(c => c.DisplacementStart.Equals(target));
        }
    }

    public class EngagementElement
    {
        public PawnModel ActivePawn;
        public PawnModel PassivePawn;
        public EngagementVisibleConsequence EngagementVisibleConsequence;
    }

    public class EngagementVisibleConsequence
    {
        private readonly Func<GameCourseModel, MasterSound, PawnModelComponent, PawnModelComponent, IAnimation> _effectUsageAnimationGenerator;

        public EngagementVisibleConsequence(Func<GameCourseModel, MasterSound, PawnModelComponent, PawnModelComponent, IAnimation> effectUsageAnimationGenerator)
        {
            _effectUsageAnimationGenerator = effectUsageAnimationGenerator;
        }

        public IAnimation EngagementAnimation(GameCourseModel courseMode, MasterSound sound, PawnModelComponent active, PawnModelComponent passive) => _effectUsageAnimationGenerator(courseMode,sound, active, passive);
    }
}
