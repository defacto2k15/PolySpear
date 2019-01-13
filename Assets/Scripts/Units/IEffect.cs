using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Battle;
using Assets.Scripts.Game;

namespace Assets.Scripts.Units
{
    public interface IEffect
    {
        UnitModel RetriveTarget(BattlefieldVision vision, MyHexPosition activatingPosition);
        bool IsActivated(BattlefieldVision vision, MyHexPosition activatingPosition);
        void Execute(BattlefieldVision vision, MyHexPosition activatingPosition, BattleEngagementResult reciever);
        bool IsDefendableEffect { get; }
        Func<GameCourseModel,PawnModelComponent,PawnModelComponent, IAnimation> UsageAnimationGenerator { get; } //todo doubious elegance
    }
}
