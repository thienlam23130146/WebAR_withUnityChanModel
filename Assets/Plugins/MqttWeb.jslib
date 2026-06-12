mergeInto(LibraryManager.library, {
    ConnectWebMQTT: function (brokerStr, port, topicStr) {
        var broker = UTF8ToString(brokerStr);
        var topic = UTF8ToString(topicStr);

        function initMQTT() {
            var clientId = "webar_client_" + Math.random().toString(16).substr(2, 8);
            
            window.mqttClient = new Paho.MQTT.Client(broker, port, "/", clientId);

            window.mqttClient.onMessageArrived = function (message) {
                SendMessage("MQTT_Manager", "ReceiveDataFromWeb", message.payloadString);
            };

            window.mqttClient.onConnectionLost = function (responseObject) {
                if (responseObject.errorCode !== 0) {
                    console.warn(" [MQTT] Disconeccted: " + responseObject.errorMessage);
                }
            };

            window.mqttClient.connect({
                useSSL: true,
                onSuccess: function () {
                    console.log("[MQTT] connect to Mosquitto Public WSS!");
                    window.mqttClient.subscribe(topic);
                },
                onFailure: function (e) {
                    console.error("[MQTT] Error: " + e.errorMessage);
                }
            });
        }

        if (typeof Paho !== 'undefined') {
            initMQTT(); // Nếu tải rồi thì chạy luôn
        } else {
            var script = document.createElement('script');
            script.src = "https://cdnjs.cloudflare.com/ajax/libs/paho-mqtt/1.0.1/mqttws31.min.js";
            
            script.onload = function () {
                initMQTT();
            };
            
            document.head.appendChild(script);
        }
    }
});