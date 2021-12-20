using System.Collections.Generic;

namespace PathFinder.Algorithms
{
    class BreadthFirst : BaseAlgorithm
    {
        private Node[] ParentTree;
        private bool[] OpenedNodes;

        private Queue<Node> queue;

        public BreadthFirst(Grid grid) : base(grid) { }

        private void OpenNode(Node target)
        {
            if (target.type != NodeType.StartPoint && target.type != NodeType.EndPoint) target.SetType(NodeType.Opened);

            OpenedNodes[target.index] = true;
        }
        private void CloseNode(Node target)
        {
            if (target.type != NodeType.StartPoint && target.type != NodeType.EndPoint)
            {
                target.SetType(NodeType.Closed);
            }
        }

        override public void Start(Node startNode, Node endNode)
        {
            base.Start(startNode, endNode);

            this.StartNode = startNode;
            this.EndNode = endNode;

            int arraySize = grid.countNodes;

            ParentTree = new Node[arraySize];
            OpenedNodes = new bool[arraySize];

            queue = new Queue<Node>();
            queue.Enqueue(startNode);
        }

        override public List<Node> GetPath()
        {
            List<Node> path = new List<Node>();

            if (ParentTree[EndNode.index] == null)
            {
                return null;
            }

            Node next = ParentTree[EndNode.index];

            while (path.Count == 0 || next != StartNode)
            {
                path.Add(next);

                next = ParentTree[next.index];
            }


            return path;
        }

        public override void Tick()
        {
            if (Finished) return;
            if (queue.Count == 0)
            {
                Finished = true;
                return;
            }
            Node current = queue.Dequeue();

            if (current == EndNode)
            {
                Finished = true;
                return;
            }

            CloseNode(current);

            Node[] neighbours = grid.GetNeighborNodes(current);

            foreach (Node neighbour in neighbours)
            {
                if (neighbour == null) continue;
                if (neighbour.type == NodeType.Block || OpenedNodes[neighbour.index] == true) continue;

                queue.Enqueue(neighbour);
                OpenNode(neighbour);
                ParentTree[neighbour.index] = current;
            }
        }
    }
}
