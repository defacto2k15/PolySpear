using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class MapModel : MonoBehaviour
    {
        public Dictionary<MyHexPosition, GameObject> Tiles { set { _tiles = value; } }
        private Dictionary<MyHexPosition, GameObject> _tiles;

        public void Start()
        {
            if (_tiles == null)
            {
                _tiles = new Dictionary<MyHexPosition, GameObject>();
                foreach (var child in transform.Cast<Transform>().Select(c => c.gameObject).Select(c => c.GetComponent<TileModel>()).Where(c => c != null))
                {
                    _tiles[child.Position] = child.gameObject;
                }
            }
        }

        public bool HasTileAt(MyHexPosition position)
        {
            return _tiles.ContainsKey(position);
        }

        public GameObject GetTileAt(MyHexPosition position)
        {
            return _tiles[position];
        }
    }
}
