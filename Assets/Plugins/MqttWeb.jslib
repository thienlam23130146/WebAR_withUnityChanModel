mergeInto(LibraryManager.library, {
    ConnectWebMQTT: function (brokerStr, port, topicStr) {
        var broker = UTF8ToString(brokerStr);
        var topic = UTF8ToString(topicStr);

        var script = document.createElement('script');
        script.src = "https://cdnjs.cloudflare.com/ajax/libs/paho-mqtt/1.0.1/mqttws31.min.js";
        
        script.onload = function () {
            var clientId = "webar_client_" + Math.random().toString(16).substr(2, 8);
            window.mqttClient = new Paho.MQTT.Client(broker, port, "", clientId);

            window.mqttClient.onMessageArrived = function (message) {
                SendMessage("MQTT_Manager", "ReceiveDataFromWeb", message.payloadString);
            };

            window.mqttClient.connect({
                useSSL: true,
                onSuccess: function () {
                    console.log("Đã kết nối Mosquitto Public WSS!");
                    window.mqttClient.subscribe(topic);
                },
                onFailure: function (e) {
                    console.error("Error MQTT: " + e.errorMessage);
                }
            });
        };
        document.head.appendChild(script);
    }
});