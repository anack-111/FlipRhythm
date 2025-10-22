using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

/*
 * 자주쓰이는 범용적인 함수들 
 */

public static class Util
{

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }


    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }


    public static Color HexToColor(string color)
    {
        Color parsedColor;
        ColorUtility.TryParseHtmlString("#" + color, out parsedColor);

        return parsedColor;
    }
    static public void LimitYVelocity(float limit, Rigidbody2D rb)
    {
        int gravityMultiplier = (int)(Mathf.Abs(rb.gravityScale) / rb.gravityScale);

        if (rb.velocity.y * -gravityMultiplier > limit)
            rb.velocity = Vector2.up * -limit * gravityMultiplier;
    }
    static public void CreateGamemode(Rigidbody2D rb, Movement host, bool onGroundRequired, float initalVelocity, float gravityScale, bool canHold = false, bool flipOnClick = false, float rotationMod = 0, float yVelocityLimit = Mathf.Infinity)
    {
        if (!Input.GetMouseButton(0) || canHold && host.OnGround())
            host._isClickProcessed = false;

        rb.gravityScale = gravityScale * host._gravity;

        LimitYVelocity(yVelocityLimit, rb);

        if (Input.GetMouseButton(0))
        {
            if (host.OnGround() && !host._isClickProcessed || !onGroundRequired && !host._isClickProcessed)
            {
                host._isClickProcessed = true;
                rb.velocity = Vector2.up * initalVelocity * host._gravity;
                host._gravity *= flipOnClick ? -1 : 1;
            }
        }

        if (host.OnGround() || !onGroundRequired)
            host._sprite.rotation = Quaternion.Euler(0, 0, Mathf.Round(host._sprite.rotation.z / 90) * 90);
        else
            host._sprite.Rotate(Vector3.back, rotationMod * Time.deltaTime * host._gravity);
    }

}
