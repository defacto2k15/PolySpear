using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace Assets.Scripts.Units
{
    public class BattleResults
    {
        private List<UnitModel> _struckUnits = new List<UnitModel>();
        private List<UnitDisplacements> _displacements = new List<UnitDisplacements>();

        public List<UnitModel> StruckUnits => _struckUnits;
        public List<UnitDisplacements> Displacements => _displacements;

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
        }

        public bool PositionWasFreed(MyHexPosition target)
        {
            return _struckUnits.Any(c => c.Position.Equals(target)) || _displacements.Any(c => c.DisplacementStart.Equals(target));
        }
    }

    public class UnitDisplacements
    {
        public UnitModel Unit;
        public MyHexPosition DisplacementStart;
        public MyHexPosition DisplacementEnd;
    }
}
