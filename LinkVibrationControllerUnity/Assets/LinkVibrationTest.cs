using UnityEngine;
using UnityEngine.UI;
using LinkVibrationController;

public class LinkVibrationTest : MonoBehaviour
{
    public int id;
    public int duration;

    public GameObject LinkVibrationController;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            LinkVibrationController.GetComponent<LinkVibration>().Vibrate(id, duration);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
