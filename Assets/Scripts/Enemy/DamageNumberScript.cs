using TMPro;
using UnityEngine;

public class DamageNumberScript : MonoBehaviour
{

    public TextMeshProUGUI dmgText;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1.4f);
    }

    // Update is called once per frame
    void Update()
    {
        // Move up
        if(!PauseMenu.isPaused)
            transform.position += new Vector3(0, 0.001f, 0);
    }

    public void setNumber(float number)
    {
        dmgText.SetText(number.ToString());
    }

}
