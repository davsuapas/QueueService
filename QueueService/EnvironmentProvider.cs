using System;
using System.Configuration;
using System.Linq;

namespace QueueService {

    public static class EnvironmentProvider {

        private static readonly IProvider[] providers;

        static EnvironmentProvider() {
            lock (typeof(EnvironmentProvider)) {
                providers = new IProvider[] { new QueueProvider() };
            }
        }

        public static void Load(ApplicationSettingsBase setting) {
            lock (typeof(EnvironmentProvider)) {
                foreach (var provider in providers) {
                    provider.Load(setting);
                }
            }
        }

        public static T GetProvider<T>() where T : IProvider {
            return (T)providers.FirstOrDefault((p) => p is T);
        }
    }
}
