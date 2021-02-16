namespace GUI
{
    using System;
    using System.Drawing;

    public enum UIMouseEventType
    {
        Down,
        Move,
        Up,
        Wheel
    }

    public enum UIMouseButtons
    {
        None,
        Left,
        Middle,
        Right
    }

    public class UIMouseEventArgs : EventArgs
    {
        public UIMouseEventArgs()
        {
        }

        public UIMouseEventArgs(UIMouseEventType eventType)
        {
            this.EventType = eventType;
        }

        public UIMouseEventArgs(UIMouseEventType eventType, UIMouseButtons button) : this(eventType)
        {
            this.Button = button;
        }

        public UIMouseEventArgs(UIMouseEventType eventType, UIMouseButtons button, int x, int y, int clicks, int delta) : this(eventType)
        {
            this.EventType = eventType;
            this.Button = button;
            this.X = x;
            this.Y = y;
            this.Clicks = clicks;
            this.Delta = delta;
        }

        public UIMouseEventArgs(UIMouseEventType eventType, string button, int x, int y, int clicks, int delta)
            : this(eventType, ParseButtonType(button), x, y, clicks, delta)
        {
        }

        public UIMouseEventType EventType { get; set; }

        public UIMouseButtons Button { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public PointF Location => new PointF(this.X, this.Y);

        public int Clicks { get; set; }

        public int Delta { get; set; }

        public static UIMouseButtons ParseButtonType(string text)
        {
            Enum.TryParse(text, out UIMouseButtons mouseButton);
            return mouseButton;
        }
    }

    public class UIMouseDownEventArgs : UIMouseEventArgs
    {
        public UIMouseDownEventArgs(string button) : base(UIMouseEventType.Down, ParseButtonType(button))
        {
        }
    }
    public class UIMouseUpEventArgs : UIMouseEventArgs
    {
        public UIMouseUpEventArgs(string button) : base(UIMouseEventType.Up, ParseButtonType(button))
        {
        }
    }

    public class UIMouseMoveEventArgs : UIMouseEventArgs
    {
        public UIMouseMoveEventArgs() : base(UIMouseEventType.Move)
        {
        }
    }

    public class UIMouseWheelEventArgs : UIMouseEventArgs
    {
        public UIMouseWheelEventArgs() : base(UIMouseEventType.Wheel)
        {
        }
    }
}
