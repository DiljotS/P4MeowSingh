


// Re-wrote and re-commented the script, reference used from tank tutorial.



using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankShooting : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // Identifies the specific player.
        public Rigidbody m_Shell;                   // Shell prefab.
        public Transform m_FireTransform;           // Transform where the shells are spawned.
        public Slider m_AimSlider;                  // Slider displaying the current launch force.
        public AudioSource m_ShootingAudio;         // Audio source for shooting sounds.
        public AudioClip m_ChargingClip;            // Audio played when charging a shot.
        public AudioClip m_FireClip;                // Audio played when firing a shot.
        public float m_MinLaunchForce = 15f;        // Minimum force applied to the shell if fire button is not held.
        public float m_MaxLaunchForce = 30f;        // Maximum force applied to the shell if fire button is held for max charge time.
        public float m_MaxChargeTime = 0.75f;       // Maximum time the shell can be charged before firing at max force.

        private string m_FireButton;                // Input axis for launching shells.
        private float m_CurrentLaunchForce;         // Force applied to the shell when fire button is released.
        private float m_ChargeSpeed;                // Rate at which launch force increases based on max charge time.
        private bool m_Fired;                       // Whether or not the shell has been launched with this button press.

        private void OnEnable()
        {
            // Reset launch force and UI when the tank is turned on.
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }

        private void Start ()
        {
            // Define the fire axis based on the player number.
            m_FireButton = "Fire" + m_PlayerNumber;

            // Calculate the charge speed based on the range of possible forces and max charge time.
            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        }

        private void Update ()
        {
            // Set the slider's default value to the minimum launch force.
            m_AimSlider.value = m_MinLaunchForce;

            // If the max force has been reached and the shell hasn't been fired yet...
            if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
            {
                // Use the max force and fire the shell.
                m_CurrentLaunchForce = m_MaxLaunchForce;
                Fire();
            }
            // Otherwise, if the fire button has just been pressed...
            else if (Input.GetButtonDown(m_FireButton))
            {
                // Reset the fired flag and launch force.
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;

                // Change to the charging clip and start playing it.
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }
            // Otherwise, if the fire button is being held and the shell hasn't been fired yet...
            else if (Input.GetButton(m_FireButton) && !m_Fired)
            {
                // Increase the launch force and update the slider.
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
                m_AimSlider.value = m_CurrentLaunchForce;
            }
            // Otherwise, if the fire button is released and the shell hasn't been fired yet...
            else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
            {
                // Fire the shell.
                Fire();
            }
        }

        private void Fire ()
        {
            // Set the fired flag to prevent multiple calls to Fire.
            m_Fired = true;

            // Create an instance of the shell and get a reference to its rigidbody.
            Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; 

            // Change to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();

            // Reset the launch force as a precaution in case of missing button events.
            m_CurrentLaunchForce = m_MinLaunchForce;
        }
    }
}
