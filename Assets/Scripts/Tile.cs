using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    private static Color selectedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    private static Color baseColor;

    /// <summary>
    /// Предыдущий выбранный тайл
    /// </summary>
    private static Tile prevSelectedTile = null;

    /// <summary>
    /// Длина рейкаста до соседний тайлов
    /// </summary>
    [SerializeField] float raycastLeght;

    [SerializeField] bool isTileEditor = false;
    [SerializeField] bool isBlock = false;
    [SerializeField] bool isEmpty = false;
    [SerializeField] bool isRandom = false;

    /// <summary>
    /// ссылка на RawImage тайла (не рамки)
    /// </summary>
    [SerializeField] private RawImage image;

    private bool isSelected = false;
    public bool isDelete = false;

    private void Awake()
    {
        baseColor = image.color;
    }

    public void SetIcon(Texture icon)
    {
        image.texture = icon;
    }

    public Texture GetIcon()
    {
        return image.texture;
    }

    public bool GetIsBlock()
    {
        return isBlock;
    }

    public bool GetIsEmpty()
    {
        return isEmpty;
    }

    public bool GetIsRandom()
    {
        return isRandom;
    }

    public void SetIsEmpty(bool isEmpty)
    {
        this.isEmpty = isEmpty;
    }

    public RawImage GetRawImage()
    {
        return image;
    }

    private void Select()
    {
        isSelected = true;
        image.color = selectedColor;
        prevSelectedTile = gameObject.GetComponent<Tile>();
    }

    private void Deselect()
    {
        isSelected = false;
        image.color = baseColor;
        prevSelectedTile = null;
    }

    private void OnMouseDown()
    {
        if (image.texture == null || MainBoard.instance.isShifting)
        {
            return;
        }

        if( (isBlock || isEmpty) && !isTileEditor && !prevSelectedTile.isTileEditor)
        {
            return;
        }

        if (isSelected)
        {
            Deselect();
        }
        else
        {
            if (prevSelectedTile == null)
            {
                if(MenuManager.instance.isGameScene())
                {
                    Select();
                }
                else
                {
                    if(isTileEditor) Select();
                }
                
            }
            else 
            {
                if (prevSelectedTile.isTileEditor)
                {
                    if(!isTileEditor)
                    {
                        CopyIcons(prevSelectedTile.image, image);
                        isBlock = prevSelectedTile.isBlock;
                        isEmpty = prevSelectedTile.isEmpty;
                        isRandom = prevSelectedTile.isRandom;
                    }
                }
                else
                {
                    if (IsAdjacent())
                    {
                        SwapIcons(image, prevSelectedTile.image);
                        MainBoard.instance.PlayerMoved();
                        prevSelectedTile.GetComponent<Tile>().Deselect();
                        DeleteTiles();
                    }
                    else
                    {
                        prevSelectedTile.GetComponent<Tile>().Deselect();
                        Select();
                    }
                } 
            }
        }
    }

    private void DeleteTiles()
    {
        while(MainBoard.instance.FindDeleteTile())
        {
            MainBoard.instance.DeleteTile();
            MainBoard.instance.TileFall();
        }
    }

    /// <summary>
    /// Меняет текстуры двух тайлов
    /// </summary>
    /// <param name="icon1"></param>
    /// <param name="icon2"></param>
    public void SwapIcons(RawImage icon1, RawImage icon2)
    {
        if (icon1.texture == icon2.texture)
        {
            return;
        }

        Texture icon = icon1.texture;
        icon1.texture = icon2.texture;
        icon2.texture = icon;

    }

    /// <summary>
    /// Копирует текстуру одного тайла в другой
    /// </summary>
    /// <param name="source">источник</param>
    /// <param name="destination">куда копируешь</param>
    private void CopyIcons(RawImage source, RawImage destination)
    {
        if (source.texture != destination.texture)
        {
            destination.texture = source.texture;
        }
    }

    /// <summary>
    /// Проверяет был ли предыдущий выбранный тайл соседним
    /// </summary>
    /// <returns></returns>
    private bool IsAdjacent()
    {
        Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        for(int i = 0; i < adjacentDirections.Length; i++)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, adjacentDirections[i], raycastLeght);
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            //Debug.DrawLine(transform.position, transform.position + new Vector3(adjacentDirections[i].x, adjacentDirections[i].y, transform.position.z)*raycastLeght, Color.red, 1);
            if (hit.collider != null && hit.collider.gameObject == prevSelectedTile.gameObject)
            {
                return true;
            }
        }

        return false;
    }
}
