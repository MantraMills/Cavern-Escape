using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spirit : MonoBehaviour
{
    public Rigidbody2D spiritRb;

    //Gameobject references
    public GameObject refToPlayer;
    public GameObject refToBullet;
    public GameObject refToWall;
    public Player playerScript;
    public SpriteRenderer mySR;

    public float spiritSpeed;
    public float spiritLife = 4f;

    public Vector3 startPos;

    public bool facingRight;

    //Soul block count
    public int blockCount = 0;

    //Bullet count
    public int bulletCount = 0;
    public SpiritOrbs lifeOrb1, lifeOrb2, lifeOrb3, lifeOrb4, lifeOrb5;

    //Start is called before the first frame update

    public void Start()
    {
        this.transform.position = refToPlayer.transform.position + new Vector3(0, 2.5f, 0);

        if (playerScript.facingRight)
        {
            facingRight = true;
        }
        else facingRight = false;

        lifeOrb1.colorFade = "in";
        lifeOrb2.colorFade = "in";
        lifeOrb3.colorFade = "in";
        lifeOrb4.colorFade = "in";
        lifeOrb5.colorFade = "in";
    }

    // Update is called once per frame
    void Update()
    {
        spiritLife -= Time.deltaTime;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(x, y);

        spiritRb.velocity = new Vector2(dir.x * spiritSpeed, dir.y * spiritSpeed);

        if(facingRight == true)
        {
            mySR.flipX = false;
        }
        else if(facingRight == false)
        {
            mySR.flipX = true;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            facingRight = false;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            facingRight = true;
        }

        //Bullet spawn
        if(Input.GetKeyDown(KeyCode.Space) && bulletCount == 0)
        {
            if (facingRight == true)
            {
                Instantiate(refToBullet, transform.position, Quaternion.Euler(0, 0, 90));

                bulletCount += 1;
            }

            else if (facingRight == false)
            {
                Instantiate(refToBullet, transform.position, Quaternion.Euler(0, 0, 270));

                bulletCount += 1;
            }
        }

        if(spiritLife < 4f)
        {
            lifeOrb1.colorFade = "out";
        }
        if (spiritLife < 3.2f)
        {
            lifeOrb2.colorFade = "out";
        }
        if (spiritLife < 2.4f)
        {
            lifeOrb3.colorFade = "out";
        }
        if (spiritLife < 1.6f)
        {
            lifeOrb4.colorFade = "out";
        }
        if (spiritLife < 0.8f)
        {
            lifeOrb5.colorFade = "out";
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(refToPlayer.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(refToPlayer.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        }
    }
}
