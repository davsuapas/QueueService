using System;
using System.Configuration;
using System.Linq;

namespace QueueService {
    public interface IProvider {
        void Load(ApplicationSettingsBase setting);
    }
}
