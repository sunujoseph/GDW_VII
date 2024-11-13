using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    //camera references used to calc the parallax later
    private Transform camTrans;
    private Vector3 prevCamPos;

    //Does the background go forever? Speed?
    [SerializeField] private bool infiniteScrollX;
    [SerializeField] private bool infiniteScrollY;
    [SerializeField] private Vector2 scrollSpeed;

    //get the texture of the object to parallax
    private Vector2 textureSize;

    void Start()
    {
        //get main cam
        camTrans = Camera.main.transform;
        //initial camera position
        prevCamPos = camTrans.position;

        //use sprite renderer to grab the texture
        UnityEngine.Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D txtr = sprite.texture;

        //how far do we move on the sprite before we scroll?
        textureSize.x = txtr.width * transform.localScale.x / sprite.pixelsPerUnit;
        textureSize.y = txtr.height * transform.localScale.y / sprite.pixelsPerUnit;
    }

    void LateUpdate()
    {
        //how far have we travelled from the last frame?
        Vector3 deltaMovement = camTrans.position - prevCamPos;

        //scroll background (using the scrollSpeed)
        transform.position += new Vector3(deltaMovement.x * scrollSpeed.x, deltaMovement.y * scrollSpeed.y, 0f);

        //adjust camera variable before moving the cam
        prevCamPos = camTrans.position;

        //account for infinite x scrolling
        if (infiniteScrollX)
        {
            if (Mathf.Abs(camTrans.position.x - transform.position.x) >= textureSize.x)
            {
                //where to move the object to create a parallax
                float positionOffsetX = (camTrans.position.x - transform.position.x) % textureSize.x;
                //use offset to create parallax effect
                transform.position = new Vector3(camTrans.position.x + positionOffsetX, transform.position.y, 0f);
            }
        }
        //account for infinite x scrolling
        if (infiniteScrollY)
        {
            if (Mathf.Abs(camTrans.position.y - transform.position.y) >= textureSize.y)
            {
                //where to move the object to create a parallax
                float positionOffsetY = (camTrans.position.y - transform.position.y) % textureSize.y;
                //use offset to create parallax effect
                transform.position = new Vector3(transform.position.x, camTrans.position.y + positionOffsetY, 0f);
            }
        }
    }
}
