using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpGate : MonoBehaviour
{
    public enum InsertKey
    {
        KEY_NONE = 0,
        KEY_UP = UnityEngine.KeyCode.UpArrow,
        KEY_DOWN = UnityEngine.KeyCode.DownArrow,
        KEY_LEFT = UnityEngine.KeyCode.LeftArrow,
        KEY_RIGHT = UnityEngine.KeyCode.RightArrow,
    }

    public enum OutDir
    {
        DIR_UP = 0,
        DIR_DOWN,
        DIR_LEFT,
        DIR_RIGHT,
    }

    public GameObject warpOut;
    public InsertKey insertKey;
    public OutDir outDir;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ("Player" == collision.transform.tag)
        {
            if (InsertKey.KEY_NONE == insertKey)
                return;

            if (InsertKey.KEY_DOWN == insertKey)
            {
                float size = transform.GetComponent<BoxCollider2D>().size.x / 4f;

                if (collision.transform.position.x < transform.position.x - size || transform.position.x + size < collision.transform.position.x)
                    return;
            }

            if (true == Input.GetKeyDown((UnityEngine.KeyCode)insertKey))
            {
                PlayerController player = GameManager.instance.player.GetComponent<PlayerController>();

                if (null == player)
                    return;

                float size = (InsertKey.KEY_DOWN == insertKey) ? transform.GetComponent<BoxCollider2D>().size.x / 2f : transform.GetComponent<BoxCollider2D>().size.y / 2f;
                Vector3 dirOut = GetOutDirect();
                Vector3 pos = warpOut.transform.position + (dirOut * size);

                player.SetWarpPosition(pos, GetinDirect(), dirOut);
            }
        }
    }

    private Vector3 GetOutDirect()
    {
        switch (outDir)
        {
            case OutDir.DIR_UP:
                return Vector3.up;
            case OutDir.DIR_DOWN:
                return Vector3.down;
            case OutDir.DIR_LEFT:
                return Vector3.left;
            case OutDir.DIR_RIGHT:
                return Vector3.right;
        }

        return Vector3.up;
    }

    private Vector3 GetinDirect()
    {
        switch (insertKey)
        {
            case InsertKey.KEY_DOWN:
                return Vector3.down;
            case InsertKey.KEY_LEFT:
                return Vector3.left;
            case InsertKey.KEY_RIGHT:
                return Vector3.right;
        }

        return Vector3.up;
    }
}

