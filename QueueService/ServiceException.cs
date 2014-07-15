using System;

namespace QueueService {

  public class ServiceException : System.Exception {

        public ServiceException(Exception ex, string message, params object[] args) : base(String.Format("System exception: {0} ; User Message: {1}", ex.Message, String.Format(message, args))) {
            LogService.WriteError(message, args);
        }

        public ServiceException(Exception ex, string message) : base(String.Format("System exception: {0} ; User Message: {1}", ex.Message, message)) {
            LogService.WriteError(message);
        }

        public ServiceException(string message, params object[] args) : base(String.Format(message, args)) {
            LogService.WriteError(message, args);
        }
    }
}
