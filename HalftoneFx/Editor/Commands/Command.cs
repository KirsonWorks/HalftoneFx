namespace HalftoneFx.Editor
{
    public abstract class Command<T> : ICommand
    {
        protected T Receiver { get; set; }

        public Command(T receiver)
        {
            this.Receiver = receiver;
        }

        public abstract void Execute();

        public abstract void Wayback();
    }
}
