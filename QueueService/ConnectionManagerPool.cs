using System;
using System.Collections.Generic;
using System.Linq;

namespace QueueService {

    internal class ConnectionManagerPool : ThreadInterface {

        private readonly IEnumerable<ConnectionEntity> connectionsEntities;

        internal ConnectionManagerPool(IEnumerable<ConnectionEntity> connectionsEntities) {
            this.connectionsEntities = connectionsEntities;
        }

        protected override void RunProcess(){
            IEnumerable<ConnectionManager> managerConnections;

            try {
                managerConnections = CreateManagerConnections();
                RunAllConnections(managerConnections);
            }
            catch (Exception ex) {
                throw new ServiceException(ex, "Impossible start subscription services. Review queue server connections");
            }

            this.ControllerLifeCycle(managerConnections);
        }

        private void ControllerLifeCycle(IEnumerable<ConnectionManager> managerConnections) {

            while (this.IsRunning == ThreadInterface.CONST_TRUE) {

                if (this.WaitSignal()) {

                    foreach (var managerConnection in managerConnections) {
                        managerConnection.ObserverChannelErrors();
                        managerConnection.ObserverConnectionErrors();
                    }
                }
            }

            foreach (var managerConnection in managerConnections) {
                managerConnection.Close();
            }
        }

        internal void Close() {
            Util.TryExecute(() => {
                this.Stop();
                this.WaitStopped();
            });
        }
  
        private static void RunAllConnections(IEnumerable<ConnectionManager> managerConnections) {
            foreach (var managerConnection in managerConnections) {
                managerConnection.Run();
            }
        }
  
        private IEnumerable<ConnectionManager> CreateManagerConnections() {
            IEnumerable<ConnectionManager> managerConnections = Util.ConvertListFromTo<ConnectionEntity, ConnectionManager>(
                this.connectionsEntities, (source) => new ConnectionManager(source, () => this.SetSignal()));
            return managerConnections;
        }
    }
}
