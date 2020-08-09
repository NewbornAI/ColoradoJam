
using System.IO;
using UnityEngine;

namespace DysonSwarm
{


    public static class Statics
    {

        public static float GAME_DURATION = 240;

        public static int LAUNCH_COOLDOWN = 10;
        public static int UPKEEP_TIME = 1;

        public static int AU_TO_IG_RATIO = 100;

        public static float MIN = 0.01f;
        public static float MAX = 1000;

        public static string LOG_PATH = "ErrorLog.txt";

        public static float CAM_LOOK_TIME = 5;
        /// <summary>
        /// loss must be negative, return is positive
        /// </summary>
        /// <param name="available"></param>
        /// <param name="loss"></param>
        /// <returns></returns>
        public static float GetAmountLost(float available, float loss)
        {
            if (available < Statics.MIN)
                return 0;
            if (loss > 0)
                return 0;

            return Mathf.Min(Mathf.Abs(loss), available);
        }

        public static void LogDebug(string msg)
        {

            if (!File.Exists(LOG_PATH))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(LOG_PATH))
                {
                    sw.WriteLine(msg);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(LOG_PATH))
                {
                    sw.WriteLine(msg);
                }
            }
        }
        public static void LogError(string msg, string script, string name)
        {
            LogDebug(name + " (" + script + ") - " + msg);
        }

    }

    
}
