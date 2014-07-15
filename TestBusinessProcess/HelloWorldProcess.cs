using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QueueService;
using System.ComponentModel.Composition;
using System.Xml;

namespace TestBusinessProcess {

    [Export("HelloWorldProcess", typeof(IProcessExecute))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HelloWorldProcess : ProcessExecute<MyConfiguration>, IDisposable {

        public HelloWorldProcess() {
        }

        // Prepara la configuración que le viene dado por el fichero .config
        protected override MyConfiguration GetConfiguration(XmlNodeList configNodeList) {
            return new MyConfiguration() { Param1 = Util.GetValueOrDefault<string>("Param1", () => configNodeList[0].Attributes["Param1"].Value, "Nothing", false) };
        }

        // Ejecuta el proceso de negocio asociado al mensaje enrutado por rabbit
        public override bool Execute(byte[] message) {

            Console.WriteLine("Nombre del canal por donde viene el mensaje: " + this.ConfigurationSection.Param1 +
            ". Mensaje a procesar por hola mundo:" + Serializer.Unserialize<String>(message) +
            ". Hilo:" + System.Threading.Thread.CurrentThread.ManagedThreadId);

            System.Threading.Thread.Sleep(1000); // Simula que el proceso dura un segundo en ejecutarse

            return true; // Todo ha sido satisfactorio y quiero seguir procesando mensajes
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing) {

            if (!this.disposed) {

                if (disposing) {
                    // Si hubiera algo que liberar se haría aquí
                }

                disposed = true;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
