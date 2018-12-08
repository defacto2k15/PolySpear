using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Units
{
    public class EffectReciever
    {
        private bool _isAlive = true;
        public bool WasKilled => !_isAlive;

        public void Kill()
        {
            _isAlive = false;
        }
    }
}
