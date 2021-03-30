namespace HalftoneFx.UI
{
    using GUI;

    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    public class UIWinForms : UIManager, IDisposable
    {
        private readonly Stopwatch idleInterval;

        private Control container = null;

        public UIWinForms() : base()
        {
            this.idleInterval = Stopwatch.StartNew();
            Application.Idle += this.IdleHandler;
        }

        public void Dispose()
        {
            this.Container = null;
            Application.Idle -= this.IdleHandler;
        }

        public Control Container
        {
            get => this.container;

            set
            {
                if (this.container == value)
                {
                    return;
                }

                if (this.container != null)
                {
                    this.Unsubscribe(this.container);
                }

                if (value != null)
                {
                    this.Subscribe(value);
                    this.Size = value.ClientSize;
                }

                this.container = value;
            }
        }

        public override void Refresh()
        {
            this.container?.Invalidate();
        }

        private void Subscribe(Control container)
        {
            Debug.Assert(container != null);
            container.Paint += this.PaintHandler;
            container.Resize += this.ResizeHandler;
            container.MouseDown += this.MouseDownHandler;
            container.MouseMove += this.MouseMoveHandler;
            container.MouseUp += this.MouseUpHandler;
            container.MouseWheel += this.MouseWheelHandler;
        }

        private void Unsubscribe(Control container)
        {
            Debug.Assert(container != null);
            container.Paint -= this.PaintHandler;
            container.Resize -= this.ResizeHandler;
            container.MouseDown -= this.MouseDownHandler;
            container.MouseMove -= this.MouseMoveHandler;
            container.MouseUp -= this.MouseUpHandler;
            container.MouseWheel -= this.MouseWheelHandler;
        }

        private void IdleHandler(object sender, EventArgs e)
        {
            if (this.container != null)
            {
                if (this.idleInterval.ElapsedMilliseconds >= 1000 / 60)
                {   
                    this.idleInterval.Restart();
                    this.container.Refresh();
                    this.container.Invalidate();
                }
            }
        }

        private void ResizeHandler(object sender, EventArgs e)
        {
            this.Size = this.container.ClientSize;
            this.container.Invalidate();
        }

        private void PaintHandler(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(this.Style.Colors.Background);
            this.Render(e.Graphics);
        }

        private void MouseDownHandler(object sender, MouseEventArgs e)
        {
            var args = new UIMouseDownEventArgs(e.Button.ToString())
            {
                X = e.X,
                Y = e.Y,
                Clicks = e.Clicks
            };

            this.HandleMouseDown(args);
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            var args = new UIMouseMoveEventArgs()
            {
                X = e.X,
                Y = e.Y,
                Button = UIMouseEventArgs.ParseButtonType(e.Button.ToString())
            };

            this.HandleMouseMove(args);
        }

        private void MouseUpHandler(object sender, MouseEventArgs e)
        {
            var args = new UIMouseUpEventArgs(e.Button.ToString())
            {
                X = e.X,
                Y = e.Y,
                Clicks = e.Clicks
            };

            this.HandleMouseUp(args);
        }

        private void MouseWheelHandler(object sender, MouseEventArgs e)
        {
            var args = new UIMouseWheelEventArgs()
            {
                X = e.X,
                Y = e.Y,
                Delta = e.Delta
            };

            this.HandleMouseWheel(args);
        }
    }
}
