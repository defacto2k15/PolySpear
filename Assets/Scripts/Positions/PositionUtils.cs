using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Positions
{
    public static class PositionUtils
    {
        public static MyHexPosition CubeToAxial(CubeHexPosition cube)
        {
            var realAxial = new MyHexPosition(cube.X, cube.Z);

            return new MyHexPosition(-realAxial.U - (realAxial.V), -realAxial.V);
        }

        public static CubeHexPosition AxialToCube(MyHexPosition axial)
        {
            var realAxial = new MyHexPosition(-axial.U + (axial.V), -axial.V );
            return new CubeHexPosition(realAxial.U, -realAxial.U - realAxial.V, realAxial.V);
        }
    }
}
