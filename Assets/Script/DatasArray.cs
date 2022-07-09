using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatasArray : MonoBehaviour
{
    public int Size;
    public Sprite[] ArraySprite;

    private void Start()
    {
        Size = 0;
    }
    private void Update()
    {
        Size = ArraySprite.Length;
    }
}
