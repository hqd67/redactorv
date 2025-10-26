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
        private Button btnSelect, btnRect, btnEllipse, btnPolygon, btnColor, btnDelete, btnSave, btnLoad;
        private Label lblTool;

        private List<Shape> shapes = new List<Shape>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(6) };
            btnSelect = new Button() { Text = "Select" };
            btnRect = new Button() { Text = "Rectangle" };
            btnEllipse = new Button() { Text = "Ellipse" };
            btnPolygon = new Button() { Text = "Polygon" };
            btnColor = new Button() { Text = "Color" };
            btnDelete = new Button() { Text = "Delete" };
            btnSave = new Button() { Text = "Save" };
            btnLoad = new Button() { Text = "Load" };
            lblTool = new Label() { Text = "Tool: Select", AutoSize = true, Padding = new Padding(10, 8, 0, 0) };

            toolbar.Controls.AddRange(new Control[] { btnSelect, btnRect, btnEllipse, btnPolygon, btnColor, btnDelete, btnSave, btnLoad, lblTool });
            this.Controls.Add(toolbar);

            canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            this.Controls.Add(canvas);
        }
    }
}