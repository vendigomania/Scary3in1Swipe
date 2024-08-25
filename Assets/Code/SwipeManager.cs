using UnityEngine;

namespace Code
{
    public class SwipeManager : MonoBehaviour
    {
        private Vector2 fingerDownPosition;
        private Vector2 fingerUpPosition;
    
        public bool detectSwipeOnlyAfterRelease = false;
        public float minDistanceForSwipe = 20f;
    
        public static event System.Action<SwipeData> OnSwipeDetected;
    
        private void Update()
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    fingerUpPosition = touch.position;
                    fingerDownPosition = touch.position;
                }
    
                if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
                {
                    fingerDownPosition = touch.position;
                    DetectSwipe();
                }
    
                if (touch.phase == TouchPhase.Ended)
                {
                    fingerDownPosition = touch.position;
                    DetectSwipe();
                }
            }
        }

        private void DetectSwipe()
        {
            if (SwipeDistanceCheckMet())
            {
                if (IsVerticalSwipe())
                {
                    var direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                    SendSwipe(direction);
                    Debug.Log(direction);
                }
                else
                {
                    var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                    SendSwipe(direction);
                    Debug.Log(direction);
                }
                fingerUpPosition = fingerDownPosition;
            }
        }
    
        bool IsVerticalSwipe()
        {
            return VerticalMovementDistance() > HorizontalMovementDistance();
        }
    
        bool SwipeDistanceCheckMet()
        {
            return VerticalMovementDistance() > minDistanceForSwipe || HorizontalMovementDistance() > minDistanceForSwipe;
        }
    
        float VerticalMovementDistance()
        {
            return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
        }
    
        float HorizontalMovementDistance()
        {
            return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
        }
    
        void SendSwipe(SwipeDirection direction)
        {
            SwipeData swipeData = new SwipeData()
            {
                Direction = direction,
                StartPosition = fingerDownPosition,
                EndPosition = fingerUpPosition
            };
            OnSwipeDetected?.Invoke(swipeData);
        }
    }
}