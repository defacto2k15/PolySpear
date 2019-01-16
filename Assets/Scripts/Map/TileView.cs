using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class TileView : MonoBehaviour
    {
        public Sprite TileSpriteNormal;
        public Sprite TileSpriteTeam1;
        public Sprite TileSpriteTeam2;
        public Sprite TileSpriteBlocked;

        private TileModel _model;
        private bool _isDisabled = false;

        public void Start()
        {
            _model = GetComponent<TileModel>();
            MyStart();
        }

        public void MyStart()
        {
            var hexPosition = GetComponent<TileModel>().Position;
            transform.localPosition = hexPosition.GetPosition();

            var role = GetComponent<TileModel>().Role;
            if (role == TileRole.Blocked)
            {
                //todo
            }
            else if (role == TileRole.Normal)
            {
                GetComponent<SpriteRenderer>().sprite = TileSpriteNormal;
            }
            else if (role == TileRole.StartTeam1)
            {
                GetComponent<SpriteRenderer>().sprite = TileSpriteTeam1;
            }
            else if (role == TileRole.StartTeam2)
            {
                GetComponent<SpriteRenderer>().sprite = TileSpriteTeam2;
            }

            this.name = "Tile " + hexPosition;

            _model.MagicWindAppliedEvent += () => //todo TL1
            {
                GetComponent<SpriteRenderer>().color = Color.red;
            };

            _model.ResetEvent += () =>
            {
                GetComponent<SpriteRenderer>().color = Color.white; //todo TL1
            };
        }

        public void Update()
        {
            if (_isDisabled != _model.IsDisabled)
            {
                GetComponent<SpriteRenderer>().enabled = !_model.IsDisabled;
                _isDisabled = _model.IsDisabled;
            }
        }
    }
}
