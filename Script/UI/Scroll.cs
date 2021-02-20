using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    private float minPosX = -15f;
    private float maxPosX = 14.5f;
    private float underPosX = -6.5f;
    private float size = 2f;

    private float mainPosY = 1f;
    private float underPosY = -3.57f;

    void Update()
    {
       CameraScroll();
    }

    void CameraScroll()
    {
        Vector2 position = CheckScrollException(GameManager.instance.player.GetComponent<PlayerController>().transform.position);

        position.x = (transform.position.x < position.x) ? position.x : transform.position.x;
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
    Vector2 CheckScrollException(Vector2 Position)
    {
        Vector2 pos = Position;

        if (pos.y < -1f)
        {
            pos.y = underPosY;
        }
        else
        {
            pos.y = mainPosY;
        }

        if (pos.y == underPosY)
        {
            pos.x = underPosX;
        }
        else
        {
            if (pos.x - size < minPosX - size)
            {
                pos.x = minPosX;
            }
            else if (maxPosX + size < pos.x + size)
            {
                pos.x = maxPosX;
            }
        }

        return pos;
    }
}
