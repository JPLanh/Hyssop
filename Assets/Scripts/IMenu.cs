using System.Collections;
public interface IMenu : IActionListener
{
    public void accessMenu(PlayerController getPC);
    public Backpack getInventory();
    public ArrayList getAcceptable();
}