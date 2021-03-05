using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Rigidbody2D ball;
    public Collider2D stick;
    public Collider2D hole;
    public Collider2D floor;

    public LineRenderer trajectory;

    public Text score, final, best;
    public GameObject gameOver;

    private int points;
    private int bestScore = 0;

    private Vector3 startPosition;
    private BallState state;

    private Vector2 speed;
    private float thisTime;
    private float force;

    void Start()
    {
        state = BallState.Idle;
        points = 0;

        ball.transform.position = Camera.main.WorldToViewportPoint(new Vector3(-150,-30,0));
        hole.transform.position = Camera.main.WorldToViewportPoint(new Vector3(Random.Range(-50, 130), -34.25f, 0));

        startPosition = ball.transform.position;

        speed = Vector2.zero;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && state == BallState.Idle)
        {
            state = BallState.Hold;
            thisTime = Time.time;
            trajectory.enabled = true;
        }

        if (Input.GetMouseButton(0) && state == BallState.Hold)
        {
            Hold();
        }

        //force at which the arc reaches the edge of the screen
        if (Input.GetMouseButtonUp(0) || force > 9.2f)
        {
            if (state == BallState.Hold)
            {
                Release();
            }
        }

        //the ball stopped without falling into the hole
        if (state == BallState.Release && ball.GetComponent<Rigidbody2D>().velocity.magnitude < 0.1f)
        {
            state = BallState.Finish;
            GameOver();
        }
    }

    private void Hold()
    {
        //The higher the level, the faster the parabola will grow
        force = 5 + (Time.time - thisTime) * (points + 5)/5;                   
        DrawParabola();
    }

    private void Release()
    {
        trajectory.enabled = false;
        speed = new Vector2(force, force);
        ball.GetComponent<Rigidbody2D>().AddForce(speed, ForceMode2D.Impulse);
        state = BallState.Release;
    }
    
    public void BallEnteredHole(Collider2D collision)
    {
        if (collision == stick)
        {
            floor.isTrigger = true;
            stick.isTrigger = true;
            ball.velocity = new Vector2(0, -1);
            state = BallState.Finish;
        }

        if (collision == hole && state == BallState.Finish)
        {
            state = BallState.Hole;
            StartCoroutine(NextLevel());           
        }        
    }

    public void GameOver()
    {
        if (points > bestScore)
        {
            bestScore = points;
        }
        gameOver.SetActive(true);
        final.text = "Your score: " + points.ToString();
        best.text = "Best score: " + bestScore.ToString();
        points = 0;
    }

    public void Restart()
    {
        floor.isTrigger = false;
        stick.isTrigger = false;
        ball.transform.position = startPosition;
        ball.velocity = Vector2.zero;
        hole.transform.position = Camera.main.WorldToViewportPoint(new Vector3(Random.Range(-50, 130), -34.25f, 0));
        score.text = points.ToString();

        state = BallState.Idle;
    }

    public IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(1.5f);
        points++;
        Restart();        
    }


    void DrawParabola()
    {
        int segmentCount = 40;
        float segmentScale = 0.5f;
        
        Vector3[] segments = new Vector3[segmentCount];

        segments[0] = ball.transform.position;

        Vector3 speed = new Vector3(force, force, 0);

        for (int i = 1; i < segmentCount; i++)
        {
            float segTime = (speed.sqrMagnitude != 0) ? segmentScale / speed.magnitude : 0;

            speed = speed + Physics.gravity * segTime;

            segments[i] = segments[i - 1] + speed * segTime;
        }

        trajectory.positionCount = segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            trajectory.SetPosition(i, segments[i]);
        }
    }

    public enum BallState
    {
        Idle,
        Hold,
        Release,
        Hole,
        Finish
    }

}
