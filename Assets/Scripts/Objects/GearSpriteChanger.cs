using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearSpriteChanger : MonoBehaviour
{
    public SpriteRenderer MainRenderer;
    public SpriteRenderer MaskRenderer;
    
    public List<Sprite> SpriteList;

    private void Start()
    {
        int randomNumber = Random.Range(0, SpriteList.Count);

        MainRenderer.sprite = SpriteList[randomNumber];
        MaskRenderer.sprite = SpriteList[randomNumber];
    }
}
