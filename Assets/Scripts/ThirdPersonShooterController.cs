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

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animator;
    private GameObject crosshairGameObject;

    private string crosshairTag = "Crosshair";

    private void Awake() {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
        crosshairGameObject = GameObject.FindWithTag(crosshairTag);
        crosshairGameObject.gameObject.SetActive(false);
        //TODO: aimCamera starts off with being set to true. Not sure why. This is a temporary fix
        aimVirtualCamera.gameObject.SetActive(false);
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
            mouseWorldPosition = raycastHit.point;
        }

        if (starterAssetsInputs.active) {
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            // no cross hairs or shooting while sprinting
            if (starterAssetsInputs.sprint) {
                crosshairGameObject.gameObject.SetActive(false);
                if (!starterAssetsInputs.aim) {
                    // reverting
                    changeThirdPersonControls(false, normalSensitivity);
                }
            } else {
                limitThirdPersonControlHandler(mouseWorldPosition);
                crosshairGameObject.gameObject.SetActive(true);
                if (starterAssetsInputs.aim) {
                    crosshairGameObject.gameObject.SetActive(true);
                    changeThirdPersonControls(true, aimSensitivity);
                } else {
                    // reverting
                    changeThirdPersonControls(false, normalSensitivity);
                }

                if (starterAssetsInputs.shoot) {
                    Vector3 aimDirection = (mouseWorldPosition - bulletSpawnPosition.position).normalized;
                    Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                    starterAssetsInputs.shoot = false;
                }
            }
        } else {
            // reverting
            changeThirdPersonControls(false, normalSensitivity);
            crosshairGameObject.gameObject.SetActive(false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }
    }

    private void limitThirdPersonControlHandler(Vector3 mouseWorldPosition) {
        thirdPersonController.SetRotateOnMove(false);

        Vector3 worldAimTarget = mouseWorldPosition;
        // y-direction as we are doing only sideways rotation
        worldAimTarget.y = transform.position.y;
        Vector3 aiDirection = (worldAimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aiDirection, Time.deltaTime * 20f);
    }

    private void changeThirdPersonControls(bool aimState, float sensitivity) {
        aimVirtualCamera.gameObject.SetActive(aimState);
        thirdPersonController.SetRotateOnMove(!aimState);
        thirdPersonController.SetSensitivity(sensitivity);
    }
}
