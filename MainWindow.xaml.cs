using PathFinder.Algorithms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
// using UniversalWPF;

namespace PathFinder
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Timer = new Stopwatch();

        }

        int height, width;
        WriteableBitmap writeableBmp;
        public Stopwatch Timer { get; }

        private List<BaseAlgorithm> ActiveAlgorithms;

        public DispatcherTimer fastTimer; // fast tick-frequency when algoritms is ON
        private void ViewPort_Loaded(object sender, RoutedEventArgs e)
        {
            width = (int)ViewPort.Width;
            height = (int)ViewPort.Height;

            writeableBmp = BitmapFactory.New(width, height);

            ViewPort.Source = writeableBmp;


            CompositionTarget.Rendering += TickHandler;

            fastTimer = new DispatcherTimer();
            fastTimer.Interval = new TimeSpan(0, 0, 0, 0, 0);
            fastTimer.Tick += TickHandler;

            Grid grid = new Grid(width, height, writeableBmp);
            grid.Generate(16, 16);

            ActiveAlgorithms = new List<BaseAlgorithm>();

            Timer.Start();
        }

        private void onGridSizeChanged(Object d, EventArgs e) // NumberBoxValueChangedEventArgs
        {
            if(Grid.Instance == null)
            {
                return;
            }
            Grid.Instance.Generate((int)numberBox_Rows.Value, (int)numberBox_Columns.Value);
        }

        private bool down = false;
        private NodeType replaceType;

        private static Node GetHoveredNode(Point pos)
        {
            return Grid.Instance.GetNode(new Position((int)pos.X, (int)pos.Y));
        }

        private void ClearAlgorithmPath()
        {
            foreach (Node target in Grid.Instance.Nodes)
            {
                if( target.type == NodeType.Opened || target.type == NodeType.Closed || target.type == NodeType.Path )
                {
                    target.SetType(NodeType.Empty, true);
                }
            }
        }

        private Type GetSelectedAlgoritmType()
        {
            var val = algoritmsList.SelectionBoxItem.ToString();

            switch (val)
            {
                case "A*":
                    return typeof(AStar);

                case "Breadth First":
                    return typeof(BreadthFirst);

                case "Best First":
                    return typeof(BestFirst);

                default:
                    return null;
            }
        }
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearAlgorithmPath();

            Mouse.Capture(ViewPort);

            if (e.LeftButton is MouseButtonState.Pressed)
            {
                Node hoveredNode = GetHoveredNode(e.GetPosition(ViewPort));
                replaceType = hoveredNode.type;

                down = true;
            }
            else if (e.RightButton is MouseButtonState.Pressed)
            {
                Node hoveredNode = GetHoveredNode(e.GetPosition(ViewPort));
                if (hoveredNode.type == NodeType.StartPoint)
                {
                    hoveredNode.SetType(NodeType.EndPoint);
                }
                else if (hoveredNode.type == NodeType.EndPoint)
                {
                    hoveredNode.SetType(NodeType.Empty);
                }
                else
                {
                    hoveredNode.SetType(NodeType.StartPoint);
                }
            }
            else if (e.MiddleButton is MouseButtonState.Pressed)
            {
                List<Node> startNodes = new List<Node>();
                Node endNode = null;

                foreach (Node node in Grid.Instance.Nodes)
                {
                    if (node.type == NodeType.StartPoint)
                    {
                        startNodes.Add(node);
                    }
                }

                foreach (Node node in Grid.Instance.Nodes)
                {
                    if (node.type == NodeType.EndPoint)
                    {
                        endNode = node;
                        break;
                    }
                }

                if (endNode != null)
                {
                    ActiveAlgorithms = new List<BaseAlgorithm>();
                    startNodes.ForEach((Node start) =>
                    {
                        BaseAlgorithm algorithm = (BaseAlgorithm)Activator.CreateInstance(GetSelectedAlgoritmType(), Grid.Instance);
                        algorithm.Start(start, endNode);
                        ActiveAlgorithms.Add(algorithm);
                    });
                    fastTimer.Start();
                }
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            if (e.LeftButton is MouseButtonState.Released)
            {
                down = false;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!down)
            {
                return;
            }

            Point pos = e.GetPosition(ViewPort).Clamp(width, height);

            Node hoveredNode = Grid.Instance.GetNode(new Position((int)pos.X, (int)pos.Y));

            if (hoveredNode == null || hoveredNode.type != replaceType) return;


            if (replaceType == NodeType.Block)
            {
                hoveredNode.SetType(NodeType.Empty);
            }
            else {
                hoveredNode.SetType(NodeType.Block);
            }
        }

        private void DrawPath( List<Node> path)
        {
            if(path == null || path.Count == 0)
            {
                return;
            }

            //Position lastPos = path[0].pos;
            //int center = path[0].size / 2;

            for (int i = 0; i < path.Count; i++)
            {
                path[i].SetType(NodeType.Path);
                //Position currentPos = path[i].pos;
                //Grid.Instance.surface.DrawLine(lastPos.X + center, lastPos.Y + center, currentPos.X + center, currentPos.Y + center, Colors.Red);
                //lastPos = currentPos;
            }
        }


        private TimeSpan preiousTick;
        public float ElapsedMilisecondsSinceLastTick
        {
            get
            {
                return (float)(Timer.Elapsed - preiousTick).TotalMilliseconds;
            }
        }

        private void TickHandler(object sender, EventArgs e)
        {
            Grid.Instance.Tick(ElapsedMilisecondsSinceLastTick);
            preiousTick = Timer.Elapsed;

            int count = ActiveAlgorithms.Count;
            for (int i = 0; i < count; i++)
            {
                BaseAlgorithm a = ActiveAlgorithms[i];
                if( a.Finished )
                {
                    DrawPath(a.GetPath());
                    ActiveAlgorithms.Remove(a);
                    count--;
                    continue;
                }
                a.Tick();
            }
            if( count == 0)
            {
                fastTimer.Stop();
            }
        }
    }
}
