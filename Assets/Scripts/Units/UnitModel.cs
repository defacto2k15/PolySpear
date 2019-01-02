using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Game;
using Assets.Scripts.Symbols;

namespace Assets.Scripts.Units
{
    public class UnitModel : PawnModel
    {
        public MyPlayer Owner { get; set; }
        public Dictionary<Orientation, SymbolModel> Symbols { get; set; }

        public UnitModel Clone()
        {
            return new UnitModel()
            {
                Owner = Owner,
                Position = Position,
                Orientation = Orientation,
                Symbols = Symbols.ToDictionary(c => c.Key, c => c.Value),
                IsUnitAlive = IsUnitAlive
            };
        }
    }
}
