namespace LinkVibrationController.models
{
    /// <summary>
    /// Dataclass for every vibration
    /// </summary>
    public class Vibration
    {
        /// <summary>
        /// Duration in ms
        /// </summary>
        public int Duration;

        /// <summary>
        /// Id of the vibration motor
        /// </summary>
        public VibratorId Id;
    }
}