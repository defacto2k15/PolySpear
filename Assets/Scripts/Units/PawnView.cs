using UnityEngine;

namespace Assets.Scripts.Units
{
    public class PawnView : MonoBehaviour
    {
        private PawnModel _model;

        public void Start()
        {
            _model = GetComponent<PawnModel>();
            _model.OnUnitKilled += () =>
            {
                GameObject.Destroy(transform.gameObject);
            };
            MyStart();
        }

        protected virtual void MyStart()
        {
        }

        public void Update()
        {
            UpdateView(); 
            MyUpdate();
        }

        protected virtual void MyUpdate()
        {
        }

        private void UpdateView()
        {
            if (_model == null) // todo more elegant solution
            {
                _model = GetComponent<PawnModel>();
            }
            transform.localPosition = _model.Position.GetPosition();
            transform.localEulerAngles = new Vector3(90,_model.Orientation.FlatRotation(),0);
        }
    }
}