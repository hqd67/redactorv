using redactorv;
using System.Drawing;

namespace redactorv
{
    public class EllipseShape : Shape
    {
        public Rectangle Rect { get; private set; }

        public EllipseShape(Rectangle rect, Color color) : base(color)
        {
            Rect = rect;
        }

        public override void Draw(Graphics g)
        {
            using (Brush b = new SolidBrush(Color))
            {
                g.FillEllipse(b, Rect);
            }
            if (IsSelected)
            {
                using (Pen p = new Pen(Color.Black, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
                {
                    g.DrawEllipse(p, Rect);
                }
            }
        }

        public override bool ContainsPoint(Point p)
        {
            float a = Rect.Width / 2f;
            float b = Rect.Height / 2f;
            float cx = Rect.X + a;
            float cy = Rect.Y + b;
            return ((p.X - cx) * (p.X - cx)) / (a * a) + ((p.Y - cy) * (p.Y - cy)) / (b * b) <= 1;
        }

        public override void Move(int dx, int dy)
        {
            Rect = new Rectangle(Rect.X + dx, Rect.Y + dy, Rect.Width, Rect.Height);
        }
    }
}