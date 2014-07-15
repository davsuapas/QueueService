using System;
using System.Linq;
using System.Diagnostics;

namespace QueueService {

    internal class WorkerProcessManager : ThreadInterface, IDisposable {

        private readonly Action activeChannel;
        private readonly Object sysLock;
        private readonly IProcessExecute process;
        private readonly String channelId;
        private readonly int inSecondProcessMaxTimeWarning;

        internal WorkerProcessManager(Action activeChannel, ChannelEntity channelEntity) {
            this.inSecondProcessMaxTimeWarning = channelEntity.InSecondProcessMaxTimeWarning;
            this.activeChannel = activeChannel;
            this.sysLock = new Object();
            this.channelId = channelEntity.ToString();
            this.process = MefService.Container.GetExportedValueOrDefault<IProcessExecute>(channelEntity.Process);
            Util.ThrowExceptionIfNull(this.process, () => new ServiceException("The process value cannot be null: {0}. Review config file", this.channelId));
            this.process.SetConfiguration(channelEntity.ConfiguractionSection);
        }

        private byte[] message;

        public byte[] Message {
            get {
                lock (this.sysLock) {
                    return this.message;
                }
            }
            set {
                lock (this.sysLock) {
                    this.message = value;
                    this.SetSignal();
                }
            }
        }

        public bool IsMessageNull {
            get {
                lock (this.sysLock) {
                    return this.message == null;
                }
           }
        }

        protected override void RunProcess() {

            bool continueProcess = true;

            Stopwatch chrono = new Stopwatch();

            while (this.IsRunning == ThreadInterface.CONST_TRUE) {

                if (this.WaitSignal()) {

                    if (continueProcess) {

                        LogService.WriteInfo("Processing message for channel: {0}", this.channelId);

                        try {

#if (DEBUG) 
                            Console.WriteLine("Ejecutando proceso: " + this.process.GetType().Name);
#endif

                            chrono.Restart();
                            continueProcess = this.process.Execute(this.message);
                            chrono.Stop();

#if (DEBUG)
                            Console.WriteLine("Fin de proceso: " + this.process.GetType().Name + " en: " + chrono.Elapsed.TotalSeconds + " segundos");
#endif

                            if (chrono.Elapsed.TotalSeconds > this.inSecondProcessMaxTimeWarning)
                                LogService.WriteWarning("You have exceeded the preset time to the process of channel: {0} in {1} seconds", this.channelId, chrono.Elapsed.TotalSeconds);
                        }
                        catch (Exception ex) {
#if (DEBUG)
                            Console.WriteLine("Error de proceso: " + this.process.GetType().Name + " Mensaje: " + ex.Message);
#endif

                            continueProcess = true;

                            new ServiceException(ex, "Error executing queue process: {0}", this.channelId);
                        }

                        if (continueProcess) {
                            lock (sysLock) {
                                this.message = null;
                                this.activeChannel();
                            }
                        }
                        else {
                            new ServiceException("Error. The process : {0} want cancel. The process dont continue", this.channelId);
                        }
                    }
                }
            }

            this.activeChannel();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing) {

            if (!this.disposed) {

                if (disposing) {
                    if (this.process is IDisposable) {
                        Util.TryExecute(() => ((IDisposable)this.process).Dispose());
                    }
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
