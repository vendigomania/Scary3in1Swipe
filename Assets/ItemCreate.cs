using System;
using Code;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class ItemCreate : MonoBehaviour
{
    public GameBoard Game;
    public GameObject PointPrefab;

    private void Start()
    {
        ItemCreateCheckEmptyCellYBelow();
    }

    private void Create(Vector2 position)
    {
        var instantiate = Instantiate(Game.ListItems[Random.Range(0, Game.ListItems.Count)], new Vector3(position.x, position.y, -1f),
            Quaternion.identity);
        instantiate.gameObject.name += " NewItem"; 
        Game._alltemsList.Add(item: instantiate);
    }

    public void ItemCreateCheckEmptyCellYBelow()
    {
        var position = transform.position;
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        Vector2 startPoint = new Vector2(x, y - 3);
        Vector2 direction = Vector2.down;


        RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, Mathf.Infinity);
        if (hit.collider != null && hit.collider.CompareTag("Item"))
        {
            
        //    Debug.Log($"the cell {x},{y} below is occupied");
        }
        else if(hit.transform != null)
        {
           // Instantiate(PointPrefab, hit.transform.position, quaternion.identity);
            Create(hit.transform.position);
            
        }
        GameBoard.Instance.CheckEmptyCellsInBoardBelow();
    }
}