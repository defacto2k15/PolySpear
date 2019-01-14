using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game;
using UnityEngine.Assertions;

namespace Assets.Scripts.Magic
{
    public class MagicUsage
    {
        private MagicType _type;
        private MyHexPosition _position;
        private readonly MyPlayer _player;
        private GameCourseModel _model;

        private IAnimation _animation;
        private bool _magicUsageEnded;

        public MagicUsage(MagicType type, MyHexPosition position, GameCourseModel model, MyPlayer player, CameraShake cameraShake)
        {
            _type = type;
            _position = position;
            _player = player;
            _model = model;
            _magicUsageEnded = false;

            if (type == MagicType.Earth)
            {
                _animation = new EarthMagicUsageAnimation(model.GetTileAt(position).gameObject, cameraShake);
                _animation.StartAnimation();
            }
        }

        public bool MagicUsageEnded => _magicUsageEnded;
        public void Update()
        {
            Assert.IsFalse(_magicUsageEnded, "magic is still in usage");
            if (_animation.WeAreDuringAnimation())
            {
                _animation.UpdateAnimation();
            }
            else
            {
                _model.UseMagic(_type, _position, _player);
                _magicUsageEnded = true;
            }
        }
    }
}
