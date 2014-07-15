using System.Configuration;
using System.Linq;

namespace QueueService {

    public class QueueProvider : IProvider {

        public string ConnectionString { get; set; }

        public void Load(ApplicationSettingsBase setting) {
            ConnectionString = (string)Util.TryGetSetting(setting, "QueueConnectionString");
        }
    }
}
