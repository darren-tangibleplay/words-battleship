using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tangible {

// Self sufficient class
public class ConnectionHelper {
	//public delegate void OnNewPair(TangibleObject o1, TangibleObject o2);

	class Connection {
		public readonly TangibleObject o1;
		public readonly TangibleObject o2;
		public Connection(TangibleObject _o1, TangibleObject _o2) {
			o1 = _o1;
			o2 = _o2;
		}
	};

	private static int CompareId(TangibleObject a, TangibleObject b) {
		return a.Id.CompareTo(b.Id);
	}

	private List<Connection> connections = new List<Connection>();

	private Connection FindConnection(TangibleObject _o1, TangibleObject _o2) {
		for (int i = 0; i < connections.Count; i++) {
			if (connections[i].o1.Id > _o1.Id) break;
			if (connections[i].o1.Id == _o1.Id && connections[i].o2.Id == _o2.Id) return connections[i];
		}
		return null;
	}

	public bool UpdateCards(TangibleObject[] objects) {
		bool newConnection = false;
		List<TangibleObject> candidates = new List<TangibleObject>(objects);
		candidates.Sort(CompareId);
		List<Connection> newConnections = new List<Connection>();
		for (int i = 0; i < candidates.Count; i++) {
			for (int j = i + 1; j < candidates.Count; j++) {
				if (!EventHelper.IsPair(candidates[i], candidates[j])) continue;

				Connection c = FindConnection(candidates[i], candidates[j]);
				if (c == null) {
					newConnection = true;
					c = new Connection(candidates[i], candidates[j]);
				}
				newConnections.Add(c);
			}
		}
		connections = newConnections;
		return newConnection;
	}
}

}