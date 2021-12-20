using System;
using System.Windows.Media.Imaging;

namespace PathFinder
{
    class Grid
    {
        public static Grid Instance;
        public WriteableBitmap surface;
        public Node[,] Nodes { get; set; }
        public int width { get; set; } // width
        public int height { get; set; } // height

        private int countRows { get; set; }
        private int countColumns { get; set; }

        public readonly int spacing = 2;
        public readonly int offset = 10;

        private int nodeSize;

        public Grid(int width, int height, WriteableBitmap surface)
        {
            this.width = width;
            this.height = height;
            this.surface = surface;

            Instance = this;
        }

        public int countNodes
        {
            get
            {
                return countRows * countColumns;
            }
        }

        public void Generate(int rows, int columns)
        {
            countRows = rows;
            countColumns = columns;

            if(Nodes != null)
            {
                surface.Clear();
            }

            Nodes = new Node[columns, rows];

            for (int row = 0; row < countRows; row++)
            {
                for (int column = 0; column < countColumns; column++)
                {
                    Nodes[column, row] = new Node
                    {
                        gridPosition = new Position( column, row ),
                        index = row * countRows + column
                    };
                }
            }

            ValidateLayout();
        }

        // Get node by table position
        public Node GetNode(int column, int row)
        {
            return Nodes[column, row];
        }

        // Get node by its index ( row * maxRows + column ) = index
        public Node GetNode(int index)
        {
            int row = index / countColumns;
            int column = index - row * countColumns;

            return GetNode(column, row);
        }

        // Get node by screen position
        public Node GetNode(Position pos)
        {
            pos.Y = (int)Math.Min(pos.Y, (nodeSize + spacing) * (countRows - 1));
            pos.X = (int)Math.Min(pos.X, (nodeSize + spacing) * (countColumns - 1));

            return GetNode(pos.X / (nodeSize + spacing), pos.Y / (nodeSize + spacing));
        }

        private static readonly int[,] NEIGHBORS = {
                   {0,-1},
            {-1,0},       {+1,0},
                   {0,+1},
        };

        private static readonly int[,] NEIGHBORS_CORNER = { // combination of row-neighbor & column-neighbor from `NEIGHBORS`
            {0,1},        {0,2},

            {3,1},        {3,2},
        };

        public Node[] GetNeighborNodes(Node target)
        {
            int row = target.gridPosition.Y, column = target.gridPosition.X;

            Node[] result = new Node[8];

            for (int i = 0; i < NEIGHBORS.GetLength(0); i++)
            {
                int newColumn = column + NEIGHBORS[i, 0];
                int newRow = row + NEIGHBORS[i, 1];
                if (
                    newColumn >= 0 && newColumn < countColumns
                    &&
                    newRow >= 0 && newRow < countRows
                )
                {
                    result[i] = GetNode(newColumn, newRow);
                }
            }

            // corners
            for (int i = 0; i < NEIGHBORS_CORNER.GetLength(0); i++)
            {
                int parent_IndexOne = NEIGHBORS_CORNER[i, 0];
                int parent_IndexTwo = NEIGHBORS_CORNER[i, 1];

                Node parentOne = result[parent_IndexOne];
                Node parentTwo = result[parent_IndexTwo];

                if(parentOne == null || parentTwo == null) continue;
                if(parentOne.type == NodeType.Block || parentTwo.type == NodeType.Block) continue;

                result[i + NEIGHBORS.GetLength(0)] = GetNode(parentTwo.gridPosition.X, parentOne.gridPosition.Y);
            }

            return result;
        }

        public void ValidateLayout()
        {
            nodeSize = Math.Min(width / countColumns, height / countRows) - spacing;

            for (int row = 0; row < countRows; row++)
            {
                for (int column = 0; column < countColumns; column++)
                {
                    Node target = GetNode(column, row);
                    target.screenPosition = new Position(spacing / 2 + (nodeSize + spacing) * column, spacing / 2 + (nodeSize + spacing) * row);
                    target.size = nodeSize;
                    target.ResetColor(true);
                }
            }
        }

        public void Tick(float msElapsed)
        {
            for (int row = 0; row < countRows; row++)
            {
                for (int column = 0; column < countColumns; column++)
                {
                    Nodes[column, row].Tick(msElapsed);
                }
            }
        }

        public void Draw()
        {
            for (int row = 0; row < countRows; row++)
            {
                for (int column = 0; column < countColumns; column++)
                {
                    Nodes[column, row].ResetColor();
                    Nodes[column, row].Draw();
                }
            }
        }
    }
}
