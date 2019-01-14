using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class UnitFlagView : MonoBehaviour
    {
        public void SetFlagColor(Color color)
        {
            GetComponent<SpriteRenderer>().color = color;
        }

        public void Update()
        {
            transform.eulerAngles = new Vector3(90,0,-90);
        }
    }
}
