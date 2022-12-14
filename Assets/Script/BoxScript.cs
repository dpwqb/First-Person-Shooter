using UnityEngine;
public class BoxScript : MonoBehaviour
{
    float BlingTime;
    private void Update()
    {
        if((BlingTime+=Time.deltaTime)>=8)
            Destroy(gameObject);
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
