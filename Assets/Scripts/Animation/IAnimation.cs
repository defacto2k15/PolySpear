namespace Assets.Scripts.Game
{
    public interface IAnimation
    {
        void Update();
        bool Finished { get; }
    }
}