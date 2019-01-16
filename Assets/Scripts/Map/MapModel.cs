using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Magic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Map
{
    public class MapModel : MonoBehaviour
    {
        public List<TileModel> Tiles { get; set; }
        private Dictionary<MyHexPosition, MagicType> _residentMagicMap = new Dictionary<MyHexPosition, MagicType>();

        public bool HasTileAt(MyHexPosition position)
        {
            return Tiles.Where(c => c.Position.Equals(position)).Any(c => !c.IsDisabled);
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
            _residentMagicMap = new Dictionary<MyHexPosition, MagicType>();
            Tiles.ForEach(c => c.MyReset());
        }

        public void DisableAt(MyHexPosition position)
        {
            GetTileAt(position).IsDisabled = true;
        }

        public void AddResidentMagic(MyHexPosition position, MagicType magic)
        {
            Assert.IsFalse(_residentMagicMap.ContainsKey(position),$"There is arleady magic at ${position}");
            Assert.IsTrue(HasTileAt(position));

            _residentMagicMap[position] = magic;
            GetTileAt(position).ApplyWindMagic();  //todo TL1
        }

        public bool IsRepeatField(MyHexPosition target) //todo technical loan TL1
        {
            return _residentMagicMap.ContainsKey(target) && _residentMagicMap[target] == MagicType.Wind;
        }
    }
}
