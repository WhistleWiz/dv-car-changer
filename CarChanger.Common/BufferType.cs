using UnityEngine;

namespace CarChanger.Common
{
    public enum BufferType
    {
        [Tooltip("Don't change the buffers")]
        Original        = 0,
        [Tooltip("Used by Boxcar")]
        Buffer02        = 400,
        [Tooltip("Used by Stock, DE2, DH4, DM3, DM1U")]
        Buffer03        = 350,
        [Tooltip("Used by Gondola, Utility Flatbed, S060")]
        Buffer04        = 550,
        [Tooltip("Used by DE6, DE6 Slug, Microshunter")]
        Buffer05        = 70,
        [Tooltip("Used by Military Boxcar, Refrigerator")]
        Buffer06        = 450,
        [Tooltip("Used by Caboose, Tank, Short Tank")]
        Buffer07        = 300,
        [Tooltip("Used by Passenger")]
        Buffer08        = 600,
        [Tooltip("Used by Autorack, Flatbed, Flatbed Military, Flatbed Stakes, Hopper, Closed Hopper, Nuclear Flask")]
        Buffer09        = 200,
        [Tooltip("Used by S282")]
        S282            = 20,
        [Tooltip("Use your own custom bogie")]
        Custom          = 10000
    }
}
