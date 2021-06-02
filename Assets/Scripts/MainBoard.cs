using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainBoard : MonoBehaviour
{
    /// <summary>
    /// хранит ссылку на скрипт
    /// </summary>
    public static MainBoard instance;
    /// <summary>
    /// Загружаемый уровень
    /// </summary>
    public static int loadLevel;

    /// <summary>
    /// Ссылка на текст для отображения количества очков
    /// </summary>
    [SerializeField] private Text point;
    /// <summary>
    /// класс с массивом с целями уровня
    /// </summary>
    [SerializeField] private Target_List targetInput;

    /// <summary>
    /// шаблон тайла
    /// </summary>
    [SerializeField] private GameObject tile;

    /// <summary>
    /// размеры поля
    /// </summary>
    [SerializeField] private int numX, numY;
    /// <summary>
    /// отступ между тайлами
    /// </summary>
    [SerializeField] private float offset;

    /// <summary>
    /// массив иконок для тайлов
    /// </summary>
    [SerializeField] private List<Texture> icons = new List<Texture>();

    /// <summary>
    /// хранит цели уровня (текстура, количество тайлов)
    /// </summary>
    private Dictionary<Texture, int> targetPoint = new Dictionary<Texture, int>();

    /// <summary>
    /// добавляемое количество очков за тайл
    /// </summary>
    [SerializeField] private int addPoint = 50;
    /// <summary>
    /// количество очков
    /// </summary>
    private int gamePoint = 0;

    /// <summary>
    /// количество ходов
    /// </summary>
    [SerializeField] private int numMove = 10;
    /// <summary>
    /// Ссылка на текст для отображения количества ходов
    /// </summary>
    [SerializeField] private Text moveText;

    /// <summary>
    /// Должны ли тайлы удаляться или генерироваться на доске?
    /// </summary>
    [SerializeField] private bool isDeleteEmpty = false;
    /// <summary>
    /// текстура пустого тайла
    /// </summary>
    [SerializeField] private Texture emptyTexture;

    /// <summary>
    /// Создавать доску пустой?
    /// </summary>
    [SerializeField] private bool isCreateEmpty = true;
    /// <summary>
    /// шаблон пустого тайла
    /// </summary>
    [SerializeField] private GameObject emptyTile;

    /// <summary>
    /// забыл убрать, пусть будет
    /// </summary>
    public bool isShifting;

    /// <summary>
    /// Массив тайлов
    /// </summary>
    private GameObject[,] tiles;

    void Start()
    {
        instance = GetComponent<MainBoard>();

        if(isCreateEmpty)
        {
            CreateBoardEmpty();
        }
        else
        {
            CreateBoard();
        }
        
    }

    /// <summary>
    /// возвращает количество ходов
    /// </summary>
    /// <returns>numMove</returns>
    public int GetNumMove()
    {
        return numMove;
    }

    /// <summary>
    /// устанавливает количество ходов
    /// </summary>
    /// <param name="numMove"></param>
    public void SetNumMove(int numMove)
    {
        this.numMove = numMove;
    }
    /// <summary>
    /// Устанавливает количество ходов в текстовом поле
    /// </summary>
    /// <param name="numMove">количество ходов</param>
    private void ReloadMove(int numMove)
    {
        if(MenuManager.instance.isGameScene())
        {
            moveText.text = numMove.ToString();
        }
        else
        {
            moveText.GetComponentInParent<InputField>().text = numMove.ToString();
        }
    }
    /// <summary>
    /// уменьшает количество ходов
    /// </summary>
    public void PlayerMoved()
    {
        numMove--;
        if (numMove <= 0) GameManager.GameOver();
        ReloadMove(numMove);
    }

    /// <summary>
    /// Возвращает цели уровня
    /// </summary>
    /// <returns></returns>
    public Dictionary<Texture, int> GetTargetPoint()
    {
        return targetPoint;
    }

    /// <summary>
    /// устанавливает цели уровня
    /// </summary>
    /// <param name="targetPoint"></param>
    public void SetTargetPoint(Dictionary<Texture, int> targetPoint)
    {
        this.targetPoint = targetPoint;
    }

    /// <summary>
    /// возвращает массив с тайлами
    /// </summary>
    /// <returns></returns>
    public GameObject[,] GetTiles()
    {
        return tiles;
    }

    /// <summary>
    /// принимает новый массив с тайлами
    /// </summary>
    /// <param name="tiles"></param>
    public void SetTiles(GameObject[,] tiles)
    {
        this.tiles = tiles;
    }

    /// <summary>
    /// Устанавливает количество очков и ходов
    /// </summary>
    /// <param name="numMovesText"></param>
    public void SaveGameSetting(Text numMovesText)
    {
        for(int i = 0;i < targetInput.targetList.Length; i++)
        {
            int point = System.Convert.ToInt32(targetInput.targetList[i].point.GetComponent<InputField>().text);
            if (point < 0) point = 0;
            if (targetPoint.ContainsKey(targetInput.targetList[i].icon.texture))
            {
                targetPoint[targetInput.targetList[i].icon.texture] = point;
            }
            else
            {
                targetPoint.Add(targetInput.targetList[i].icon.texture, point);
            }
        }
        numMove = System.Convert.ToInt32(numMovesText.text);
        Debug.Log("SaveGameSetting");
    }

    /// <summary>
    /// Сохраняет уровень
    /// </summary>
    /// <param name="levelText"></param>
    public void SaveBoard(InputField levelText)
    {
        int level = System.Convert.ToInt32(levelText.text);
        if(level>0)
        {
            Save.SaveGame(level);
        }
        else
        {
            Debug.LogError("error: incorrect save level number");
        }
    }

    /// <summary>
    /// Загружает уровень
    /// </summary>
    /// <param name="levelText"></param>
    public void LoadBoard(InputField levelText)
    {
        int level = System.Convert.ToInt32(levelText.text);
        if (level > 0)
        {
            Save.LoadGame(level);
            ReloadTargetPoint();
            ReloadMove(numMove);
        }
        else
        {
            Debug.LogError("error: incorrect load level number");
        }
    }

    /// <summary>
    /// Загружает уровень
    /// </summary>
    /// <param name="level"></param>
    public void LoadBoard(int level)
    {
        if (level > 0)
        {
            Save.LoadGame(level);
            ReloadTargetPoint();
            ReloadMove(numMove);
        }
        else
        {
            Debug.LogError("error: incorrect load level number");
        }
    }

    /// <summary>
    /// Проверяет выполнена ли цель уровня
    /// </summary>
    /// <returns></returns>
    private bool CheckTarget()
    {
        for(int i =0;i< icons.Count; i++)
        {
            if (targetPoint[icons[i]] > 0) return false;
        }
        return true;
    }

    /// <summary>
    /// Добавляет очки и уменьшает цель уровня
    /// </summary>
    /// <param name="icon"></param>
    private void AddPoint(Texture icon)
    {
        gamePoint += addPoint;
        point.text = gamePoint.ToString();
        if(targetPoint[icon] > 0) targetPoint[icon]--;
        ReloadTargetPoint();
        if (CheckTarget()) GameManager.GameWin();
    }

    /// <summary>
    /// Создает доску со случайными тайлами
    /// </summary>
    private void CreateBoard()
    {
        tiles = new GameObject[numX, numY];

        float startX = transform.position.x;
        float startY = transform.position.y;

        float fullOffset = tile.GetComponent<RectTransform>().sizeDelta.x + offset;

        Texture[] previousLeft = new Texture[numY];
        Texture previousBelow = null;

        for (int x = 0; x < numX; x++)
        {
            for(int y = 0; y < numY; y++)
            {
                GameObject newTile = Instantiate(tile, new Vector3(startX + (fullOffset * x), startY + (fullOffset * y), 0), tile.transform.rotation);
                newTile.name = "(" + (x+1) + "," + (y+1) + ")";
                tiles[x, y] = newTile;
                newTile.transform.SetParent(transform);

                List<Texture> possibleIcons = new List<Texture>();
                possibleIcons.AddRange(icons);

                possibleIcons.Remove(previousLeft[y]);
                possibleIcons.Remove(previousBelow);
                
                Texture newIcon = possibleIcons[Random.Range(0, possibleIcons.Count)];
                newTile.GetComponent<Tile>().SetIcon(newIcon);

                previousLeft[y] = newIcon;
                previousBelow = newIcon;

            }
        }       
    }

    /// <summary>
    /// Создает доску с пустыми тайлами
    /// </summary>
    private void CreateBoardEmpty()
    {
        tiles = new GameObject[numX, numY];

        float startX = transform.position.x;
        float startY = transform.position.y;

        float fullOffset = tile.GetComponent<RectTransform>().sizeDelta.x + offset;

        for (int x = 0; x < numX; x++)
        {
            for (int y = 0; y < numY; y++)
            {
                GameObject newTile = Instantiate(emptyTile, new Vector3(startX + (fullOffset * x), startY + (fullOffset * y), 0), tile.transform.rotation);
                newTile.name = "(" + (x + 1) + "," + (y + 1) + ")";
                tiles[x, y] = newTile;
                newTile.transform.SetParent(transform);

            }
        }
    }

    /// <summary>
    /// Удаляет все тайлы со сцены
    /// </summary>
    private void DeleteAllTileObject()
    {
        for (int x = 0; x < numX; x++)
        {
            for (int y = 0; y < numY; y++)
            {
                Destroy(this.tiles[x, y]);
            }
        }
    }

    /// <summary>
    /// Меняет все тайлы на доске на пустые
    /// </summary>
    public void ClearBoard()
    {
        DeleteAllTileObject();
        CreateBoardEmpty();
    }

    /// <summary>
    /// Подгружает новую доску
    /// </summary>
    /// <param name="tiles"></param>
    public void ReloadBoard(GameObject[,] tiles)
    {
        float startX = transform.position.x;
        float startY = transform.position.y;

        float fullOffset = tile.GetComponent<RectTransform>().sizeDelta.x + offset;

        Texture[] previousLeft = new Texture[numY];
        Texture previousBelow = null;

        for (int x = 0; x < numX; x++)
        {
            for (int y = 0; y < numY; y++)
            {
                Destroy(this.tiles[x, y]);
                GameObject newTile = Instantiate(tiles[x,y], new Vector3(startX + (fullOffset * x), startY + (fullOffset * y), 0), tile.transform.rotation);
                newTile.name = "(" + (x + 1) + "," + (y + 1) + ")";
                this.tiles[x, y] = newTile;
                newTile.transform.SetParent(transform);

                List<Texture> possibleIcons = new List<Texture>();
                possibleIcons.AddRange(icons);

                possibleIcons.Remove(previousLeft[y]);
                possibleIcons.Remove(previousBelow);

                Texture newIcon = possibleIcons[Random.Range(0, possibleIcons.Count)];
                if (newTile.GetComponent<Tile>().GetIsRandom())
                {
                    newTile.GetComponent<Tile>().SetIcon(newIcon);
                }

                previousLeft[y] = newTile.GetComponent<Tile>().GetRawImage().texture;
                previousBelow = newTile.GetComponent<Tile>().GetRawImage().texture;

                
            }
        }
    }

    /// <summary>
    /// Подгружает актуальные цели уровня на сцену
    /// </summary>
    private void ReloadTargetPoint()
    {
        for (int i = 0; i < targetInput.targetList.Length; i++)
        {
            if(MenuManager.instance.isGameScene())
            {
                targetInput.targetList[i].point.GetComponentInChildren<Text>().text = targetPoint[targetInput.targetList[i].icon.texture].ToString();
            }
            else
            {
                targetInput.targetList[i].point.GetComponent<InputField>().text = targetPoint[targetInput.targetList[i].icon.texture].ToString();
            }
        }
    }

    /// <summary>
    /// Поиск и отметка тайлов, которые нужно удалить
    /// </summary>
    /// <returns></returns>
    public bool FindDeleteTile()
    {
        bool isFindDeleteTile = false;
        for (int x = 0; x < numX; x++)
        {
            for (int y = 0; y < numY; y++)
            {
                if(!tiles[x,y].GetComponent<Tile>().GetIsBlock() && !tiles[x, y].GetComponent<Tile>().GetIsEmpty())
                {
                    if (x > 0 && x < numX - 1)
                    {
                        if (tiles[x, y].GetComponent<Tile>().GetIcon() == tiles[x - 1, y].GetComponent<Tile>().GetIcon() &&
                           tiles[x, y].GetComponent<Tile>().GetIcon() == tiles[x + 1, y].GetComponent<Tile>().GetIcon())
                        {
                            tiles[x, y].GetComponent<Tile>().isDelete = true;
                            tiles[x - 1, y].GetComponent<Tile>().isDelete = true;
                            tiles[x + 1, y].GetComponent<Tile>().isDelete = true;
                            isFindDeleteTile = true;
                        }
                    }
                    if (y > 0 && y < numY - 1)
                    {
                        if (tiles[x, y].GetComponent<Tile>().GetIcon() == tiles[x, y + 1].GetComponent<Tile>().GetIcon() &&
                           tiles[x, y].GetComponent<Tile>().GetIcon() == tiles[x, y - 1].GetComponent<Tile>().GetIcon())
                        {
                            tiles[x, y].GetComponent<Tile>().isDelete = true;
                            tiles[x, y - 1].GetComponent<Tile>().isDelete = true;
                            tiles[x, y + 1].GetComponent<Tile>().isDelete = true;
                            isFindDeleteTile = true;
                        }
                    }
                }
            }
        }
        return isFindDeleteTile;
    }

    /// <summary>
    /// Удаляет отмеченные тайлы
    /// </summary>
    public void DeleteTile()
    {
        for (int x = 0; x < numX; x++)
        {
            for (int y = 0; y < numY; y++)
            {
                if (tiles[x, y].GetComponent<Tile>().isDelete == true)
                {
                    AddPoint(tiles[x, y].GetComponent<Tile>().GetIcon());
                    tiles[x, y].GetComponent<Tile>().SetIcon(null);
                    tiles[x, y].GetComponent<Tile>().isDelete = false;
                }
            }
        }
    }

    /// <summary>
    /// Падение тайлов
    /// </summary>
    public void TileFall()
    {

        for (int x = 0; x < numX; x++)
        {
            for(int y = numY -1; y>= 0; y--)
            {
                if (tiles[x, y].GetComponent<Tile>().GetIcon() == null)
                {
                    int nowY = y;
                    int nextY;
                    for(nextY = y+1; nextY< numY; nextY++)
                    {
                        if (tiles[x, nextY].GetComponent<Tile>().GetIsBlock()) break;
                        if (tiles[x, nextY].GetComponent<Tile>().GetIsEmpty()) continue;
                        tiles[x, nowY].GetComponent<Tile>().SwapIcons(tiles[x, nowY].GetComponent<Tile>().GetRawImage(), tiles[x, nextY].GetComponent<Tile>().GetRawImage());
                        nowY = nextY;
                    }

                    if(isDeleteEmpty)
                    {
                        tiles[x, nowY].GetComponent<Tile>().SetIcon(emptyTexture);
                        tiles[x, nowY].GetComponent<Tile>().SetIsEmpty(true);
                    }
                    else
                    {
                        tiles[x, nowY].GetComponent<Tile>().SetIcon(icons[Random.Range(0, icons.Count)]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Перевод библиотеки целей уровня в массив чисел
    /// </summary>
    /// <param name="targetPoint"></param>
    /// <returns>Возвращает массив целей уровня относительно массива иконок(icons)</returns>
    public int[] DictionaryToInt(Dictionary<Texture, int> targetPoint)
    {
        int[] newTargetPoint = new int[targetPoint.Count];

        for(int i = 0; i < icons.Count; i++)
        {
            newTargetPoint[i] = targetPoint[icons[i]];
        }

        return newTargetPoint;
    }

    /// <summary>
    /// Перевод числового массива целей уровня в библиотеку 
    /// </summary>
    /// <param name="targetPoint"></param>
    /// <returns>Возвращает библиотеку целей уровня относительно массива иконок(icons)</returns>
    public Dictionary<Texture, int> IntToDictionary(int[] targetPoint)
    {
        Dictionary<Texture, int> newTargetPoint = new Dictionary<Texture, int>();

        for (int i = 0; i < icons.Count; i++)
        {
            newTargetPoint.Add(icons[i], targetPoint[i]);
        }

        return newTargetPoint;
    }

    /// <summary>
    /// Перевод массива объектов в массив чисел относительно массива шаблонов тайлов
    /// </summary>
    /// <param name="tiles"></param>
    /// <returns></returns>
    public int[,] GameobjectToInt(GameObject[,] tiles)
    {
        Tile_List tileList = GameObject.FindObjectOfType<Tile_List>();
        int[,] newTiles = new int[numX, numY];
        if(tileList != null)
        {
            for(int x=0;x< numX;x++)
            {
                for(int y =0;y<numY;y++)
                {
                    newTiles[x, y] = tileList.FindIndexForTexture(tiles[x, y].GetComponent<Tile>().GetRawImage().texture);
                }
            }
        }
        return newTiles;
    }

    /// <summary>
    /// Перевод числового массива в массив объектов относительно массива шаблонов тайлов
    /// </summary>
    /// <param name="tiles"></param>
    /// <returns></returns>
    public GameObject[,] IntToGameobject(int[,] tiles)
    {
        Tile_List tileList = GameObject.FindObjectOfType<Tile_List>();
        GameObject[,] newTiles = new GameObject[numX, numY];
        if (tileList != null)
        {
            for (int x = 0; x < numX; x++)
            {
                for (int y = 0; y < numY; y++)
                {
                    newTiles[x, y] = tileList.tiles[tiles[x, y]];
                }
            }
        }
        return newTiles;
    }

    
}
