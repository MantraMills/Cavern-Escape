using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //References
    public Vector3 startPos;
    public GameObject refToPlayer;
    public GameObject effect;
    public Player playerScript;
    public GameObject refToBullet;
    public Transform attackPoint;
    public GameManager gM;
    public Animator anim;
    
    //Sprites
    public SpriteRenderer mySR;
    public Rigidbody2D rb;

    //Floats & Bools
    private float speed;
    public float shootingRange;
    public bool chase = false;
    public bool dead = false;
    public bool canShoot;
    public bool canMove;
    public float fireRate = 3;
    private float nextFireTime;

    //HP values
    public int maxHealth = 100;
    int currentHealth;

    //Knockback

    public bool knocked = false;

    //Fixes
    public bool countTime;
    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;

        currentHealth = maxHealth;

        Debug.Log(speed);

        canShoot = true;
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {

        if(gM.gameState == GameManager.AllGameStates.Play)
        {
            
            canShoot = true;
            canMove = true;

            if (canMove)
            {
                if (refToPlayer == null)
                {
                    return;
                }
                if (chase == true && dead == false)
                {
                    Chase();
                }
                else if (chase == false && dead == false)
                {
                    ReturnStartPos();

                    speed = 1f;
                }
                Flip();
            }
        }
        else if(gM.gameState == GameManager.AllGameStates.Pause || playerScript.spiritMode)
        {
            canShoot = false;
            canMove = false;
        }

        if (!playerScript.spiritMode)
        {
            countTime = true;

            if (countTime)
            {
                timer += Time.deltaTime;
            }
        }
        else countTime = false;
    }

    void Chase()
    {
        if (playerScript.spiritMode == false && canShoot)
        {
            transform.position = Vector3.MoveTowards(transform.position, refToPlayer.transform.position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, refToPlayer.transform.position) <= shootingRange && nextFireTime < timer)
            {
                speed = 0.7f;

                Debug.Log(speed);
                Instantiate(refToBullet, this.transform.position, Quaternion.identity);
                nextFireTime = timer + fireRate;
            }
        }
    }

    void Flip()
    {
        if (transform.position.x > refToPlayer.transform.position.x && dead == false)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    
    void ReturnStartPos()
    {
        transform.position = Vector2.MoveTowards(transform.position, startPos, speed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        FindObjectOfType<AudioManager>().Play("Attack");
        StartCoroutine(FlashRed());
        BloodEffect();

        //Play Hurt Animation

        if (currentHealth <= 0)
        {
            Die();
            FindObjectOfType<AudioManager>().Play("Enemy Death");
        }
    }

    void Die()
    {
        Debug.Log("Enemy Died!");       
        //Die Animation
        anim.SetTrigger("Dead");

        //Disable Enemy
        dead = true;
        chase = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground" && dead == true)
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
            GetComponent<Collider2D>().isTrigger = false;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            GetComponent<Collider2D>().enabled = false;

            Destroy(gameObject, 2.5f);
        }
    }

    void BloodEffect()
    {
        Instantiate(effect, transform.position, Quaternion.identity);
    }

    IEnumerator FlashRed()
    {
        mySR.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        mySR.color = Color.white;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
