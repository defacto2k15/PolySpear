using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class StringInList : PropertyAttribute {

        public List<Orientation> List => Orientation.AllOrientationsClockwise;
    }
}