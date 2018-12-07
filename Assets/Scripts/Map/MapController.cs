using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class MapController : MonoBehaviour
    {
        private MapModel _model;
        private MapView _view;

        public void Start()
        {
            _model = GetComponent<MapModel>();
            _view = GetComponent<MapView>();
        }

        public void Update()
        {
            var hexPosition = getMouseHex();
            if (hexPosition != null)
            {
                if (_model.HasTileAt(hexPosition))
                {
                    _view.MakeSelectorVisible();
                    _view.MoveSelectorTo(hexPosition);
                }
                else
                {
                    _view.MakeSelectorInisible();
                }
                Debug.Log("Paiting at: " + hexPosition);
            }
            else
            {
                _view.MakeSelectorInisible();
            }
            var ay = 2;
        }

        private MyHexPosition getMouseHex()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue);
            if (hits.Length == 0)
            {
                return null;
            }
            else
            {
                float minDist = float.PositiveInfinity;
                int min = 0;
                for (int i = 0; i < hits.Length; ++i)
                {
                    if (hits[i].distance < minDist)
                    {
                        minDist = hits[i].distance;
                        min = i;
                    }
                }
                return MyHexPosition.FromWorldPosition(hits[min].point);
            }
        }
    }
}
