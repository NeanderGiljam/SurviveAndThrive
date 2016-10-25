using UnityEngine;
using System.Collections;

public static class FXHelper {

    /// <summary>
    /// Used to instantiate a self-destructing particle, make sure prefab is in the "Visuals/Particles/" folder!
    /// destroyAfter = 0 --> don't destroy
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="?"></param>
    public static GameObject CreateParticle(string prefabName, Vector3 location, float destroyAfter = 1, float delayTime = 0) {
        GameObject prefab = (GameObject)Resources.Load("Visuals/Particles/" + prefabName);
        GameObject particle = (GameObject)GameObject.Instantiate(prefab, location, Quaternion.identity);
        ParticleDelay delay = particle.AddComponent<ParticleDelay>();
        delay.StartCoroutine(delay.PlayParticle(delayTime, destroyAfter));
        return particle;
    }
}