using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Symbols;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class UnitsContainer : PawnsContainer<UnitModel>
    {
        public UnitModel AddUnit(MyHexPosition position, MyPlayer player, Orientation orientation)
        {
            var unit = new UnitModel
            {
                Orientation = orientation,
                Position = position,
                Owner = player
            };

            base.AddPawn(position, unit);
            return unit;
        }

        public bool HasAnyUnits(MyPlayer player)
        {
            return _pawns.Values.Any(c => c.Owner == player);
        }

        public List<UnitModel> GetUnitsOfPlayer(MyPlayer player)
        {
            return _pawns.Values.Where(c => c.Owner == player).ToList();
        }

        public UnitsContainer Clone()
        {
            return new UnitsContainer()
            {
                _pawns = _pawns.ToDictionary(c => c.Key, c => c.Value.Clone())
            };
        }
    }
}