using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QueueService;
using System.IO;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;

namespace HostService {

    class Program {

        static void Main(string[] args) {

            try {
                InitializeMef();
                var config = (IEnumerable<ConnectionEntity>)ConfigurationManager.GetSection("SubcriptionGroup/SubcriptionSection");
                var sub = new SubcriptionWorker(config);
                System.Console.WriteLine("Iniciando servicio de subcripción");
                sub.Run();
                System.Console.WriteLine("Servicio de subcripción iniciado");
                System.Console.WriteLine("Pulse una tecla para terminar");
                System.Console.ReadLine();
                sub.Stop();
            }
            catch (Exception ex) {
                System.Console.WriteLine("Error: " + ex.Message);
                System.Console.WriteLine("Pulse una tecla para terminar");
                System.Console.ReadLine();
            }
        }

        private static void InitializeMef() {
            var catalog = new AggregateCatalog(new DirectoryCatalog(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(Program)).Location), "BusinessProcess")));
            MefService.Init(catalog);
        }
    }
}
