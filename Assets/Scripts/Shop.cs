using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable, IMenu, IActionListener
{
    PlayerController activePC;
    public Entity currentNPC;
    ArrayList acceptable;
    public string prevPlayerState;
    GameObject exitMenu;


    public void accessMenu(PlayerController getPC)
    {

    }

    public ArrayList getAcceptable()
    {
        return acceptable;
    }

    public Backpack getInventory()
    {
        return currentNPC.backpack;
    }

    public void interact(PlayerController getInteractor, bool in_modified)
    {
        if (!getInteractor.isPaused)
        {
            activePC = getInteractor;
            prevPlayerState = getInteractor.playerEntity.state;
            getInteractor.playerEntity.state = currentNPC.state;
            getInteractor.mainMenu.focusShop = this;
            Dictionary<string, string> list_Dialog = currentNPC.getDialog();
            string prompt = "Prompt 1";
            getInteractor.newMessage(currentNPC.entityName, list_Dialog[prompt]);
            string[] promptParser = prompt.Split(' ');
            foreach (KeyValuePair<string, string> it_prompt in list_Dialog)
            {
                string[] responseParser = it_prompt.Key.Split('.');
                if (responseParser[0].Equals(prompt) && responseParser.Length != 1)
                {

                    getInteractor.newResponse(it_prompt.Value);
                }
            }
        }
    }


    public IActionListener getActionListener()
    {
        return this;
    }

    public void listen(string getAction)
    {

    }

    public void setActionListener(IActionListener listener)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        acceptable = new ArrayList();
        acceptable.Add("Money");

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void reaction(PlayerController getInteractor)
    {
        throw new System.NotImplementedException();
    }
}
