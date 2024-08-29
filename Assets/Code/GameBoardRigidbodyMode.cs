using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameBoardRigidbodyMode : MonoBehaviour
{
    
   // public List<LineRenderer> LinesList = new();
    public Camera Camera;
    public List<GameObject> ListItems = new();
    public float AllowedDistance;
    public int XSizeBoard;
    public int YSizeBoard;
    public GameObject CellPrefab;

    private int _indexLineRenderer;
    private List<Vector2> _pointsLine = new();
    public List<GameObject> _selectItemsList = new();
    private GameObject _currentItem;
    public GameObject _selectItem;
    private List<GameObject> _listCells = new();
    private List<GameObject> _listCorrectItem = new();


    private void Start()
    {
        GenerateBoard();
    }

    private void Update()
    {
        CheckHit();
    }

    [ContextMenu("Generate")]
    public void GenerateBoard()
    {
        //XSize = Convert.ToInt32(InputFieldX.text);
        //ZSize = Convert.ToInt32(InputFieldZ.text);
        for (int x = 0; x < XSizeBoard; x++)
        {
            for (int y = 0; y < YSizeBoard; y++)
            {
                var cell = Instantiate(CellPrefab, new Vector3(x, y), Quaternion.identity);
                var item = Instantiate(ListItems[Random.Range(0, ListItems.Count)], new Vector3(x, y),
                    Quaternion.identity);

                _listCells.Add(cell);
            }
        }
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
                if (!hitItem.IsSelect)
                {
                    hitItem.Selectable();
                    _selectItem = hitItem.gameObject;
                }
                
              // else if (
              //     hitItem.IsSelect && 
              //     hitItem.gameObject.transform.position != _selectItem.transform.position &&
              //     _selectItemsList.Count > 1 && 
              //     _selectItemsList[^1] != null)
              // {
              //     _selectItemsList[^1].gameObject.GetComponent<Item>().Deselect();
              //     
              // }
                else
                {
                    return;
                }
            }
        }
        else
        {
            Cleanup();
        }

       
    }

    private void CheckingSimilarities()
    {
        if (_selectItemsList.Count > 0)
        {
            foreach (var VARIABLE in _selectItemsList)
            {
                
            }
        }
    }
    private void SelectableItem(RaycastHit2D hit)
    {
        var item = hit.collider.GetComponent<Item>();
        if (!_selectItemsList.Contains(hit.collider.GetComponent<Item>().gameObject))
        {
            _selectItem.GetComponent<SpriteRenderer>().color = Color.green;
            _selectItemsList.Add(item.gameObject);
            // AddLine(item.gameObject.transform.position);
            item.IsSelect = true;
            Debug.Log($"Add Item");
        }
        else
        {
            // Deselect(item);
            Debug.Log($"Item already in list ");
            return;
        }
    }

    private void Deselect(Item item)
    {
        _selectItem.GetComponent<SpriteRenderer>().color =
            _selectItem.GetComponent<Item>().OriginalColor;

        item.IsSelect = false;
        var lastObject = _selectItemsList[^1];

        if (lastObject != null)
        {
            lastObject.GetComponent<SpriteRenderer>().color =
                lastObject.GetComponent<Item>().OriginalColor;
        }

        _selectItemsList.Remove(_selectItem);
    }

   

    [ContextMenu("Clean")]
    public void Cleanup()
    {
      //  _pointsLine.Clear();
      //  foreach (var line in LinesList)
      //  {
      //      line.positionCount = 0;
      //  }

        foreach (var select in _selectItemsList.ToList())
        {
            select.GetComponent<Item>().Deselect();
        }

       // _selectItemsList.Clear();
    }
}


// if (_selectItem != null)
// {
//     if (hit.collider.gameObject.transform.position == _selectItem.transform.position)
//     {
//         return;
//     }
// }
//
//
// _selectItem = hit.collider.gameObject;
//
// if (_currentItem == null)
// {
//     _currentItem = _selectItem;
//     hit.collider.GetComponent<Item>().Selectable();
//     //SelectableItem(hit);
// }
// else
// {
//     var distnace = Vector3.Distance(_currentItem.transform.position,
//         _selectItem.transform.position);
//     // Debug.Log($"Distance {distnace}");
//     if (_currentItem.GetComponent<Item>().Type == _selectItem.GetComponent<Item>().Type &&
//         distnace < AllowedDistance)
//     {
//         _currentItem = _selectItem;
//         hit.collider.GetComponent<Item>().Selectable();
//         //SelectableItem(hit);
//     }
//     else
//     {
//         _currentItem = null;
//         Cleanup();
//         return;
//     }
// }