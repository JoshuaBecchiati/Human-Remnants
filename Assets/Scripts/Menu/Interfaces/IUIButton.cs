public interface IUIButton
{
    UIButtonType buttonType { get; }
    void OnButtonPressed();
}
