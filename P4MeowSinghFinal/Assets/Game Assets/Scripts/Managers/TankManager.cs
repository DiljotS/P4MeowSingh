
// Re-wrote and re-commented the script, reference used from tank tutorial.




using System;
using UnityEngine;

namespace Complete
{
    [Serializable]
    public class TankManager
    {
       public Color m_PlayerColor;                             // The designated color that will be applied to this tank.
    public Transform m_SpawnPoint;                          // The position and orientation at which the tank will spawn.
    [HideInInspector] public int m_PlayerNumber;            // Identifies the player associated with this manager.
    [HideInInspector] public string m_ColoredPlayerText;    // A formatted string representing the player with their number and tank color.
    [HideInInspector] public GameObject m_Instance;         // A reference to the instantiated tank object.
    [HideInInspector] public int m_Wins;                    // The number of victories this player has achieved so far.

    private TankMovement m_Movement;                        // Reference to the tank's movement script for control manipulation.
    private TankShooting m_Shooting;                        // Reference to the tank's shooting script for control manipulation.
    private GameObject m_CanvasGameObject;                  // The UI element in the game world associated with the tank.

    public void Setup()
    {
        // Obtain references to the tank's components.
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        // Synchronize the player numbers across the scripts.
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        // Create a formatted player text with the appropriate color, indicating the player's number.
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        // Retrieve all the renderers of the tank.
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        // Iterate through each renderer...
        for (int i = 0; i < renderers.Length; i++)
        {
            // ... assign the specific color to the tank's material.
            renderers[i].material.color = m_PlayerColor;
        }
    }

    // Disable tank control during specific game phases where player control is not allowed.
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;
        m_CanvasGameObject.SetActive(false);
    }

    // Enable tank control during specific game phases where player control is allowed.
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;
        m_CanvasGameObject.SetActive(true);
    }

    // Reset the tank to its default state at the start of each round.
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
}