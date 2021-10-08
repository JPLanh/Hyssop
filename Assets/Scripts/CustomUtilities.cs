using UnityEngine;

/*
 * 
 * A helper class to help with some commonly use functionalities
 * 
 */

public class CustomUtilities
{
    static float precision = 100f;

    public static Vector3 vector3Rounder(Vector3 in_vector)
    {
        float x = (float)Mathf.Round(in_vector.x * precision) / precision;
        float y = (float)Mathf.Round(in_vector.y * precision) / precision;
        float z = (float)Mathf.Round(in_vector.z * precision) / precision;
        return new Vector3(x, y, z);
    }
    public static Quaternion vector3Rounder(Quaternion in_vector)
    {
        float x = (float)Mathf.Round(in_vector.x * precision) / precision;
        float y = (float)Mathf.Round(in_vector.y * precision) / precision;
        float z = (float)Mathf.Round(in_vector.z * precision) / precision;
        float w = (float)Mathf.Round(in_vector.w * precision) / precision;
        return new Quaternion(x, y, z, w);
    }
}