using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShouldFlip : MonoBehaviour
{
    public bool facingRight;
    public bool canFlip = true;
    public Vector3 currentTarget;
    SpriteRenderer myRend;
    Grid grid;
    Unit myUnit;
    List<Vector3> surroundingEnemies;

    private void Start()
    {
        myRend = GetComponentInChildren<SpriteRenderer>();
        grid = GameObject.Find("A*").GetComponent<Grid>();
        myUnit = GetComponent<Unit>();
        canFlip = true;
    }

    private void Update()
    {
        if (myUnit.alive)
        {
            if (canFlip)
            {
                if (myUnit.moving == false)
                {
                    surroundingEnemies = new List<Vector3>();
                    surroundingEnemies.Clear();

                    surroundingEnemies = grid.GetSurroundingEnemyLocations(transform.position, myUnit.team);

                    for (int i = 0; i < surroundingEnemies.Count; i++)
                    {
                        currentTarget = surroundingEnemies[i];
                        CompareLocation();
                    }
                }
                else
                    CompareLocation();
            }
        }
    }

    public void StartFlipDelay( float time)
    {
        StartCoroutine(FlipDelay(time));
    }

    IEnumerator FlipDelay(float time)
    {
        canFlip = false;
        yield return new WaitForSeconds(time);
        canFlip = true;

    }

    public void CompareLocation()
    {
        Vector3 myPos = transform.position;

        Vector3 myScreenPos = Camera.main.WorldToScreenPoint(myPos);
        Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(currentTarget);
        
        if(targetScreenPos.x > myScreenPos.x) //should face right
        {
            if (!facingRight)
            {
                Flip(false);
            }
        }
        if(targetScreenPos.x < myScreenPos.x) //should face left
        {
            if (facingRight)
            {
                Flip(true);
            }
        }
    }

    private void Flip(bool faceLeft)
    {
        myRend.flipX = faceLeft;
        facingRight = !faceLeft;
    }
}
