using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Battle;
using UnityEngine;

namespace Assets.Scripts
{
    public class DebugTransformator : MonoBehaviour
    {
        public void Start2()
        {
            Test(new MyHexPosition(1, 1), Orientation.N, new MyHexPosition(2, 1));
            Test(new MyHexPosition(1, 1), Orientation.N, new MyHexPosition(2, 2));
            Test(new MyHexPosition(1, 1), Orientation.NE, new MyHexPosition(2, 1));
            Test(new MyHexPosition(1, 1), Orientation.S, new MyHexPosition(2, 2));
            Test(new MyHexPosition(1, 1), Orientation.NW, new MyHexPosition(2, 2));
            Test(new MyHexPosition(1, 1), Orientation.N, new MyHexPosition(4, 1));
            Test(new MyHexPosition(1,1),Orientation.NW, new MyHexPosition(4,4) );
        }

        private void Test(MyHexPosition position, Orientation orientation, MyHexPosition testPoint)
        {
            testPoint = testPoint - position;
            position = new MyHexPosition(0,0);
            var transformator = new BattlefieldPointOfViewTransformator(position, orientation);

            var local = transformator.ToLocalPosition(testPoint);
            var global = transformator.ToGlobalPosition(local);

            Debug.Log($"Center {position} orientation {orientation} target {testPoint} || local {local} || global {global}");
        }
    }
}
