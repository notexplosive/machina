namespace Machina.Data
{
    public class XYPair<T>
    {
        public T X;
        public T Y;

        public XYPair(T x, T y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}