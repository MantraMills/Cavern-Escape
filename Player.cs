using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //Gameobject references
    public Rigidbody2D rb;
    Collision coll;
    public GameObject refToSpirit;
    public GameObject refToMargin;
    //public GameObject spiritBlocks;
    public GameObject refToFlag;
    public GameObject nextLevel;
    public GameObject endGame;
    public GameObject deathBG;

    //Animator
    public Animator anim;

    //Script references
    public GameManager gM;
    public Spirit refSpiritScript;
    public Ghost ghost;
    public LoadScene refToLoad;
    public GameObject refHitBox;

    //Player Stats
    public float speed;
    public float jumpForce;

    //Player Bools
    public bool canMove;
    public bool canJump;
    public bool canDash;
    public bool dashing;
    public bool isOnGround;
    public bool spiritMode;
    public bool facingRight;
    public bool dead;

    //Dash Mechanic
    public float dashSpeed;
    public bool hasDashed;

    //Spirit life timer
    //public float spiritLife = 5f;

    //Line of sight position
    public float xPos;
    public float playerHalfWidth;
    public float rightX, leftX;

    //Sprites
    public SpriteRenderer mySR;

    //Other
    public int jumpSound;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        canMove = true;
        canJump = true;
        spiritMode = false;
        deathBG.SetActive(false);
        facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gM.gameState == GameManager.AllGameStates.Play)
        {
            gameObject.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;          

            if (spiritMode == false && !dead)
            {
                //General Movement
                float x = Input.GetAxis("Horizontal");
                float y = Input.GetAxis("Vertical");
                float xRaw = Input.GetAxisRaw("Horizontal");
                float yRaw = Input.GetAxisRaw("Vertical");
                Vector2 dir = new Vector2(x, y);

                Walk(dir);

                refHitBox.GetComponent<BoxCollider2D>().isTrigger = false;

                if (Input.GetKey(KeyCode.D) && isOnGround || Input.GetKey(KeyCode.RightArrow) && isOnGround)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    facingRight = true;
                    anim.SetBool("Walking", true);
                }
                else if (Input.GetKey(KeyCode.A) && isOnGround || Input.GetKey(KeyCode.LeftArrow) && isOnGround)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    facingRight = false;
                    anim.SetBool("Walking", true);
                }
                else
                {
                    anim.SetBool("Walking", false);
                }

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    return;
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    return;
                }

                //Jump
                if (Input.GetButtonDown("Jump") && canJump && !dashing)
                {
                    anim.SetBool("Jumping", true);
                    Jump(Vector2.up);
                    JumpSound();
                    canDash = true;
                    
                }
                else if(canJump == false)
                {
                    anim.SetBool("Jumping", false);
                }

                //Dash
                if (Input.GetButtonDown("Fire1") && canDash && !hasDashed)
                {
                    print("check");

                    if (xRaw != 0 || yRaw != 0)
                    {
                        Dash(xRaw, yRaw);
                        canJump = false;
                        canDash = false;
                    }
                }

                if (dashing)
                {
                    canDash = false;
                }

                refToSpirit.SetActive(false);
                refToMargin.SetActive(false);
            }

            //Ground Check
            if (coll.onGround)
            {
                canJump = true;
                canDash = false;
                rb.gravityScale = 1;
            }
            else if (!coll.onGround)
            {
                canDash = true;
                canJump = false;
            }

            if (coll.onGround && !isOnGround)
            {
                isOnGround = true;
                hasDashed = false;
            }

            if (!coll.onGround && isOnGround)
            {
                isOnGround = false;
            }

            //Reset
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetScene();
            }


            // Activate spirit mode
            if (Input.GetKeyDown(KeyCode.E))
            {              
                if (spiritMode == false && isOnGround)
                {
                    Vector2 dir = Vector2.zero;
                    rb.velocity = Vector2.zero;
                    spiritMode = !spiritMode;
                    FindObjectOfType<AudioManager>().PlayMusic("Spirit Ambience");
                    anim.SetBool("Idle", true);

                    refToSpirit.SetActive(true);
                    refSpiritScript.Start();
                    refToMargin.SetActive(true);                    
                    canMove = false;

                    //refSpiritScript.OrbColorOn();
                }
            }

            // Spirit mode conditions
            if (spiritMode == true)
            {
                refHitBox.GetComponent<BoxCollider2D>().isTrigger = true;              

                //if(SceneManager.GetActiveScene().name == "scene_1")
                //{
                if (refSpiritScript.spiritLife < 0)
                {
                    spiritMode = false;
                    refSpiritScript.spiritLife = 5f;
                    print("resetSpirit1");
                    FindObjectOfType<AudioManager>().StopMusic("Spirit Ambience");
                    canMove = true;
                    anim.SetBool("Idle", false);
                }
                //    else if (refSpiritScript.spiritLife < 0 && refSpiritScript.blockCount < 3)
                //    {
                //        spiritMode = false;
                //        refSpiritScript.spiritLife = 5f;                     
                //        refSpiritScript.blockCount = 0;
                //        print("resetSpirit2");
                //    }
                //}

                //if (SceneManager.GetActiveScene().name == "scene_2")
                //{
                //    if (refSpiritScript.spiritLife < 0 && refSpiritScript.blockCount == 4)
                //    {
                //        spiritMode = false;
                //        refSpiritScript.spiritLife = 5f;
                //    }
                //    else if (refSpiritScript.spiritLife < 0 && refSpiritScript.blockCount < 4)
                //    {
                //        spiritMode = false;
                //        refSpiritScript.spiritLife = 5f;
                //        refSpiritScript.blockCount = 0;
                //    }
                //}
            }

            // Rock fall trigger
            xPos = transform.position.x;

            playerHalfWidth = GetComponent<SpriteRenderer>().bounds.extents.x;

            leftX = xPos - playerHalfWidth;
            rightX = xPos + playerHalfWidth;
        }

        if (dead)
        {
            gameObject.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

            Scene scene = SceneManager.GetActiveScene();
            string sceneName = scene.name;

            if(sceneName == "scene_1")
            {
                FindObjectOfType<AudioManager>().PauseMusic("Scene 1 Music");
            }

            if (sceneName == "scene_2")
            {
                FindObjectOfType<AudioManager>().PauseMusic("Scene 2 Music");
            }

            if (sceneName == "scene_3")
            {
                FindObjectOfType<AudioManager>().PauseMusic("Scene 3 Music");
            }
        }
        else if (!dead)
        {
            Scene scene = SceneManager.GetActiveScene();
            string sceneName = scene.name;
            /*
            if (sceneName == "scene_1")
            {
                FindObjectOfType<AudioManager>().UnPauseMusic("Scene 1 Music");
            }

            if (sceneName == "scene_2")
            {
                FindObjectOfType<AudioManager>().UnPauseMusic("Scene 2 Music");
            }
            
            if (sceneName == "scene_3")
            {
                FindObjectOfType<AudioManager>().UnPauseMusic("Scene 3 Music");
            }*/
        }
    }

    void Walk(Vector2 dir)
    {
        if (!canMove)
        {
            return;
        }

        if (dashing)
        {
            return;
        }

        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
    }

    void OnGround()
    {
        canDash = false;
        canJump = true;
        hasDashed = false;
    }

    void Jump(Vector2 dir)
    {
        if (dashing)
        {
            return;
        }

        if (!canMove)
        {
            return;
        }

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }

    void JumpSound()
    {
        jumpSound = Random.Range(1, 4);

        if (jumpSound == 1)
        {
            FindObjectOfType<AudioManager>().Play("Jump1");
        }

        if (jumpSound == 2)
        {
            FindObjectOfType<AudioManager>().Play("Jump2");
        }

        if (jumpSound == 3)
        {
            FindObjectOfType<AudioManager>().Play("Jump3");
        }
    }

    void Dash(float x, float y)
    {
        hasDashed = true;
        canMove = false;

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);
        anim.SetBool("Dashing", true);
        anim.SetBool("Jumping", false);
        anim.SetBool("Idle", false);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    public void Death()
    {
        dead = true;       
        anim.SetTrigger("Death");
        deathBG.SetActive(true);

        StartCoroutine(DeathCo());
    }

    public void ResetScene()
    {       
        StartCoroutine(ResetSceneCo());
    }

    IEnumerator DashWait()
    {        
        StartCoroutine(GroundDash());
        rb.gravityScale = 0;
        dashing = true;
        GetComponent<Jump>().enabled = false;
        ghost.makeGhost = true;
        canDash = false;

        yield return new WaitForSeconds(.35f);

        anim.SetBool("Dashing", false);
        rb.gravityScale = 1;
        dashing = false;       
        canMove = true;
        GetComponent<Jump>().enabled = true;
        ghost.makeGhost = false;
        ghost.ghostMade = 0;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
        {
            hasDashed = false;
        }
    }

    IEnumerator DeathCo()
    {
        FindObjectOfType<AudioManager>().Play("Death");
        yield return new WaitForSeconds(1f);
        refToLoad.DeathAnim();     
        yield return new WaitForSeconds(1f);
        FindObjectOfType<AudioManager>().Play("Reset");
        refToLoad.ResetScene();
    }

    IEnumerator ResetSceneCo()
    {     
        refToLoad.DeathAnim();
        yield return new WaitForSeconds(1f);
        FindObjectOfType<AudioManager>().Play("Reset");
        refToLoad.ResetScene();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Ladder")
        {
            print("level completed");

            anim.SetBool("Walking", false);

            FindObjectOfType<AudioManager>().Play("Stage Complete");

            gM.gameState = GameManager.AllGameStates.Pause;

            Instantiate(nextLevel, nextLevel.transform.position, Quaternion.identity);
        }

        if(collision.gameObject.name == "Escape")
        {
            print("game finished");

            anim.SetBool("Walking", false);

            FindObjectOfType<AudioManager>().Play("Stage Complete");

            gM.gameState = GameManager.AllGameStates.Pause;

            Instantiate(endGame, endGame.transform.position, Quaternion.identity);
        }

        if(collision.gameObject.tag == "Rock")
        {
            gM.gameState = GameManager.AllGameStates.Pause;

            Death();
        }
    }
}
