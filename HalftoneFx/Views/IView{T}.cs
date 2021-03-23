namespace HalftoneFx.Views
{
    public interface IView<T>
    {
        T Presenter { get; set; }

        void SetUp();
    }
}
