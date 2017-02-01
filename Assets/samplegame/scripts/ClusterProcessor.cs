using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tangible {

	public abstract class ClusterProcessor : MonoBehaviour {

		// This function is called on vision platform events, at most once a frame.
		public virtual void UpdateClusters(List<List<ClusterHelper.Card>> clusters) {
		}

		protected List<ClusterHelper.Card> CardsWithId (List<List<ClusterHelper.Card>> clusters, int id) {
			List<ClusterHelper.Card> matchingCards = new List<ClusterHelper.Card> ();
			if (clusters != null) {
				int numClusters = clusters.Count;
				for (int i = 0; i < numClusters; i++) {
					List<ClusterHelper.Card> cards = clusters [i];
					int numCards = cards.Count;
					for (int j = 0; j < numCards; j++) {
						if (cards [j].id == id) {
							matchingCards.Add (cards [j]);
						}
					}
				}
			}
			return matchingCards;
		}

		protected bool ContainsUniqueId(List<List<ClusterHelper.Card>> clusters, int unique_id) {
			if (clusters != null) {
				int numClusters = clusters.Count;
				for (int i = 0; i < numClusters; i++) {
					List<ClusterHelper.Card> cards = clusters [i];
					int numCards = cards.Count;
					for (int j = 0; j < numCards; j++) {
						if (cards [j].unique_id == unique_id) {
							return true;
						}
					}
				}
			}
			return false;
		}
	}
		
}
