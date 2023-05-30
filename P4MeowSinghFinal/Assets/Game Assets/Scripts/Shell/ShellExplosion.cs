

// Re-wrote and re-commented the script, reference used from tank tutorial.



using UnityEngine;

namespace Complete
{
    public class ShellExplosion : MonoBehaviour
    {
        public LayerMask m_TankMask;                        // Defines the target objects affected by the explosion, specifically set to "Players".
        public ParticleSystem m_ExplosionParticles;         // Reference to the explosion particle system.
        public AudioSource m_ExplosionAudio;                // Reference to the explosion audio source.
        public float m_MaxDamage = 100f;                    // Maximum damage caused by a centered explosion.
        public float m_ExplosionForce = 1000f;              // Amount of force applied to tanks at the center of the explosion.
        public float m_MaxLifeTime = 2f;                    // Time in seconds before the shell is removed.
        public float m_ExplosionRadius = 5f;                // Maximum distance from the explosion center where tanks are affected.

        private void Start ()
        {
            // Destroy the shell after its lifetime if not destroyed earlier.
            Destroy (gameObject, m_MaxLifeTime);
        }

        private void OnTriggerEnter (Collider other)
        {
            // Collect all colliders within the explosion radius from the shell's current position.
            Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);

            // Iterate through all colliders...
            for (int i = 0; i < colliders.Length; i++)
            {
                // ... and find the associated rigidbody.
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();

                // If the collider doesn't have a rigidbody, move on to the next collider.
                if (!targetRigidbody)
                    continue;

                // Apply explosion force.
                targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

                // Find the TankHealth script associated with the rigidbody.
                TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> ();

                // If the gameobject doesn't have a TankHealth script, move on to the next collider.
                if (!targetHealth)
                    continue;

                // Calculate the damage based on the distance from the shell.
                float damage = CalculateDamage (targetRigidbody.position);

                // Inflict damage on the tank.
                targetHealth.TakeDamage (damage);
            }

            // Detach the particles from the shell.
            m_ExplosionParticles.transform.parent = null;

            // Play the particle system.
            m_ExplosionParticles.Play();

            // Play the explosion sound effect.
            m_ExplosionAudio.Play();

            // Destroy the particle system gameobject after it finishes playing.
            ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
            Destroy (m_ExplosionParticles.gameObject, mainModule.duration);

            // Destroy the shell.
            Destroy (gameObject);
        }

        private float CalculateDamage (Vector3 targetPosition)
        {
            // Calculate the vector from the shell to the target.
            Vector3 explosionToTarget = targetPosition - transform.position;

            // Calculate the distance between the shell and the target.
            float explosionDistance = explosionToTarget.magnitude;

            // Calculate the relative distance as a proportion of the maximum distance (explosionRadius).
            float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

            // Calculate the damage as a proportion of the maximum possible damage.
            float damage = relativeDistance * m_MaxDamage;

            // Ensure the minimum damage is always 0.
            damage = Mathf.Max (0f, damage);

            return damage;
        }
    }
}
