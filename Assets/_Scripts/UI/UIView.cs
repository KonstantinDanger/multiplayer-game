public class UIView : UI
{
    public void Open()
        => gameObject.SetActive(true);

    public void Close()
        => gameObject.SetActive(false);
}
