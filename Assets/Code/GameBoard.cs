using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code
{
    public class GameBoard : MonoBehaviour
    {
        public static GameBoard Instance { get; private set; }

        public Camera Camera;
        public int XGridSize;
        public int YGridSize;
        public GameObject CellPrefab;
        public GameObject SpawnCell;
        public List<GameObject> ListItems = new();
        public List<LineRenderer> LinesList = new();
        public List<GameObject> _selectItemsList = new();
        public List<GameObject> _alltemsList = new();
        public GameObject _selectItem;
        public GameObject vfxEffect;
        public float AllowedDistance;
        public AudioSource Audio;
        public Game Game;
        public GameObject Grid;
       

        private int _renderedLineIndex;
        private GameObject _currentItemGameObject;
        private List<GameObject> _createsGameObjectsList = new();
        public List<GameObject> _listOfCells = new();
        private List<Vector2> _pointsLine = new();
        private float _offset;


        private void Start()
        {
            Instance = this;

            // GenerateBoard();
        }

        private void Update()
        {
            CheckHit();
        }

        [ContextMenu("Generate")]
        public void GenerateBoard(int xsize, int ysize)
        {
            
            XGridSize = xsize;
            YGridSize = ysize;
            CameraCenter();
            for (int x = 0; x < XGridSize; x++)
            {
                for (int y = 0; y < YGridSize; y++)
                {
                    var cell = Instantiate(CellPrefab, new Vector3(x, y), Quaternion.identity);

                    cell.transform.parent = Grid.transform;

                    _listOfCells.Add(cell);


                    _alltemsList.Add(Instantiate(ListItems[Random.Range(0, ListItems.Count)], new Vector3(x, y, -1f),
                        Quaternion.identity));

                }

                //var spawncell = Instantiate(SpawnCell, new Vector3(x, YGridSize + 2), quaternion.identity);
                //spawncell.transform.parent = Grid.transform;
                //_createsGameObjectsList.Add(spawncell);
                //spawncell.GetComponent<ItemCreate>().Game = this;
            }

        }

        public void CameraCenter()
        {
            Camera.orthographicSize = (XGridSize + YGridSize) / 2 + 2;
            if (XGridSize % 2 == 0)
            {
                _offset = .5f;
            }
            else
            {
                _offset = 0;
            }

            Camera.transform.position = new Vector3(XGridSize / 2 - _offset, YGridSize / 2, -10);
        }

        private void CheckHit()
        {
            if (Input.GetMouseButton(0))
            {
                Ray mousePosition = Camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition.origin, mousePosition.direction, Mathf.Infinity);
                Debug.DrawRay(Camera.transform.position, mousePosition.origin, Color.red);

                if (!hit.collider)
                {
                    return;
                }

                if (hit.collider.GetComponent<Item>())
                {
                    var hitItem = hit.collider.GetComponent<Item>();
                    if (_selectItem != null)
                    {
                        if ((hitItem.Type != _selectItem.GetComponent<Item>().Type))
                        {
                            return;
                        }
                    }

                    if (_selectItem != null)
                    {
                        var distance = Vector3.Distance(hitItem.transform.position, _selectItem.transform.position);
                        if (distance > AllowedDistance)
                        {
                            return;
                        }
                    }


                    if (!hitItem.IsSelect)

                    {
                        hitItem.Selectable();
                        _selectItem = hitItem.gameObject;
                    }

                    else if (
                        hitItem.IsSelect &&
                        hitItem.gameObject.transform.position != _selectItem.transform.position &&
                        _selectItemsList.Count > 1 &&
                        _selectItemsList[^1] != null)
                    {
                        _selectItemsList[^1].gameObject.GetComponent<Item>().Deselect();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                if (_selectItemsList.Count >= 3)
                {
                    int[] towers = new int[XGridSize];

                    for(int i = 0; i < _selectItemsList.Count; i++)
                    {
                        Audio.Play();
                        if (_selectItemsList[i])
                        {
                            vfxEffect.transform.position = _selectItemsList[i].transform.position;
                            vfxEffect.SetActive(true);
                            Invoke("DisableVfx", 0.3f);
                        }

                        Game.Bank(50);
                        Game.GhostTakeDamage(10);

                        foreach(var item in _alltemsList)
                        {
                            if(item != _selectItemsList[i] 
                                && item.transform.position.x == _selectItemsList[i].transform.position.x
                                && item.transform.position.y > _selectItemsList[i].transform.position.y)
                            {
                                item.transform.Translate(Vector2.down);
                            }
                        }
                        int x = Mathf.RoundToInt(_selectItemsList[i].transform.position.x);                        
                        towers[x]++;

                        _alltemsList.Remove(_selectItemsList[i]);
                        Destroy(_selectItemsList[i]);
                    }

                    //foreach (var creat in _createsGameObjectsList)
                    //{
                    //    if (creat != null)
                    //    {
                    //        creat.GetComponent<ItemCreate>().ItemCreateCheckEmptyCellYBelow();
                    //    }
                    //}

                    //CheckEmptyCellsInBoard();

                    for(var i = 0; i < towers.Length; i++)
                    {
                        for(var j = 0; j < towers[i]; j++)
                        {
                            var cell = Instantiate(
                                ListItems[Random.Range(0 , ListItems.Count)], 
                                new Vector3(i, YGridSize - 1 - j, -1f), 
                                Quaternion.identity);
                            _alltemsList.Add(cell);

                            //_createsGameObjectsList[i].GetComponent<ItemCreate>().ItemCreateCheckEmptyCellYBelow();
                        }
                        towers[i] = 0;
                    }

                    CleanBoard();
                }
                else
                {
                    if(Game.Score == 0)
                        foreach (var creat in _createsGameObjectsList)
                        {
                            if (creat != null)
                            {
                                creat.GetComponent<ItemCreate>().ItemCreateCheckEmptyCellYBelow();
                            }
                        }

                    CleanBoard();
                }
            }
        }

        private void CheckEmptyCellsInBoard()
        {
            CheckEmptyCellsInBoardBelow();

            CleanBoard();
        }

        public void CheckEmptyCellsInBoardBelow()
        {
            foreach (var item in _alltemsList)
            {
                if (item != null)
                {
                    item.GetComponent<Item>().CheckEmptyCellBelow();
                }
            }
        }

        private void DisableVfx() => vfxEffect.SetActive(false);

        [ContextMenu("Clean")]
        public void CleanBoard()
        {
            if (_selectItemsList.Count > 0)
            {
                foreach (var select in _selectItemsList.ToList())
                {
                    if (select != null)
                    {
                        select.GetComponent<Item>().Deselect();
                    }
                }
            }

            _selectItem = null;
            _selectItemsList.Clear();
        }
    }
}