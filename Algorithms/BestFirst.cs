
namespace PathFinder.Algorithms
{
    class BestFirst : AStar
    {
        public BestFirst(Grid grid) : base(grid) { }

        protected override float GetHeuristic(int dx, int dy)
        {
            return base.GetHeuristic(dx, dy) * 1000000;
        }
    }
}
