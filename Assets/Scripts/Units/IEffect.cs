using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Units
{
    public interface IEffect
    {
        void Execute(EffectReciever effectReciever);
    }
}
