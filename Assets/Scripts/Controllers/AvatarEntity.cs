using System;
using UnityEngine;

/**
 * 
 * Controller for the visual of the avatars
 * 
 */
[Serializable]
public class AvatarEntity : MonoBehaviour
{
    public Renderer leftArm;
    public Renderer rightArm;
    public Renderer leftLeg;
    public Renderer rightLeg;
    public Renderer body;
    public Renderer head;
    public Renderer leftEar;
    public Renderer rightEar;
    public Animator animator;
    public Transform avatar_head_camera;
    // Start is called before the first frame update
    void Start()
    {
        head.materials[0] = Resources.Load("Avatar/Player_Primary", typeof(Material)) as Material;
        leftArm.materials[0] = Resources.Load("Avatar/Player_Primary", typeof(Material)) as Material;
        rightArm.materials[0] = Resources.Load("Avatar/Player_Primary", typeof(Material)) as Material;
        body.materials[0] = Resources.Load("Avatar/Player_Primary", typeof(Material)) as Material;
        leftEar.materials[0] = Resources.Load("Avatar/Player_Primary", typeof(Material)) as Material;
        rightEar.materials[0] = Resources.Load("Avatar/Player_Primary", typeof(Material)) as Material;
        leftLeg.materials[0] = Resources.Load("Avatar/Player_Primary", typeof(Material)) as Material;
        rightLeg.materials[0] = Resources.Load("Avatar/Player_Primary", typeof(Material)) as Material;
        leftArm.materials[1] = Resources.Load("Avatar/Player_Secondary", typeof(Material)) as Material;
        rightArm.materials[1] = Resources.Load("Avatar/Player_Secondary", typeof(Material)) as Material;
        body.materials[1] = Resources.Load("Avatar/Player_Secondary", typeof(Material)) as Material;
        leftEar.materials[1] = Resources.Load("Avatar/Player_Secondary", typeof(Material)) as Material;
        rightEar.materials[1] = Resources.Load("Avatar/Player_Secondary", typeof(Material)) as Material;
        leftLeg.materials[1] = Resources.Load("Avatar/Player_Secondary", typeof(Material)) as Material;
        rightLeg.materials[1] = Resources.Load("Avatar/Player_Secondary", typeof(Material)) as Material;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setAllColor(Color in_color, string in_type)
    {
        int color_index = 0;
        switch (in_type)
        {
            case "Primary":
                color_index = 0;
                head.materials[0].color = in_color;
                break;

            case "Secondary":
                color_index = 1;
                break;
        }

        leftArm.materials[color_index].color = in_color;
        rightArm.materials[color_index].color = in_color;
        body.materials[color_index].color = in_color;
        leftEar.materials[color_index].color = in_color;
        rightEar.materials[color_index].color = in_color;
        leftLeg.materials[color_index].color = in_color;
        rightLeg.materials[color_index].color = in_color;
    }
}
