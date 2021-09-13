using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class ThirdPersonShooterController : MonoBehaviour {
    
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    /* Need a different sensitivity for Camera rotation because of the zoom in */
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderMask = new LayerMask();
    [SerializeField] private Transform bulletPrefab;
    [SerializeField] private Transform bulletSpawnPosition;
    // [SerializeField] private Transform debugTransform;

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animator;

    private void Awake() {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
    }

    private void Update() {
        Vector3 mouseWorldPosition = Vector3.zero;

        /* 
        Mouse could be used to get the co-ods instead of screenCenterPoint 
        but it would break if Mouse is not being used 
        */
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderMask)) {
            // debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }
        
        if (starterAssetsInputs.aim) {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            Vector3 worldAimTarget = mouseWorldPosition;
            // y-direction as we are doing only sideways rotation
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        } else {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
       }

        if (starterAssetsInputs.shoot) {
            //TODO: Instantiate at the mouth of the gun
            Vector3 aimDirection = (mouseWorldPosition - bulletSpawnPosition.position).normalized;
            Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
            starterAssetsInputs.shoot = false;
        }
    }
}
