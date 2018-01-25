using UnityEngine;

namespace Assets.Scripts.Logs
{
    public class LogHandler : MonoBehaviour
    {
        void OnEnable()
        {
            Application.logMessageReceivedThreaded += LogMessageReceived;
        }

        void OnDisable()
        {
            Application.logMessageReceivedThreaded -= LogMessageReceived;
        }

        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Exception:
                    //dealing with exceptions logs
                    break;
                 case LogType.Error:
                    //dealing with error logs
                    break;
            }
        }
    }
}
