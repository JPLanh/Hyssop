using UnityEngine;
using UnityEngine.UI;

public class progressBar : MonoBehaviour
{
    [SerializeField] private Text textField;
    [SerializeField] private float value;
    [SerializeField] private float initValue;
    [SerializeField] Transform progress;
    private string action;
    private PlayerController currentPlayer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            value += Time.deltaTime;
            progress.localScale = new Vector3((value / initValue), 1, 1);
            if (value >= initValue)
            {
                currentPlayer.unfreeze();
                currentPlayer.doAction(action);
                gameObject.SetActive(false);
                value = 0;
            }
        }
    }

    public void activate(string in_message, float in_value, PlayerController in_player)
    {
        action = in_message;
        textField.text = in_message;
        value = 0;
        initValue = in_value;
        in_player.freeze();
        currentPlayer = in_player;
        gameObject.SetActive(true);
    }
}
