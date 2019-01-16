using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class UnitSpriteView : MonoBehaviour
    {
        public void Update()
        {
            transform.eulerAngles = new Vector3(90,0,180);
        }
    }
}
