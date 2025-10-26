using redactorv;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace redactorv
{
    public class PolygonShape : Shape
    {
        public List<Point> Points { get; private set; }

        public PolygonShape(List<Point> points, Color color) : base(color)
        {
            Points = points;
        }

        public override void Draw(Graphics g)
        {
            using (Brush b = new SolidBrush(Color))
            {
                if (Points.Count >= 3)
                    g.FillPolygon(b, Points.ToArray());
            }
            if (IsSelected && Points.Count >= 3)
            {
                using (Pen p = new Pen(Color.Black, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
                {
                    g.DrawPolygon(p, Points.ToArray());
                }
            }
        }

        public override bool ContainsPoint(Point p)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(Points.ToArray());
                return path.IsVisible(p);
            }
        }

        public override void Move(int dx, int dy)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = new Point(Points[i].X + dx, Points[i].Y + dy);
            }
        }
    }
}