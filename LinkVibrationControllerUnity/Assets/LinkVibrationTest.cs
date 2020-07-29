using LinkVibrationController;
using UnityEngine;

public class LinkVibrationTest : LinkVibration
{
    public void VibrateShort()
    {
        client.Vibrate(0, 500);
    }

    public void VibrateLong()
    {
        client.Vibrate(0, 1000);
    }
}
