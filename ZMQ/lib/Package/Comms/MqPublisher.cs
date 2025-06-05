/*
**  XIF MqPublisher.cs
**
**  Abstraction class for publishing to a ZeroMQ topic.
*/

using System;
using System.Text;
using System.Text.Json;

using NetMQ;
using NetMQ.Sockets;

using XIF.Comms.Common;

namespace XIF.Comms
{
    public class MqPublisher: IDisposable
    {
        private PublisherSocket publisher;

        private static readonly JsonSerializerOptions _aotCompatibleJsonOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = ApplicationJsonContext.Default
        };

        public MqPublisher(PortDefinition port)
        {
            AsyncIO.ForceDotNet.Force();
            NetMQConfig.Linger = new TimeSpan(0, 0, 1);

            publisher = new PublisherSocket();
            publisher.Options.Linger = new TimeSpan(0, 0, 1);
            publisher.Bind($"tcp://127.0.0.1:{(int)port}");
            Console.WriteLine($"ZeroMQ Server started on tcp://127.0.0.1:{(int)port}");     
        }

        /* generic transmit method */
        public void Transmit<T>(string topic, T data)
        {
            publisher.SendMoreFrame(topic).SendFrame(JsonSerializer.Serialize<T>(data, _aotCompatibleJsonOptions));
        }

        public void Dispose()
        {
            publisher.Dispose();
        }

    }
}
