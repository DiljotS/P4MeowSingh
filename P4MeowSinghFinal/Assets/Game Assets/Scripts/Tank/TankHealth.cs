



// Re-wrote and re-commented the script, reference used from tank tutorial.


using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankHealth : MonoBehaviour
    {
        public float m_StartingHealth = 100f;               // Initial health of each tank.
        public Slider m_Slider;                             // Slider representing the tank's current health.
        public Image m_FillImage;                           // Image component of the slider.
        public Color m_FullHealthColor = Color.green;       // Color of the health bar when at full health.
        public Color m_ZeroHealthColor = Color.red;         // Color of the health bar when at zero health.
        public GameObject m_ExplosionPrefab;                // Prefab instantiated in Awake and used for tank destruction.
        
        
        private AudioSource m_ExplosionAudio;               // Audio source for tank explosions.
        private ParticleSystem m_ExplosionParticles;        // Particle system played when the tank is destroyed.
        private float m_CurrentHealth;                      // Current health of the tank.
        private bool m_Dead;                                // Flag indicating if the tank is destroyed.


        private void Awake ()
        {
            // Instantiate the explosion prefab and get a reference to its particle system.
            m_ExplosionParticles = Instantiate (m_ExplosionPrefab).GetComponent<ParticleSystem> ();

            // Get the audio source from the instantiated prefab.
            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource> ();

            // Disable the prefab so it can be activated when needed.
            m_ExplosionParticles.gameObject.SetActive (false);
        }


        private void OnEnable()
        {
            // When the tank is enabled, reset its health and the "dead" flag.
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;

            // Update the health slider's value and color.
            SetHealthUI();
        }


        public void TakeDamage (float amount)
        {
            // Reduce the current health by the damage amount.
            m_CurrentHealth -= amount;

            // Update the UI elements accordingly.
            SetHealthUI ();

            // If the current health is at or below zero and the tank is not already considered dead, call OnDeath.
            if (m_CurrentHealth <= 0f && !m_Dead)
            {
                OnDeath ();
            }
        }


        private void SetHealthUI ()
        {
            // Set the slider's value to reflect the current health.
            m_Slider.value = m_CurrentHealth;

            // Interpolate the color of the fill image based on the current health percentage relative to the starting health.
            m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        }


        private void OnDeath ()
        {
            // Set the "dead" flag to prevent multiple calls to this function.
            m_Dead = true;

            // Position the instantiated explosion prefab at the tank's position and activate it.
            m_ExplosionParticles.transform.position = transform.position;
            m_ExplosionParticles.gameObject.SetActive (true);

            // Play the particle system for tank explosion.
            m_ExplosionParticles.Play ();

            // Play the sound effect for tank explosion.
            m_ExplosionAudio.Play();

            // Disable the tank object.
            gameObject.SetActive (false);
        }
    }
}
