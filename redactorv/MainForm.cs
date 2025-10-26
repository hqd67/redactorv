using redactorv;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace redactorv
{
    public class MainForm : Form
    {
        private Panel canvas;
        private FlowLayoutPanel toolbar;
        private Button btnSelect;
        private Button btnRect;
        private Button btnEllipse;
        private Button btnPolygon;
        private Button btnColor;
        private Button btnDelete;
        private Button btnSave;
        private Button btnLoad;
        private Label lblTool;

        private List<Shape> shapes = new List<Shape>();
        private Shape selectedShape = null;
        private Shape tempShape = null;
        private List<Point> polygonPoints = new List<Point>();

        private bool isDrawing = false;
        private Point startPoint;
        private Color currentColor = Color.LightBlue;

        private enum Tool { Select, Rectangle, Ellipse, Polygon }
        private Tool currentTool = Tool.Select;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "Vector Editor (WinForms, .NET Framework 4.7.2)";
            this.Width = 1000;
            this.Height = 700;
            this.DoubleBuffered = true;
        }

        private void InitializeComponent()
        {
            toolbar = new FlowLayoutPanel();
            toolbar.Dock = DockStyle.Top;
            toolbar.AutoSize = true;
            toolbar.Padding = new Padding(6);

            btnSelect = new Button() { Text = "Select" };
            btnRect = new Button() { Text = "Rectangle" };
            btnEllipse = new Button() { Text = "Ellipse" };
            btnPolygon = new Button() { Text = "Polygon" };
            btnColor = new Button() { Text = "Color" };
            btnDelete = new Button() { Text = "Delete" };
            btnSave = new Button() { Text = "Save" };
            btnLoad = new Button() { Text = "Load" };
            lblTool = new Label() { Text = "Tool: Select", AutoSize = true, Padding = new Padding(10, 8, 0, 0) };

            btnSelect.Click += (s, e) => SetTool(Tool.Select);
            btnRect.Click += (s, e) => SetTool(Tool.Rectangle);
            btnEllipse.Click += (s, e) => SetTool(Tool.Ellipse);
            btnPolygon.Click += (s, e) => SetTool(Tool.Polygon);
            btnColor.Click += BtnColor_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSave.Click += BtnSave_Click;
            btnLoad.Click += BtnLoad_Click;

            toolbar.Controls.AddRange(new Control[] {
                btnSelect, btnRect, btnEllipse, btnPolygon, btnColor, btnDelete, btnSave, btnLoad, lblTool
            });

            this.Controls.Add(toolbar);

            canvas = new Panel();
            canvas.Dock = DockStyle.Fill;
            canvas.BackColor = Color.White;
            canvas.Paint += Canvas_Paint;
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            canvas.DoubleClick += Canvas_DoubleClick;

            this.Controls.Add(canvas);

            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
        }

        private void SetTool(Tool tool)
        {
            currentTool = tool;
            lblTool.Text = "Tool: " + tool.ToString();
            polygonPoints.Clear();
            tempShape = null;
            isDrawing = false;
            selectedShape = null;
            canvas.Invalidate();
        }

        private void BtnColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            try
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    currentColor = cd.Color;
                }
            }
            finally
            {
                cd.Dispose();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedShape != null)
            {
                shapes.Remove(selectedShape);
                selectedShape = null;
                canvas.Invalidate();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JSON files (*.json)|*.json";
            try
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveLoad.SaveToFile(shapes, sfd.FileName);
                }
            }
            finally
            {
                sfd.Dispose();
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON files (*.json)|*.json";
            try
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    List<Shape> loaded = SaveLoad.LoadFromFile(ofd.FileName);
                    shapes = (loaded != null) ? loaded : new List<Shape>();
                    selectedShape = null;
                    polygonPoints.Clear();
                    canvas.Invalidate();
                }
            }
            finally
            {
                ofd.Dispose();
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && selectedShape != null)
            {
                shapes.Remove(selectedShape);
                selectedShape = null;
                canvas.Invalidate();
            }
        }

        private void Canvas_DoubleClick(object sender, EventArgs e)
        {
            if (currentTool == Tool.Polygon && polygonPoints.Count >= 3)
            {
                PolygonShape poly = new PolygonShape(new List<Point>(polygonPoints), currentColor);
                shapes.Add(poly);
                polygonPoints.Clear();
                canvas.Invalidate();
            }
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
            if (currentTool == Tool.Select)
            {
                selectedShape = null;
                for (int i = shapes.Count - 1; i >= 0; i--)
                {
                    if (shapes[i].ContainsPoint(e.Location))
                    {
                        selectedShape = shapes[i];
                        break;
                    }
                }
                foreach (Shape s in shapes) s.IsSelected = false;
                if (selectedShape != null) selectedShape.IsSelected = true;
                canvas.Invalidate();
                if (selectedShape != null && e.Button == MouseButtons.Left)
                {
                    isDrawing = true; 
                }
            }
            else if (currentTool == Tool.Polygon)
            {
                if (e.Button == MouseButtons.Left)
                {
                    polygonPoints.Add(e.Location);
                    canvas.Invalidate();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (polygonPoints.Count >= 3)
                    {
                        PolygonShape poly = new PolygonShape(new List<Point>(polygonPoints), currentColor);
                        shapes.Add(poly);
                        polygonPoints.Clear();
                        canvas.Invalidate();
                    }
                }
            }
            else 
            {
                isDrawing = true;
                Rectangle r = new Rectangle(startPoint, new Size(0, 0));
                if (currentTool == Tool.Rectangle)
                {
                    tempShape = new RectangleShape(r, currentColor);
                }
                else
                {
                    tempShape = new EllipseShape(r, currentColor);
                }
                canvas.Invalidate();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            if (currentTool == Tool.Select)
            {
                if (selectedShape != null && e.Button == MouseButtons.Left)
                {
                    int dx = e.X - startPoint.X;
                    int dy = e.Y - startPoint.Y;
                    selectedShape.Move(dx, dy);
                    startPoint = e.Location;
                    canvas.Invalidate();
                }
            }
            else 
            {
                Rectangle r = MakeRect(startPoint, e.Location);

                if (tempShape is RectangleShape)
                {
                    tempShape = new RectangleShape(r, currentColor);
                }
                else if (tempShape is EllipseShape)
                {
                    tempShape = new EllipseShape(r, currentColor);
                }
                canvas.Invalidate();
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            if (currentTool == Tool.Select)
            {
                isDrawing = false;
            }
            else if (tempShape != null)
            {
                shapes.Add(tempShape);
                tempShape = null;
                isDrawing = false;
                canvas.Invalidate();
            }
        }

        private Rectangle MakeRect(Point a, Point b)
        {
            return new Rectangle(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y),
                Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (Shape s in shapes)
            {
                s.Draw(e.Graphics);
            }

            if (tempShape != null)
            {
                tempShape.Draw(e.Graphics);
            }

            if (polygonPoints.Count > 0)
            {
                using (Pen p = new Pen(currentColor, 2))
                {
                    for (int i = 0; i < polygonPoints.Count - 1; i++)
                        e.Graphics.DrawLine(p, polygonPoints[i], polygonPoints[i + 1]);
                }
                foreach (Point pt in polygonPoints)
                {
                    e.Graphics.FillEllipse(Brushes.Black, pt.X - 3, pt.Y - 3, 6, 6);
                }
            }
        }
    }
}
