using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float lineattack;
    [SerializeField] float linerange;
    [SerializeField] float linefollow;

    private Transform player;
    private Animator animate;
    private bool isFacingRight;
    private float prepare;
    public bool isAlive;

    private PlayerHealth playerhealth;

    public GameObject zbullet;
   
    private float zstartfirerate;
    public float zfirerate;
    public bool zshoot;

    // Start is called before the first frame update
    void Start()
    {
        isAlive = true;
        zshoot = false;
        prepare = speed;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerhealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        animate = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector2 playerposition = new Vector2(player.transform.position.x,transform.position.y);
        Vector2 direction = (player.position - transform.position).normalized;

        float distanceplayer = Vector2.Distance(transform.position, playerposition);
       
        if (isAlive)
        {

            if (direction.x > 0 && isFacingRight)
                Flip();
            else if (direction.x < 0 && !isFacingRight)
                Flip();
            
            if (distanceplayer < lineattack)
            {
                prepare = 0;
                animate.SetTrigger("attack");
            }
            else if (distanceplayer > lineattack)
            {
                prepare = speed;
            }

            if (distanceplayer < linefollow)
            {
                this.transform.position = Vector2.MoveTowards(transform.position, playerposition, prepare * Time.deltaTime);
                zshoot = false;
                animate.SetTrigger("run");
            }
            else
            {
                animate.SetTrigger("Idle");
                zshoot = true;
            }

            if (distanceplayer > linerange)
            {
                zshoot = false;
            }

            ZombieShoot();
        }

        if (playerhealth.health <= 0)
        {
            isAlive = false;
            animate.SetTrigger("Idle");
        }

        if (this.transform.position.y < -5)
        {
            Destroy(this.gameObject);
        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180f, 0);
    }
    public void ZombieShoot()
    {
        if (zshoot == true)
        {
            if (zstartfirerate < zfirerate)
                zstartfirerate = zstartfirerate + Time.deltaTime;
            if (zstartfirerate >= zfirerate)
            {
                Instantiate(zbullet, this.transform.position, this.transform.rotation);
                zstartfirerate = 0;
            }
        }  
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lineattack);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, linerange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, linefollow);
    }
}
