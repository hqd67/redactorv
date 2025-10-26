using redactorv;
using System.Drawing;

namespace redactorv
{
    public class RectangleShape : Shape
    {
        public Rectangle Rect { get; private set; }

        public RectangleShape(Rectangle rect, Color color) : base(color)
        {
            Rect = rect;
        }

        public override void Draw(Graphics g)
        {
            using (Brush b = new SolidBrush(Color))
            {
                g.FillRectangle(b, Rect);
            }
            if (IsSelected)
            {
                using (Pen p = new Pen(Color.Black, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
                {
                    g.DrawRectangle(p, Rect);
                }
            }
        }

        public override bool ContainsPoint(Point p) => Rect.Contains(p);

        public override void Move(int dx, int dy)
        {
            Rect = new Rectangle(Rect.X + dx, Rect.Y + dy, Rect.Width, Rect.Height);
        }
    }
}