using System;
using System.Linq;
using System.Threading;

namespace QueueService {

    internal abstract class ThreadInterface {

        public const long CONST_TRUE = 1;
        public const long CONST_FALSE = 0;

        protected readonly Guid id;

        protected internal ThreadInterface()
        {
            this.signal = new AutoResetEvent(false);
            this.hasError = CONST_FALSE;
            this.isRunning = CONST_FALSE;
            id = Guid.NewGuid();
        }
        
        private readonly AutoResetEvent signal;

        protected bool WaitSignal() {
            bool result = this.signal.WaitOne();
            if (this.isRunning == CONST_FALSE)
                return false;
            else
                return result;
        }

        internal bool SetSignal() {
            return this.signal.Set();
        }

        internal void Stop() {
            this.IsRunning = CONST_FALSE;
            this.SetSignal();
        }

        internal void WaitStopped() {
            while (this.IsStopped == CONST_FALSE) {
                System.Threading.Thread.Sleep(500);
            }
        }

        private long isStopped;
        internal long IsStopped {
            get {
                return Interlocked.Read(ref this.isStopped);
            }
            set {
                Interlocked.Exchange(ref this.isStopped, value);
            }
        }

        private long isRunning;
        internal long IsRunning {
            get {
                return Interlocked.Read(ref this.isRunning);
            }
            set {
                Interlocked.Exchange(ref this.isRunning, value);
            }
        }
       
        private long hasError;
        internal long HasError {
            get {
                return Interlocked.Read(ref this.hasError);
            }
            set {
                Interlocked.Exchange(ref this.hasError, value);
            }
        }

        internal delegate void ErrorEventHandler(object sender, ThreadErrorEventArgs e);
        internal event ErrorEventHandler ErrorEvent;

        internal void OnErrorEventHandler(Exception ex) {
            if (this.ErrorEvent != null)
                this.ErrorEvent(this, new ThreadErrorEventArgs(ex));
        }

        internal void Start() {
            this.HasError = CONST_FALSE;
            this.IsStopped = CONST_FALSE;
            this.IsRunning = CONST_TRUE;
        }

        internal void Run() {
            this.RunProcess();

            this.IsRunning = CONST_FALSE;
            this.IsStopped = CONST_TRUE;
        }

        protected abstract void RunProcess();
    }
}
