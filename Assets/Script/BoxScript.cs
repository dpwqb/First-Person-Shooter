using UnityEngine;
public class BoxScript : MonoBehaviour
{
    //存在时间
    float LiveTime;
    //闪烁时间
    float BlingTime;
    //是否显示
    bool Display;
    private void Update()
    {
        if((LiveTime+=Time.deltaTime)>8)
            Destroy(gameObject);
        else if(LiveTime>5&&(BlingTime+=Time.deltaTime)>0.1f)
        {
            BlingTime=0;
            gameObject.GetComponent<MeshRenderer>().enabled=Display=!Display;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            GameObject.Find("玩家").GetComponent<Player>().GetBox();
            Destroy(gameObject);
        }
    }
}
