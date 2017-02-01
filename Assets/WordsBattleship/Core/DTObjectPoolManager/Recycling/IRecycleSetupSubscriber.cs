using System.Collections;

namespace DTObjectPoolManager {
    public interface IRecycleSetupSubscriber {
        void OnRecycleSetup();
    }
}