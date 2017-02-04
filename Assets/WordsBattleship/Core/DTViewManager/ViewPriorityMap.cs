using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DTViewManager.Internal;

namespace DTViewManager {
    public class ViewPriorityMap {
        // PRAGMA MARK - Public Interface
        public ViewPriorityMap() {}

        public ViewPriorityMap(int defaultPriority) {
            this._defaultPriority = defaultPriority;
        }

        public int PriorityForPrefabName(string prefabName) {
            return this._priorityMap.SafeGet(prefabName.ToLower(), defaultValue: this._defaultPriority);
        }

        public void SetPriorityForPrefabName(string prefabName, int priority) {
            this._priorityMap[prefabName.ToLower()] = priority;
        }

        public int DefaultPriority { get { return this._defaultPriority; } }


        // PRAGMA MARK - Internal
        private Dictionary<string, int> _priorityMap = new Dictionary<string, int>();
        private int _defaultPriority = 100;
    }
}