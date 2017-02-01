using System.Collections;

namespace DTObjectPoolManager {
    public interface IRecycleCleanupSubscriber {
        void OnRecycleCleanup();
    }
}