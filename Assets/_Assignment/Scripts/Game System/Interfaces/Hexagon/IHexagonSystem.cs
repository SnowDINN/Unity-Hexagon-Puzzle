namespace Anonymous.Game.Hexagon
{
    public interface IHexagonSystem
    {
        void Setup(IHexagon hexagon);
        void Teardown();
    }
}