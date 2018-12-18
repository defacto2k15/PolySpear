using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class TileModel : MonoBehaviour
    {
        [SerializeField] public TileRole Role;
        [SerializeField] public MyHexPosition Position;

        public TileModel Clone()
        {
            return new TileModel()
            {
                Role = Role,
                Position =  Position
            };
        }
    }
}
