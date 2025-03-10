namespace Renderers
{
    public interface IRenderer<T>
    {
        T Data { get; set; }

        public void SetView();

        public void SetView(T Data)
        {
            this.Data = Data;
            SetView();
        }
        public void ToggleVisibility(bool visible);
    }
}