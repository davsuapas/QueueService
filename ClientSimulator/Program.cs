using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QueueService;

namespace ClientSimulator {

    class Program {

        static void Main(string[] args) {
            Console.WriteLine("Enviando mensajes"); 
            Publish50Messages("Canal1");
            Publish50Messages("Canal2");
            Console.WriteLine("Mensajes enviados");
            Console.WriteLine("Pulse una tecla para terminar");
            Console.Read();
        }

        private static void Publish50Messages(String channel) {

            PublicationService publication = new PublicationService(
            new QueueInfoEntity() { QueueName = channel },
            new QueueProvider() { ConnectionString = "amqp://guest:guest@david" });

            for (int i = 0; i < 50; i++) {
                publication.Publish(String.Format("Mensaje para el canal: {0}. Número: {1}", channel, i.ToString())); 
            }
        }
    }
}
