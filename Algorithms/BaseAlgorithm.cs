using System.Collections.Generic;

namespace PathFinder.Algorithms
{
    abstract class BaseAlgorithm
    {
        protected Grid grid;
        public bool Finished { get; set; }

        protected Node EndNode;
        protected Node StartNode;

        public BaseAlgorithm( Grid grid )
        {
            this.grid = grid;
        }

        virtual public void Start(Node startNode, Node endNode)
        {
            Finished = false;

            this.StartNode = startNode;
            this.EndNode = endNode;

            // start
        }

        virtual public List<Node> GetPath()
        {
            return null;
        }

        virtual public void Tick()
        {
            // Do next iteration here
            return;
        }
    }
}
