using UnityEngine;
using System.Collections;

public class ParticleDelay : MonoBehaviour {

    public IEnumerator PlayParticle(float delay, float destroyTime = 1) {
        yield return new WaitForSeconds(delay);

        ParticleSystem system = GetComponent<ParticleSystem>();
        if (system != null) {
            system.Play();
        }

        if (destroyTime > 0) {
            StartCoroutine(DestroyAfterDone(destroyTime));
        }
    }

    private IEnumerator DestroyAfterDone(float time) {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
        StopAllCoroutines();
    }
}
