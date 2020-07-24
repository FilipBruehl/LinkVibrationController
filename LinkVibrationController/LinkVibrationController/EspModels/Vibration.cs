using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkVibrationController.EspModels
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
