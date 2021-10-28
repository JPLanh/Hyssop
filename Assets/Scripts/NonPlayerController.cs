using System.Collections;
using UnityEngine;

public class NonPlayerController : MonoBehaviour
{
    public EntityExistanceDTO<EntityDTO> playerEntity;
    public Vector3 currentRotation;
    public GameObject currentObject;
    public Animator entityAnimation;
    public avatarProperties current_avatar;
    public float lastUpdate;

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
            transform.eulerAngles = new Vector2(0, playerEntity.rotation.x);
            //        userHead.localRotation = Quaternion.Euler(playerEntity.rotation.y, 0, 0);
            //            StartCoroutine(LerpPosition(positionToMoveTo, 5));
//            entityAnimation.SetBool("isWalking", true);
            transform.position = playerEntity.position;
            currentRotation = playerEntity.rotation;
        }
        else
        {
//            entityAnimation.SetBool("isWalking", false);

        }

        if (Time.time - lastUpdate >= .5f && lastUpdate != 0f)
        {
            InGameListener.removeCharacter(playerEntity._id);
        }
    }


    public void load()
    {
        current_avatar = new avatarProperties(playerEntity.entityObj.currentAnimal);
        current_avatar.primary_currentRed = playerEntity.entityObj.primary_currentRed;
        current_avatar.primary_currentGreen = playerEntity.entityObj.primary_currentGreen;
        current_avatar.primary_currentBlue = playerEntity.entityObj.primary_currentBlue;
        current_avatar.secondary_currentBlue = playerEntity.entityObj.secondary_currentBlue;
        current_avatar.secondary_currentRed = playerEntity.entityObj.secondary_currentRed;
        current_avatar.secondary_currentGreen = playerEntity.entityObj.secondary_currentGreen;
        current_avatar.currentAvatar = Instantiate(Resources.Load<GameObject>("Avatar/" + playerEntity.entityObj.currentAnimal), new Vector3(0f, 0f, 0f), Quaternion.identity);
        current_avatar.currentAvatar.transform.SetParent(transform);
        current_avatar.currentAvatar.transform.localPosition = new Vector3(-0f, -0f, -0f);
        current_avatar.currentAvatar.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);


        if (current_avatar.currentAvatar.TryGetComponent<AvatarEntity>(out AvatarEntity out_avatarEntity))
        {
            current_avatar.current_avatarEntity = out_avatarEntity;
            entityAnimation = out_avatarEntity.animator;
        }
        current_avatar.current_avatarEntity.setAllColor(new Color(current_avatar.primary_currentRed / 255f, current_avatar.primary_currentGreen / 255f, current_avatar.primary_currentBlue / 255f), "Primary");
        current_avatar.current_avatarEntity.setAllColor(new Color(current_avatar.secondary_currentRed / 255f, current_avatar.secondary_currentGreen / 255f, current_avatar.secondary_currentBlue / 255f), "Secondary");
        shopCheck();
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

    private void shopCheck()
    {
        switch (playerEntity.entityObj.entityName)
        {
            case "Izak":
            case "Trevik":
                Shop temp_shop = gameObject.AddComponent<Shop>();
                temp_shop.currentNPC = playerEntity.entityObj.getActual();
                break;

        }
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