using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Units
{
    public class EffectReciever
    {
        public bool IsAlive => !WasStruck;
        public bool WasStruck { get; set; } 
        public bool WasPushed { get; set; }
    }
}
