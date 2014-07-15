using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Collections.Generic;

namespace QueueService {

    public static class MefService {

        private static CompositionContainer _container;
        public static CompositionContainer Container {
            get {
                return _container;
            }
        }

        public static void Init(ComposablePartCatalog catalogs) {
            _container = new CompositionContainer(catalogs,
                    CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);
            _container.ComposeParts();
            _container.ComposeExportedValue<CompositionContainer>(_container);
        }

        public static void Dispose() {
            Container.Dispose();
        }
    }
}
