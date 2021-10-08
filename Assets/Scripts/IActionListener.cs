public interface IActionListener
{
    public void setActionListener(IActionListener listener);
    public IActionListener getActionListener();
    public void listen(string getAction);
}