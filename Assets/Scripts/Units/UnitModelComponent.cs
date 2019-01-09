namespace Assets.Scripts.Units
{
    public class UnitModelComponent : PawnModelComponent
    {
        public UnitModel Model = new UnitModel();
        public override PawnModel PawnModel => Model;
    }
}