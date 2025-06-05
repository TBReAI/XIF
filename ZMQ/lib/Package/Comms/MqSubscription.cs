/*
**  XIF MqSubscription.cs
**
**  Abstraction class for subscribing to a ZeroMQ topic.
*/

using System;
using System.Text;
using System.Text.Json;

using NetMQ;
using NetMQ.Sockets;

using XIF.Comms.Common;

namespace XIF.Comms
{

    public class MqSubscription: IDisposable
    {
        private SubscriberSocket subscriberSocket;

        private static readonly JsonSerializerOptions _aotCompatibleJsonOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = ApplicationJsonContext.Default
        };

        public MqSubscription(PortDefinition port, string topic)
        {
            subscriberSocket = new SubscriberSocket();
            subscriberSocket.Options.Linger = new System.TimeSpan(0, 0, 1);
            subscriberSocket.Connect($"tcp://127.0.0.1:{(int)port}");
            subscriberSocket.Subscribe(topic);
        }

        /* generic receive method */
        public bool TryReceive<T>(ref T frame)
        {
            string? topic;
            byte[]? payload;

            bool gotTopic = subscriberSocket.TryReceiveFrameString(out topic);

            if (gotTopic)
            {
                bool gotPayload = subscriberSocket.TryReceiveFrameBytes(out payload);

                if (gotPayload)
                {
                    T? deserialized = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(payload!), _aotCompatibleJsonOptions);
                    if (deserialized != null)
                    {
                        frame = deserialized;
                        return true;
                    }
                }
            }

            return false;
        }

        public void Dispose()
        {
            subscriberSocket.Dispose();
        }

    }
}
