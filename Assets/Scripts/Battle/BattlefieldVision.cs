using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game;
using Assets.Scripts.Map;
using Assets.Scripts.Positions;
using Assets.Scripts.Units;

namespace Assets.Scripts.Battle
{
    public class BattlefieldVision
    {
        private UnitModel _posessedUnit;
        private readonly Orientation _directionOfAttack;
        private UnitsContainer _units;
        private MapModel _map;
        private BattleCircumstances _battleCircumstances;

        public BattlefieldVision(UnitModel posessedUnit, Orientation directionOfAttack, UnitsContainer units, MapModel map, BattleCircumstances battleCircumstances)
        {
            _posessedUnit = posessedUnit;
            _directionOfAttack = directionOfAttack;
            _units = units;
            _map = map;
            _battleCircumstances = battleCircumstances;
        }

        public UnitModel GetUnitAt(MyHexPosition localPosition)
        {
            if (!_units.IsPawnAt(ToGlobalPosition(localPosition)))
            {
                return null;
            }
            return _units.GetPawnAt(ToGlobalPosition(localPosition));
        }

        public UnitModel PossesedUnit => _posessedUnit;
        public BattleCircumstances BattleCircumstances => _battleCircumstances;

        public MyHexPosition ToGlobalPosition(MyHexPosition myHexPosition)
        {
            var transformator = new BattlefieldPointOfViewTransformator(_posessedUnit.Position, _posessedUnit.Orientation.AddRotation(_directionOfAttack));
            return transformator.ToGlobalPosition(myHexPosition);
        }

        public bool HasTileAt(MyHexPosition pos)
        {
            return _map.HasTileAt(pos);
        }
    }

    public class BattlefieldPointOfViewTransformator
    {
        private MyHexPosition _globalCenterPosition;
        private Orientation _globalCenterOrientation;

        public BattlefieldPointOfViewTransformator(MyHexPosition globalCenterPosition, Orientation globalCenterOrientation)
        {
            _globalCenterPosition = globalCenterPosition;
            _globalCenterOrientation = globalCenterOrientation;
        }

        public MyHexPosition ToLocalPosition(MyHexPosition globalPosition)
        {
            var localPositionBeforeOrientation = globalPosition - _globalCenterPosition;
            var localPositionCube = PositionUtils.AxialToCube(localPositionBeforeOrientation);
            for (int i = 0; i < _globalCenterOrientation.ClockwiseIndex(); i++)
            {
                localPositionCube = RotateLeftByOneStep( localPositionCube);
            }
            return PositionUtils.CubeToAxial(localPositionCube);
        }

        private CubeHexPosition RotateLeftByOneStep(CubeHexPosition cube)
        {
            return new CubeHexPosition(-cube.Y, -cube.Z, -cube.X);
        }

        private CubeHexPosition RotateRightByOneStep(CubeHexPosition cube)
        {
            return new CubeHexPosition(-cube.Z, -cube.X, -cube.Y);
        }

        public MyHexPosition ToGlobalPosition(MyHexPosition position)
        {
            var localPositionCube = PositionUtils.AxialToCube(position);
            for (int i = 0; i < _globalCenterOrientation.ClockwiseIndex(); i++)
            {
                localPositionCube = RotateRightByOneStep( localPositionCube);
            }
            return PositionUtils.CubeToAxial(localPositionCube) + _globalCenterPosition;
        }
    }

}
