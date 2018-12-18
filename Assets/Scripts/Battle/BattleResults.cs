using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Units;

namespace Assets.Scripts.Battle
{
    public class BattleResults
    {
        public List<UnitModel> UnitsKilled = new List<UnitModel>();
        public List<PushResult> UnitsPushed = new List<PushResult>();

        public static BattleResults Empty => new BattleResults();

        public void Add(BattleResults other)
        {
            UnitsKilled.AddRange(other.UnitsKilled);
            UnitsPushed.AddRange(other.UnitsPushed);
        }

        public bool UnitIncapaciated(UnitModel unit)
        {
            return UnitsKilled.Contains(unit) || UnitsPushed.Any(c => c.UnitPushed == unit);
        }

        public List<UnitModel> UnitsIncapaciated
        {
            get
            {
                var outList = new List<UnitModel>();
                outList.AddRange(UnitsKilled);
                outList.AddRange( UnitsPushed.Select(c => c.UnitPushed));
                return outList.Distinct().ToList();
            }
        }
    }
}