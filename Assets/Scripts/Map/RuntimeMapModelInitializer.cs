using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class RuntimeMapModelInitializer : MonoBehaviour
    {
        public void Start()
        {
            var model = GetComponent<MapModel>();
            if (model.Tiles == null)
            {
                var tiles = new List<TileModel>();
                foreach (var child in transform.Cast<Transform>().Select(c => c.gameObject).Select(c => c.GetComponent<TileModel>()).Where(c => c != null))
                {
                    tiles.Add(child.GetComponent<TileModel>());
                }
                model.Tiles = tiles;
            }
        }
    }
}