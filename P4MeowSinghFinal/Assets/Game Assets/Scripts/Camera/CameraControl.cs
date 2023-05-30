



// Re-wrote and re-commented the script, reference used from the tank tutorial.





using UnityEngine;

namespace Complete
{
    public class CameraControl : MonoBehaviour
    {
        public float m_DampTime = 0.2f;                 // Time taken for the camera to adjust smoothly.
        public float m_ScreenEdgeBuffer = 4f;           // Space between the top/bottom target and screen edge.
        public float m_MinSize = 6.5f;                  // The smallest size the camera can be.
        [HideInInspector] public Transform[] m_Targets; // All targets the camera needs to focus on.

        private Camera m_Camera;                        // Reference to the camera.
        private float m_ZoomSpeed;                      // Speed for smoothly changing the size.
        private Vector3 m_MoveVelocity;                 // Velocity for smooth camera movement.
        private Vector3 m_DesiredPosition;              // The position the camera is moving towards.

        private void Awake ()
        {
            m_Camera = GetComponentInChildren<Camera> ();
        }

        private void FixedUpdate ()
        {
            // Move the camera to a desired position.
            Move ();

            // Adjust the camera size.
            Zoom ();
        }

        private void Move ()
        {
            // Calculate the average position of the targets.
            FindAveragePosition ();

            // Move the camera smoothly towards the desired position.
            transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
        }

        private void FindAveragePosition ()
        {
            Vector3 averagePos = new Vector3 ();
            int numTargets = 0;

            // Calculate the sum of positions of all active targets.
            for (int i = 0; i < m_Targets.Length; i++)
            {
                // Skip inactive targets.
                if (!m_Targets[i].gameObject.activeSelf)
                    continue;

                // Add the position to the average and increment the target count.
                averagePos += m_Targets[i].position;
                numTargets++;
            }

            // Calculate the average position.
            if (numTargets > 0)
                averagePos /= numTargets;

            // Maintain the same y value.
            averagePos.y = transform.position.y;

            // Set the desired position to the calculated average position.
            m_DesiredPosition = averagePos;
        }

        private void Zoom ()
        {
            // Calculate the required size based on the desired position and smoothly adjust the camera size.
            float requiredSize = FindRequiredSize();
            m_Camera.orthographicSize = Mathf.SmoothDamp (m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
        }

        private float FindRequiredSize ()
        {
            // Calculate the desired position in local space.
            Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

            // Initialize the camera size calculation.
            float size = 0f;

            // Iterate through all the targets...
            for (int i = 0; i < m_Targets.Length; i++)
            {
                // ... and skip inactive targets.
                if (!m_Targets[i].gameObject.activeSelf)
                    continue;

                // Find the position of the target in local space.
                Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

                // Calculate the distance between the target and desired position.
                Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

                // Determine the maximum distance vertically from the camera.
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

                // Determine the maximum distance horizontally from the camera.
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
            }

            // Add the edge buffer to the size.
            size += m_ScreenEdgeBuffer;

            // Ensure the camera size is not below the minimum.
            size = Mathf.Max (size, m_MinSize);

            return size;
        }

        public void SetStartPositionAndSize ()
        {
            // Calculate the desired position.
            FindAveragePosition ();

            // Set the camera's position to the desired position without smoothing.
            transform.position = m_DesiredPosition;

            // Calculate and set the required camera size.
            m_Camera.orthographicSize = FindRequiredSize ();
        }
    }
}