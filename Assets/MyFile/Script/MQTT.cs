using UnityEngine;
using System.Runtime.InteropServices;

public class MQTT : MonoBehaviour
{
    [Header("Mosquitto Public Config")]
    public string brokerHostname = "test.mosquitto.org";
    public int brokerPort = 8081; // gate WSS of Mosquitto
    public string topic = "hcmus/bot/to/unity/webar/face/acttion/weight/31313131";

    [Header("Reference")]
    public trigger modelTrigger;

    [DllImport("__Internal")]
    private static extern void ConnectWebMQTT(string broker, int port, string topic);

    void Start()
    {
        if (gameObject.name != "MQTT_Manager")
            Debug.LogError(" MQTT_Manager error ");

#if UNITY_WEBGL && !UNITY_EDITOR
            ConnectWebMQTT(brokerHostname, brokerPort, topic);
#else
        Debug.Log("please Build -> WebGL");
#endif
    }

    // chuyen JSON -> JavaScript
    public void ReceiveDataFromWeb(string jsonMessage)
    {
        try
        {
            DataSensor latestData = JsonUtility.FromJson<DataSensor>(jsonMessage);
            if (latestData != null && modelTrigger != null)
            {
                modelTrigger.FaceEmotion = latestData.FaceID;
                modelTrigger.Action = latestData.ActionID;
                modelTrigger.targetWeight = latestData.Weight;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error JSON: " + ex.Message);
        }
    }
}