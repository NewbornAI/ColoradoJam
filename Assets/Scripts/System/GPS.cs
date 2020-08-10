using System.Collections;
using System.Collections.Generic;
using coloradoJam;
using UnityEngine;

public class GPS : MonoBehaviour
{
    public List<Sun> suns = new List<Sun>();
    public Vector3 gravityCenter = Vector3.zero;

    public Camera cam;
    public Planet terra;

    public Transform camLookAt;
    protected float camSpeed = 500;
    protected float camRotationSpeed = 1;
    protected float watchDistance = 500;

    // Start is called before the first frame update
    void Start()
    {
        if (suns.Count > 0)
            gravityCenter = suns[0].transform.position;

        CamLook(terra.transform);
    }

    public float camLookTimer = 0;

    // Update is called once per frame
    void Update()
    {
        camLookTimer += Time.deltaTime;

        if (camLookAt)
        {
            Vector3 direction = new Vector3(camLookAt.position.x, watchDistance ,camLookAt.position.z) - cam.transform.position;

            float distance = direction.magnitude;
            if (distance > 0) // Move towards target
            {
                float speed = Mathf.Max(camSpeed * Mathf.Pow(camLookTimer,2) ,camSpeed * 0.5f);
                if (distance < 50)
                    speed = camSpeed * 0.5f;

                Vector3 translation = direction.normalized * speed * Time.deltaTime;
                float maxMagnitude = direction.magnitude;
                if (translation.magnitude > maxMagnitude)
                    translation = Vector3.Scale(translation.normalized, new Vector3(maxMagnitude, maxMagnitude, maxMagnitude));

                cam.transform.Translate(translation,Space.World);
            }

            if (camLookTimer > Statics.CAM_LOOK_TIME)
                CamLook(terra.transform);
        }
    }


    public Vector3 GlobalPos(Transform body)
    {
        return body.position - gravityCenter;
    }

    public void ShipArriving(Fleet fleet)
    {
        if (camLookTimer >= Statics.CAM_LOOK_TIME || camLookAt == terra.transform)
        {
            CamLook(fleet.destination.transform);
        }
    }

    public void CamLook(Transform target)
    {
        cam.transform.parent = target;
        camLookAt = target;
        if (target.localScale.x < 30)
            watchDistance = Mathf.Min(-50, -30 * camLookAt.localScale.x);
        else
            watchDistance = Mathf.Max(-700, -10 * camLookAt.localScale.x);
        camLookTimer = 0;

    }

}
