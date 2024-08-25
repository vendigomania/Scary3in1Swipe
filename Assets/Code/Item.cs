using Code;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameBoardRigidbodyMode GameRbMode;
    public GameObject PointPrefab;
    public Animator Animator;
    public AudioSource Audio;

    public enum ItemType
    {
        Circle,
        Square,
        Sliced,
        Triangle,
        Hex
    }

    public bool IsSelect = false;
    public Color OriginalColor;
    public ItemType Type;
    public SpriteRenderer SpriteRenderer;
    public GameObject Gpx;


    private void Start()
    {
        //GameBoard.Instance.CheckEmptyCellsInBoardBelow();
    }

    public void Selectable()
    {
        SpriteRenderer.color = Color.green;
        IsSelect = true;
        Animator.SetTrigger("Select");
        Audio.Play();
        if (!GameBoard.Instance._selectItemsList.Contains(gameObject))
        {
            GameBoard.Instance._selectItemsList.Add(gameObject);
        }
        else
        {
            return;
        }


        Debug.Log($"Add Item");
    }

    public void Deselect()
    {
        SpriteRenderer.color = OriginalColor;
        IsSelect = false;
        Animator.SetTrigger("Deselect");
        if (GameBoard.Instance._selectItemsList.Contains(gameObject))
        {
            GameBoard.Instance._selectItemsList.Remove(gameObject);
        }
        else
        {
            return;
        }
    }


    public void CheckEmptyCellBelow()
    {
        if (gameObject)
        {
            CheckEmptyCellBelow(transform.position);
        }
    }
    public void CheckEmptyThisCell()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, Mathf.Infinity);
        
        if (hit.collider != null && hit.collider.CompareTag("Item"))
        {
           Destroy(hit.collider.gameObject);
        }
    }

    private void CheckEmptyCellBelow(Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        Vector2 startPoint = new Vector2(x, y - 1);
        Vector2 direction = Vector2.down;

        if (y > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, Mathf.Infinity);
            if (hit.collider != null && hit.collider.CompareTag("Item"))
            {
                // Debug.Log($"the cell {x},{y} below is occupied");
            }
            else
            {
                // Instantiate(PointPrefab, hit.transform.position, quaternion.identity);
                //  Debug.Log($"the cell {x},{y} below is free");
                if (hit)
                {
                    Vector3 newPosition = hit.transform.position + Vector3.back;
                    RecheckingEmptyCellBelow(newPosition);
                }
            }
        }
    }

    private void RecheckingEmptyCellBelow(Vector3 position)
    {
        // Transform currentGpxTransform = Gpx.transform;
        // currentGpxTransform.DOMove(position, 0.2f);
        transform.position = position;
        CheckEmptyThisCell();

        //GameBoard.Instance.CheckEmptyCellsInBoardBelow();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("collision");
        if (other.collider != null && other.collider.CompareTag("Item"))
        {
            Destroy(gameObject);
        }
    }
}