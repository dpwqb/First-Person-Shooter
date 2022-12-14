using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    public GameObject player;
    public GameObject box;
    public float attackDis;
    Animator anim;
    NavMeshAgent nav;
    // Start is called before the first frame update
    void Start()
    {
        player=GameObject.Find("玩家");
        anim=GetComponent<Animator>();
        nav=GetComponent<NavMeshAgent>();
    }
    // Update is called once per frame
    void Update()
    {
        nav.SetDestination(player.transform.position);
        if(Vector3.Distance(transform.position,player.transform.position)<=attackDis)
            anim.SetBool("攻击",true);
        else
            anim.SetBool("攻击",false);
    }
    public void AttackEvent()
    {
        player.GetComponent<Player>().Hurt(5);
    }
    public void EnemyDead()
    {
        anim.SetTrigger("死");
        nav.isStopped=true;
        gameObject.GetComponent<Collider>().enabled=false;
        Destroy(gameObject,3);
        //打掉僵尸后生成的宝箱
        if(Random.Range(0,8)==0)
        Instantiate(box,transform.position+new Vector3(0,0.25f,0),Quaternion.identity);
    }
}
