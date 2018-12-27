using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    public class LateStart
    {
        private bool _executed = false;

        public bool ShouldRunStart
        {
            get
            {
                if (_executed == false)
                {
                    _executed = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
