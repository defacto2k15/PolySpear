using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class MapView : MonoBehaviour
    {
        public GameObject Selector;

        public void MoveSelectorTo(MyHexPosition position)
        {
            Selector.transform.localPosition = position.GetPosition();
        }

        public void MakeSelectorVisible()
        {
            Selector.SetActive(true);
        }

        public void MakeSelectorInisible()
        {
            Selector.SetActive(false);
        }
    }
}
