using UnityEngine;
using UnityEngine.EventSystems;


public class MenuSelectionBox : EventTrigger
{
    [SerializeField] private Hotbar currentHotbar;
    public IActionListener listener;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onClick()
    {
        currentHotbar.doAction();
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        currentHotbar.doHover();
    }
}
