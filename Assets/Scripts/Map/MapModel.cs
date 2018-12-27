using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class MapModel : MonoBehaviour
    {
        public List<TileModel> Tiles { get; set; }

        public bool HasTileAt(MyHexPosition position)
        {
            return Tiles.Any(c => c.Position.Equals(position));
        }

        public TileModel GetTileAt(MyHexPosition position)
        {
            return Tiles.First(c => c.Position.Equals(position));
        }

        public MapModel Clone()
        {
            return new MapModel()
            {
                Tiles = Tiles.Select(c => c.Clone()).ToList()
            };
        }

        public void Reset()
        {
        }
    }
}
