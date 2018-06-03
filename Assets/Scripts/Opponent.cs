using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Opponent : MonoBehaviour
{

    public GameObject Instantiater, Arrow, UpperBody, StringRenderer, StringRendererStartPos, StringRendererMiddlePos, StringRendererEndPos;
    LineRenderer stringRenderer;

    float f_lastX = 0.0f;
    float f_difX = 0.5f;
    float f_steps = 0.0f;
    int i_direction = 1;

    Vector2 initialTouch, latestTouch;
    int fingerId = -1;
    float lastTouchDist = 0.0f;
    float force;

    // Use this for initialization
    void Start()
    {
        stringRenderer = StringRenderer.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        stringRenderer.SetPosition(0, StringRendererStartPos.transform.position);
        stringRenderer.SetPosition(1, StringRendererMiddlePos.transform.position);
        stringRenderer.SetPosition(2, StringRendererEndPos.transform.position);

        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            return;

        // HANDLING THE X-BOW & POWER
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.touches[i];
            if (touch.phase == TouchPhase.Began && touch.position.x > Screen.width / 2 && fingerId == -1)
            {
                fingerId = touch.fingerId;
                latestTouch = touch.position;
                initialTouch = touch.position;
                f_difX = 0.0f;      // X-BOW MOVEMENT
            }
            if (touch.phase == TouchPhase.Moved && touch.fingerId == fingerId)
            {
                latestTouch = touch.position;
                
                // X-BOW MOVEMENT
                f_difX = Mathf.Abs(f_lastX - Input.GetAxis("Mouse Y"));

                if (f_lastX < Input.GetAxis("Mouse Y"))
                {
                    if (UpperBody.transform.eulerAngles.z < 30 | UpperBody.transform.eulerAngles.z > 300)
                    {
                        UpperBody.transform.Rotate(Vector3.forward, f_difX);
                    }
                }

                if (f_lastX > Input.GetAxis("Mouse Y"))
                {
                    if (UpperBody.transform.eulerAngles.z < 60 | UpperBody.transform.eulerAngles.z > 330)
                    {
                        UpperBody.transform.Rotate(Vector3.forward, -f_difX);
                    }
                }

                float dist = Vector2.Distance(initialTouch, latestTouch) / 9000.0f;
                if (lastTouchDist < dist && force < 2.5f)
                {
                    force = Vector2.Distance(initialTouch, latestTouch) / 100;
                    lastTouchDist = dist;
                }
                else if (lastTouchDist > dist)
                {
                    force = Vector2.Distance(initialTouch, latestTouch) / 100;
                }
                if (force == 2.5 || force > 2.5)
                    StringRendererMiddlePos.transform.localPosition = new Vector3(-1.5f, 0, 0);
                else if (force < 2.5f && force > 2.25f)
                    StringRendererMiddlePos.transform.localPosition = new Vector3(-1.35f, 0, 0);
                else if (force < 2.25f && force > 2f)
                    StringRendererMiddlePos.transform.localPosition = new Vector3(-1.2f, 0, 0);
                else if (force < 2f && force > 1.75f)
                    StringRendererMiddlePos.transform.localPosition = new Vector3(-1.05f, 0, 0);
                else if (force < 1.75f && force > 1.5f)
                    StringRendererMiddlePos.transform.localPosition = new Vector3(-0.9f, 0, 0);
                else if (force < 1.5f && force > 1.25f)
                    StringRendererMiddlePos.transform.localPosition = new Vector3(-0.75f, 0, 0);
                else if (force < 1.25f && force > 1f)
                    StringRendererMiddlePos.transform.localPosition = new Vector3(-0.6f, 0, 0);
                else if (force < 1f && force > 0.75f)
                    StringRendererMiddlePos.transform.localPosition = new Vector3(-0.45f, 0, 0);
                else if (force < 0.75f && force > 0.5f)
                    StringRendererMiddlePos.transform.localPosition = new Vector3(-0.3f, 0, 0);
                else if (force < 0.5f && force > 0.25f)
                    StringRendererMiddlePos.transform.localPosition = new Vector3(-0.15f, 0, 0);
                else if (force < 0.25f)
                    StringRendererMiddlePos.transform.localPosition = new Vector3(0, 0, 0);

                f_lastX = -Input.GetAxis("Mouse Y");
            }
            if (touch.phase == TouchPhase.Ended && touch.fingerId == fingerId)
            {
                // SHOOTING ARROWS
                GameObject arrow = Instantiate(Arrow, Instantiater.transform.position, Quaternion.Euler(new Vector3(0, -180, transform.eulerAngles.z))) as GameObject;
                arrow.GetComponent<Rigidbody2D>().velocity = Instantiater.transform.right.normalized * force * 8;

                UpperBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                StringRendererMiddlePos.transform.localPosition = new Vector3(0, 0, 0);

                fingerId = -1;
                lastTouchDist = 0.0f;
                force = 0.0f;
            }
        }
    }
}