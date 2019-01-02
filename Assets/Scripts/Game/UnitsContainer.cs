using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Symbols;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class UnitsContainer : PawnsContainer<UnitModel>
    {
        public UnitModel AddUnit(MyHexPosition position, MyPlayer player, Orientation orientation, GameObject unitPrefab)
        {
            var unit = Instantiate(unitPrefab, transform);
            unit.GetComponent<UnitModel>().Orientation = orientation;
            unit.GetComponent<UnitModel>().Position = position;
            unit.GetComponent<UnitModel>().Owner = player;

            var model = unit.GetComponent<UnitModel>();
            base.AddPawn(position, model);
            return model;
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