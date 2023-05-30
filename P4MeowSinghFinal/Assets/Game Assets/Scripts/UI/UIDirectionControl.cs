


// Re-wrote and re-commented the script, reference used from tank tutorial.



using UnityEngine;

namespace Complete
{
    public class UIDirectionControl : MonoBehaviour
    {
        // This script ensures that world space UI elements, such as the health bar, face the correct direction.

        public bool m_UseRelativeRotation = true;       // Should this gameobject use relative rotation?

        private Quaternion m_RelativeRotation;          // The initial local rotation of the object.

        private void Start ()
        {
            // Store the local rotation at the beginning of the scene.
            m_RelativeRotation = transform.parent.localRotation;
        }

        private void Update ()
        {
            // If relative rotation should be used...
            if (m_UseRelativeRotation)
            {
                // Set the rotation to the stored relative rotation.
                transform.rotation = m_RelativeRotation;
            }
        }
    }
}
