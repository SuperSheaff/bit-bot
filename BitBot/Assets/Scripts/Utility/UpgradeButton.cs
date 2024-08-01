// using TMPro;
// using UnityEngine;

// // Enum to define different upgrade types
// public enum UpgradeType
// {
//     Jump,
//     Dash,
//     DoubleJump,
//     // Add more upgrade types as needed
// }

// public class UpgradeButton : MonoBehaviour
// {
//     public PlayerController playerController; // Reference to the PlayerController script
//     public LayerMask playerLayer; // Layer to detect the player
//     public UpgradeType upgradeType; // The type of upgrade this button provides
//     public TMP_Text PressJump;

//     // Triggered when another collider enters the trigger collider attached to this object
//     void OnTriggerEnter(Collider other)
//     {
//         if (IsPlayer(other))
//         {
//             ApplyUpgrade();
//         }

//         StartCoroutine(CameraController.instance.FadeInText(PressJump, 1f));
//     }

//     // Checks if the collider belongs to the player
//     private bool IsPlayer(Collider other)
//     {
//         return ((1 << other.gameObject.layer) & playerLayer) != 0;
//     }

//     // Applies the upgrade based on the upgrade type
//     private void ApplyUpgrade()
//     {
//         switch (upgradeType)
//         {
//             case UpgradeType.Jump:
//                 playerController.EnableJump();
//                 break;

//             case UpgradeType.Dash:
//                 // playerController.EnableDash();
//                 break;

//             case UpgradeType.DoubleJump:
//                 // playerController.EnableDoubleJump();
//                 break;

//             // Add more cases for other upgrade types
//         }

//     }
// }