using System.Drawing;

namespace redactorv
{
    public abstract class Shape
    {
        public Color Color { get; set; }
        public bool IsSelected { get; set; }

        public Shape(Color color)
        {
            Color = color;
            IsSelected = false;
        }

        public abstract void Draw(Graphics g);
        public abstract bool ContainsPoint(Point p);
        public abstract void Move(int dx, int dy);
    }
}