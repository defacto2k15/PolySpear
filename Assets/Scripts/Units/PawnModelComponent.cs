using UnityEngine;

namespace Assets.Scripts.Units
{
    public abstract class PawnModelComponent : MonoBehaviour
    {
        public abstract PawnModel PawnModel { get; }
    }
}