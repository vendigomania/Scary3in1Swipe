using Code;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
   public GameBoard GameBoard;
   public TMP_Text ScoreText;
   public TMP_Text TimeText;
   public int Score;
   public GameObject Ghost;
   public bool Play;
   public float GhostSpeed = 2;
   public Vector3 GhostTransform;
   public bool isPause;
   public int LevelSize;
   public float Timer;
   public GameObject WinPanel;
   public GameObject LosePanel;
   public GameObject PausePanel;
   public GameObject MainCanvas;

   private void Start()
   {
      GhostTransform = Ghost.transform.position;
      Play = false;
      ScoreText.text = Score.ToString();
   }

   private void Update()
   {
      if (Play)
      {
         ProgressLevel();
      }
   }

   public void StartGame(int size)
   {
      Timer = 120;
      LevelSize = size;
      GameBoard.GenerateBoard(size,size);
      Ghost.transform.position = GhostTransform;
      Play = true;
      Score = 0;
      ScoreText.text = Score.ToString();
      WinPanel.SetActive(false);
      LosePanel.SetActive(false);
      Time.timeScale = 1;
   }
   [ContextMenu("Restart")]
   public void RestartGame()
   {
      foreach (var item in GameBoard._alltemsList)
      {
         Destroy(item);
      }
      foreach (var cell in GameBoard._listOfCells)
      {
         Destroy(cell);
      }
      GameBoard._alltemsList.Clear();
      GameBoard._listOfCells.Clear();
      StartGame(LevelSize);
      
      
   }

   public void MenuButton()
   {
      Play = false;
      foreach (var item in GameBoard._alltemsList)
      {
         Destroy(item);
      }
      foreach (var cell in GameBoard._listOfCells)
      {
         Destroy(cell);
      }
      GameBoard._alltemsList.Clear();
      GameBoard._listOfCells.Clear();
      Ghost.transform.position = GhostTransform;
      MainCanvas.SetActive(true);
      
   }
   public void NextLevel()
   {
      
   }

   public void Pause()
   {
      isPause = !isPause;
      PausePanel.SetActive(isPause);
      Time.timeScale = isPause ? 0 : 1;
   }

   public void GhostTakeDamage(int value)
   {
      Ghost.transform.position += Vector3.left * value ;
   }
   private void ProgressLevel()
   {
      Timer -= Time.deltaTime;
      TimeText.text = Timer.ToString("0");
      Ghost.transform.position += Vector3.right * GhostSpeed * Time.deltaTime;
      if (Timer <0 )
      {
         Win();
      }
   }

   private void Win()
   {
      WinPanel.SetActive(true);
      Time.timeScale = 0;
   }
   public void Lose()
   {
      LosePanel.SetActive(true);
      Time.timeScale = 0;
   }

   public void Bank(int value)
   {
      Score += value;
      ScoreText.text = Score.ToString();
   }
   
}
