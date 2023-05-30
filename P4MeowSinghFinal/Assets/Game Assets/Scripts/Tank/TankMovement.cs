


// Re-wrote and re-commented the script, reference used from tank tutorial.



using UnityEngine;

namespace Complete
{
    public class TankMovement : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // Identifies the tank's player number assigned by the manager.
        public float m_Speed = 12f;                 // Movement speed of the tank forwards and backwards.
        public float m_TurnSpeed = 180f;            // Turning speed of the tank in degrees per second.
        public AudioSource m_MovementAudio;         // Audio source for engine sounds (different from shooting audio source).
        public AudioClip m_EngineIdling;            // Audio played when the tank is not moving.
        public AudioClip m_EngineDriving;           // Audio played when the tank is moving.
		public float m_PitchRange = 0.2f;           // Variation range for engine pitch.

        private string m_MovementAxisName;          // Input axis name for forward and backward movement.
        private string m_TurnAxisName;              // Input axis name for turning.
        private Rigidbody m_Rigidbody;              // Reference to move the tank.
        private float m_MovementInputValue;         // Current value of movement input.
        private float m_TurnInputValue;             // Current value of turn input.
        private float m_OriginalPitch;              // Starting pitch of the audio source.
        private ParticleSystem[] m_particleSystems; // References to particle systems used by the tank.

        private void Awake ()
        {
            m_Rigidbody = GetComponent<Rigidbody> ();
        }


        private void OnEnable ()
        {
            // Activate the tank and make sure it's not kinematic.
            m_Rigidbody.isKinematic = false;

            // Reset input values.
            m_MovementInputValue = 0f;
            m_TurnInputValue = 0f;

            // Retrieve all the particle systems attached to the tank for stopping/playing during activation/deactivation.
            // This prevents undesired smoke trails when the tank is spawned and moving.
            m_particleSystems = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particleSystem in m_particleSystems)
            {
                particleSystem.Play();
            }
        }


        private void OnDisable ()
        {
            // Deactivate the tank and set it to kinematic to stop movement.
            m_Rigidbody.isKinematic = true;

            // Stop all particle systems to reset their positions instead of assuming movement during spawning.
            foreach (ParticleSystem particleSystem in m_particleSystems)
            {
                particleSystem.Stop();
            }
        }


        private void Start ()
        {
            // Set input axis names based on the player number.
            m_MovementAxisName = "Vertical" + m_PlayerNumber;
            m_TurnAxisName = "Horizontal" + m_PlayerNumber;

            // Store the original pitch of the audio source.
            m_OriginalPitch = m_MovementAudio.pitch;
        }


        private void Update ()
        {
            // Retrieve the input values for movement and turning.
            m_MovementInputValue = Input.GetAxis (m_MovementAxisName);
            m_TurnInputValue = Input.GetAxis (m_TurnAxisName);

            // Manage engine audio.
            EngineAudio ();
        }


        private void EngineAudio ()
        {
            // If there is no input (tank is stationary)...
            if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
            {
                // ... and if the audio source is currently playing the driving clip...
                if (m_MovementAudio.clip == m_EngineDriving)
                {
                    // ... change the clip to idling and play it.
                    m_MovementAudio.clip = m_EngineIdling;
                    m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play ();
                }
            }
            else
            {
                // Otherwise, if the tank is moving and the idling clip is currently playing...
                if (m_MovementAudio.clip == m_EngineIdling)
                {
                    // ... change the clip to driving and play it.
                    m_MovementAudio.clip = m_EngineDriving;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
                }
            }
        }


        private void FixedUpdate ()
        {
            // Adjust the position and orientation of the rigidbody in FixedUpdate.
            Move ();
            Turn ();
        }


        private void Move ()
        {
            // Create a movement vector in the tank's forward direction based on input, speed, and frame time.
            Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

            // Apply this movement to the rigidbody's position.
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }


        private void Turn ()
        {
            // Calculate the rotation amount based on input, turn speed, and frame time.
            float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

            // Convert the rotation amount into a rotation in the y-axis.
            Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

            // Apply this rotation to the rigidbody's rotation.
            m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
        }
    }
}
