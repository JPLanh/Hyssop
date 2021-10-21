using System.Collections;
using UnityEngine;

public class NonPlayerController : MonoBehaviour
{
    public EntityExistanceDTO<EntityDTO> playerEntity;
    public Vector3 currentRotation;
    public GameObject currentObject;
    public Animator entityAnimation;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position != playerEntity.position ||
            currentRotation != playerEntity.rotation)
        {
            print(playerEntity.rotation);
            print(transform.rotation + " , " + transform.eulerAngles);
            transform.eulerAngles = new Vector2(0, playerEntity.rotation.x);
            print(transform.rotation + " , " + transform.eulerAngles);
            //        userHead.localRotation = Quaternion.Euler(playerEntity.rotation.y, 0, 0);
            //            StartCoroutine(LerpPosition(positionToMoveTo, 5));
            entityAnimation.SetBool("isWalking", true);
            transform.position = playerEntity.position;
            currentRotation = playerEntity.rotation;
        }
        else
        {
            entityAnimation.SetBool("isWalking", false);

        }
    }
    IEnumerator LerpPosition(Vector2 targetPosition, float duration)
    {
        float time = 0;
        Vector2 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}

class EntityUpdateWrapper
{
    public string entityName;
    public string areaName;
    public string worldName;
    public Vector3 position;
    public Quaternion rotation;
}