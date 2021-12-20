using System;
using System.Collections.Generic;

namespace PathFinder.Algorithms
{
    class AStar : BaseAlgorithm
    {
        private float[] GCost; // Distance from starting node
        private float[] HCost; // Distance from end node
        private float[] FCost; // G cost + H cost

        private Node[] ParentTree;

        private bool[] OpenedNodes;
        private bool[] ClosedNodes;

        private bool diagonalMovement = true; // todo;

        public AStar(Grid grid) : base(grid) { }

        private float SQRT2 = MathF.Sqrt(2);

        protected virtual float GetHeuristic( int dx, int dy )
        {
            if (diagonalMovement)
            {
                return Heuristic.Manhattan(dx, dy);
            }
            else
            {
                return Heuristic.Octile(dx, dy);
            }
        }

        private void OpenNode( Node target )
        {
            if(target.type != NodeType.StartPoint && target.type != NodeType.EndPoint) target.SetType(NodeType.Opened);

            // OpenedNodes.Add(target);
            OpenedNodes[target.index] = true;
        }
        private void CloseNode( Node target )
        {
            if (target.type != NodeType.StartPoint && target.type != NodeType.EndPoint) target.SetType(NodeType.Closed);

            // OpenedNodes.Remove(target);
            // ClosedNodes.Add(target);
            OpenedNodes[target.index] = false;
            ClosedNodes[target.index] = true;
        }

        private Node GetLowestFNode()
        {
            Node nodeWithLowestFCost = null;
            float lowestFCost = 0f;

            for (int i = 0; i < OpenedNodes.Length; i++)
            {
                if (!OpenedNodes[i]) continue;

                Node node = Grid.Instance.GetNode(i);
                float fCost = FCost[node.index];
                if (lowestFCost == 0 || (fCost != 0 && fCost < lowestFCost))
                {
                    nodeWithLowestFCost = node;
                    lowestFCost = fCost;
                }
            }

            return nodeWithLowestFCost;
        }

        override public void Start(Node startNode, Node endNode)
        {
            base.Start( startNode, endNode );

            this.StartNode = startNode;
            this.EndNode = endNode;

            int arraySize = grid.countNodes;

            GCost = new float[arraySize];
            HCost = new float[arraySize];
            FCost = new float[arraySize];
            ParentTree = new Node[arraySize];

            // OpenedNodes = new List<Node>();
            // ClosedNodes = new List<Node>();
            OpenedNodes = new bool[arraySize];
            ClosedNodes = new bool[arraySize];


            OpenNode(startNode);
        }

        override public List<Node> GetPath()
        {
            List<Node> path = new List<Node>();

            if( ParentTree[ EndNode.index ] == null)
            {
                return null;
            }

            Node next = ParentTree[EndNode.index];

            while (path.Count == 0 || next != StartNode)
            {
                path.Add( next );

                next = ParentTree[next.index];
            }


            return path;
        }

        public override void Tick()
        {
            if( Finished) return;

            Node current = GetLowestFNode();
            if( current == null || current == EndNode)
            {
                Finished = true;
                return;
            }

            CloseNode(current);

            Node[] neighbours = grid.GetNeighborNodes( current );

            float currentG = GCost[current.index];

            foreach (Node neighbour in neighbours)
            {
                if (neighbour == null) continue;
                if (neighbour.type == NodeType.Block || ClosedNodes[neighbour.index]) continue;

                float gCost = GCost[neighbour.index];
                bool isOpened = OpenedNodes[neighbour.index];

                float newGCost = currentG + ((neighbour.gridPosition.X == current.gridPosition.X || neighbour.gridPosition.Y == current.gridPosition.Y) ? 1 : SQRT2); //neighbour.GetDistanceSqr(current);
                if (gCost == 0 || gCost > newGCost || !isOpened)
                {
                    if (HCost[neighbour.index] == 0)
                    {
                        HCost[neighbour.index] = GetHeuristic(Math.Abs(neighbour.gridPosition.X - EndNode.gridPosition.X), Math.Abs(neighbour.gridPosition.Y - EndNode.gridPosition.Y));
                    }
                    GCost[neighbour.index] = newGCost;
                    FCost[neighbour.index] = newGCost + HCost[neighbour.index];

                    ParentTree[neighbour.index] = current;

                    if (!isOpened) OpenNode(neighbour);
                }

            }
        }
    }
}
