using redactorv;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace redactorv
{
    public enum Tool
    {
        Select,
        Rectangle,
        Ellipse,
        Polygon
    }

    public class MainForm : Form
    {
        private Panel canvas;
        private FlowLayoutPanel toolbar;
        private Button btnSelect, btnRect, btnEllipse, btnPolygon, btnColor;
        private Label lblTool;

        private Tool currentTool = Tool.Select;
        private List<Shape> shapes = new List<Shape>();
        private Shape tempShape = null;
        private List<Point> polygonPoints = new List<Point>();

        private bool isDrawing = false;
        private Point startPoint;
        private Color currentColor = Color.LightBlue;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Vector Editor (Lab 1)";
            this.Width = 1200;
            this.Height = 800;

            toolbar = new FlowLayoutPanel();
            toolbar.Dock = DockStyle.Top;
            toolbar.AutoSize = true;
            toolbar.Padding = new Padding(6);

            btnSelect = new Button() { Text = "Select" };
            btnRect = new Button() { Text = "Rectangle" };
            btnEllipse = new Button() { Text = "Ellipse" };
            btnPolygon = new Button() { Text = "Polygon" };
            btnColor = new Button() { Text = "Color" };

            lblTool = new Label() { Text = "Tool: Select", AutoSize = true, Padding = new Padding(10, 8, 0, 0) };

            toolbar.Controls.AddRange(new Control[] {
                btnSelect, btnRect, btnEllipse, btnPolygon, btnColor, lblTool
            });
            this.Controls.Add(toolbar);

            canvas = new Panel();
            canvas.Dock = DockStyle.Fill;
            canvas.BackColor = Color.White;
            this.Controls.Add(canvas);

            canvas.Paint += Canvas_Paint;
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            canvas.DoubleClick += Canvas_DoubleClick;

            btnSelect.Click += (s, e) => SetTool(Tool.Select);
            btnRect.Click += (s, e) => SetTool(Tool.Rectangle);
            btnEllipse.Click += (s, e) => SetTool(Tool.Ellipse);
            btnPolygon.Click += (s, e) => SetTool(Tool.Polygon);
            btnColor.Click += PickColor;
        }

        private void SetTool(Tool t)
        {
            currentTool = t;
            lblTool.Text = "Tool: " + t;
            tempShape = null;
            polygonPoints.Clear();
            canvas.Invalidate();
        }

        private void PickColor(object sender, EventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    currentColor = dlg.Color;
                }
            }
        }

        private Rectangle MakeRect(Point a, Point b)
        {
            return new Rectangle(Math.Min(a.X, b.X),
                                 Math.Min(a.Y, b.Y),
                                 Math.Abs(b.X - a.X),
                                 Math.Abs(b.Y - a.Y));
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentTool == Tool.Select)
                return;

            isDrawing = true;
            startPoint = e.Location;

            if (currentTool == Tool.Polygon)
            {
                polygonPoints.Add(e.Location);
                canvas.Invalidate();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            if (currentTool == Tool.Rectangle)
            {
                tempShape = new RectangleShape(MakeRect(startPoint, e.Location), currentColor);
            }
            if (currentTool == Tool.Ellipse)
            {
                tempShape = new EllipseShape(MakeRect(startPoint, e.Location), currentColor);
            }

            canvas.Invalidate();
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;
            isDrawing = false;

            if (tempShape != null)
            {
                shapes.Add(tempShape);
                tempShape = null;
            }

            canvas.Invalidate();
        }

        private void Canvas_DoubleClick(object sender, EventArgs e)
        {
            if (currentTool == Tool.Polygon && polygonPoints.Count >= 3)
            {
                shapes.Add(new PolygonShape(new List<Point>(polygonPoints), currentColor));
                polygonPoints.Clear();
                canvas.Invalidate();
            }
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (Shape shape in shapes)
                shape.Draw(e.Graphics);

            tempShape?.Draw(e.Graphics);

            if (currentTool == Tool.Polygon && polygonPoints.Count > 0)
            {
                using (Pen p = new Pen(currentColor, 2))
                {
                    for (int i = 0; i < polygonPoints.Count - 1; i++)
                        e.Graphics.DrawLine(p, polygonPoints[i], polygonPoints[i + 1]);
                }

                foreach (Point pt in polygonPoints)
                    e.Graphics.FillEllipse(Brushes.Black, pt.X - 3, pt.Y - 3, 6, 6);
            }
        }
    }
}