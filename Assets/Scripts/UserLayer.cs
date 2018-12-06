using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class UserLayer
    {
        public static UserLayer TileCollisionLayer = new UserLayer(8);

        private readonly int _layerIndex;

        private UserLayer(int layerIndex)
        {
            _layerIndex = layerIndex;
        }

        public int LayerMask => 1 << _layerIndex;

        public int LayerIndex => _layerIndex;
    }
}
