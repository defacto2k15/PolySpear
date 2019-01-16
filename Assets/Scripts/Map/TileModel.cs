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
        [SerializeField] public bool IsDisabled = false;

        public event Action MagicWindAppliedEvent;
        public event Action ResetEvent;

        public void ApplyWindMagic()
        {
            MagicWindAppliedEvent?.Invoke();
        }

        public void MyReset()
        {
            IsDisabled = false;
            ResetEvent?.Invoke();
        }

        public TileModel Clone()
        {
            return new TileModel()
            {
                Role = Role,
                Position =  Position,
                IsDisabled = IsDisabled
            };
        }
    }
}
