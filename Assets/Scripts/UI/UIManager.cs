using UnityEngine;

public class UIManager : MonoBehaviour
{
    // �̱���
    public UI UI { get; set; }
    public EventPanel EventUI { get; set; }
    public GameObject GameOverUI { get; set; }

    public void CreateUI()
    {
        GameObject temp = Resources.Load("Prefabs/UI/UI") as GameObject;
        GameObject ui = Instantiate(temp, transform.position, Quaternion.identity);
        ui.GetComponent<UI>().Init();
    }
}