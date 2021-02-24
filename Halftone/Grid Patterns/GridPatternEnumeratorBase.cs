namespace Halftone
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public class GridPatternEnumeratorBase : IEnumerator<Point>
    {
        public GridPatternEnumeratorBase(int width, int height, int cellSize)
        {
            this.Width = width;
            this.Height = height;
            this.CellSize = cellSize;
        }

        protected int Width { get; set; }

        protected int Height { get; set; }

        protected int CellSize { get; set; }

        protected int Position { get; set; } = -1;

        object IEnumerator.Current => throw new NotImplementedException();

        public Point Current => this.GetCurrent();

        public virtual void Dispose()
        {
        }

        public virtual bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public virtual void Reset()
        {
            this.Position = -1;
        }

        protected virtual Point GetCurrent()
        {
            throw new NotImplementedException();
        }
    }
}
