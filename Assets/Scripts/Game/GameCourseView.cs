using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class GameCourseView : MonoBehaviour
    {
        public GameObject MarkerPrefab;
        private GameObject _selector;
        private GameObject _selectedUnitMarker;
        private List<GameObject> _moveTargetMarkers;

        public void Start()
        {
            _selector = CreateMarker("Selector", Constants.SelectorColor, Vector3.zero, "TileSelector");
        }

        public void Reset()
        {
            GameObject.Destroy(_selector);
            _selector = null;
            if (_selectedUnitMarker != null)
            {
                GameObject.Destroy(_selectedUnitMarker);
                _selectedUnitMarker = null;
            }
            if (_moveTargetMarkers != null)
            {
                foreach (var marker in _moveTargetMarkers)
                {
                    GameObject.Destroy(marker);
                }
                _moveTargetMarkers = null;
            }
        }

        public void MoveSelectorTo(MyHexPosition position)
        {
            _selector.transform.localPosition = position.GetPosition();
        }

        public void MakeSelectorVisible()
        {
            _selector.SetActive(true);
        }

        public void MakeSelectorInvisible()
        {
            _selector.SetActive(false);
        }

        private GameObject CreateMarker(string markerName, Color color, Vector3 position, string sortingLayerName)
        {
            var marker = Instantiate(MarkerPrefab);
            marker.name = markerName;
            marker.transform.SetParent(transform);
            marker.GetComponent<SpriteRenderer>().color = color;
            marker.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
            marker.transform.localPosition = position;
            return marker;
        }

        public void SetSelectedMarker(MyHexPosition position)
        {
            RemoveSelectedMarker();
            _selectedUnitMarker = CreateMarker("selectedUnitMarker", Constants.SelectedUnitMarkerColor, position.GetPosition(), "TileMarker");
        }

        public void RemoveSelectedMarker()
        {
            if (_selectedUnitMarker != null)
            {
                GameObject.Destroy(_selectedUnitMarker);
                _selectedUnitMarker = null;
            }
        }

        public void SetMoveTargets(List<MyHexPosition> moveTargets )
        {
            RemoveMoveTargets();
            _moveTargetMarkers = moveTargets.Select(c => CreateMarker("MoveTargetMarker" + c, Constants.MoveTargetMarker, c.GetPosition(), "TileMarker")).ToList();
        }

        public void RemoveMoveTargets()
        {
            if (_moveTargetMarkers == null)
            {
                return;
            }
            foreach (var target in _moveTargetMarkers)
            {
                GameObject.Destroy(target);
            }
            _moveTargetMarkers = null;
        }
    }
}
