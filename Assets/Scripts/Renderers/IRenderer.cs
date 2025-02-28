namespace Renderers
{
    public interface IRenderer<T>
    {
        public void SetView(T data);
        public void ToggleVisibility(bool visible);
    }
}