using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShot : MonoBehaviour
{   [Header("Rubber Band")]
    [SerializeField] private LineRenderer rubber;
    [SerializeField] private Transform firstpoint;
    [SerializeField] private Transform secondPoint;

    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    void Awake()
    {
        rubber.positionCount = 3;
        rubber.SetPosition(0, firstpoint.position);
        rubber.SetPosition(1, transform.position);
        rubber.SetPosition(2, secondPoint.position);
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
   void OnMouseEnter()
   {
       launchPoint.SetActive(true);
   }
   void OnMouseExit()
   {
       launchPoint.SetActive(false);
   }
   void OnMouseDown()
   {
       aimingMode = true;
       projectile = Instantiate(projectilePrefab) as GameObject;
       projectile.transform.position = launchPos;
       projectile.GetComponent<Rigidbody>().isKinematic = true;
   }
   void Update()
   {
       if (!aimingMode) {
        rubber.SetPosition(1, launchPos);
        return;
       }
       Vector3 mousePos2D = Input.mousePosition;
       mousePos2D.z = -Camera.main.transform.position.z;
       Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);
       Vector3 mouseDelta = mousePos3D - launchPos;
       float maxMagnitude = this.GetComponent<SphereCollider>().radius;
       if (mouseDelta.magnitude > maxMagnitude)
       {
           mouseDelta.Normalize();
           mouseDelta *= maxMagnitude;
       }
       Vector3 projPos = launchPos + mouseDelta;
       projectile.transform.position = projPos;
       rubber.SetPosition(1, projPos);
       if (Input.GetMouseButtonUp(0))
       {
           aimingMode = false;
           Rigidbody projRB = projectile.GetComponent<Rigidbody>();
           projRB.isKinematic = false;
           projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
           projRB.linearVelocity = -mouseDelta * velocityMult;
           FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
           FollowCam.POI = projectile;
           if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
           Instantiate<GameObject>(projLinePrefab, projectile.transform);
           projectile = null;
           MissionDemolition.SHOT_FIRED();
       }
   }
}
