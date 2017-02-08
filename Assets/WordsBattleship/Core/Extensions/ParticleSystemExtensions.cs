using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tangible.WordsBattleship {
    public static class ParticleSystemExtensions {
        public static void SetEmissionEnabled(this ParticleSystem particleSystem, bool enabled) {
            ParticleSystem.EmissionModule em = particleSystem.emission;
            em.enabled = enabled;
        }

        public static void SetRate(this ParticleSystem particleSystem, ParticleSystem.MinMaxCurve rate) {
            ParticleSystem.EmissionModule em = particleSystem.emission;
            em.rate = rate;
        }
    }
}
