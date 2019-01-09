using UnityEngine;

namespace Assets.Scripts.Units
{
    public class ProjectileModelComponent : PawnModelComponent
    {
        public ProjectileModel Model = new ProjectileModel();
        public override PawnModel PawnModel => Model;
    }
}