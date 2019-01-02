using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Battle;

namespace Assets.Scripts.Units
{
    public interface IEffect
    {
        UnitModel RetriveTarget(BattlefieldVision vision, MyHexPosition activatingPosition);
        bool IsActivated(BattlefieldVision vision, MyHexPosition activatingPosition);
        void Execute(BattlefieldVision vision, MyHexPosition activatingPosition, BattleResults reciever);
        bool IsDefendableEffect { get; }
    }
}
