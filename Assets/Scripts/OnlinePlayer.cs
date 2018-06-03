using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine.SocialPlatforms;
using System.Text;
using System;

public class OnlinePlayer : MonoBehaviour, RealTimeMultiplayerListener {

    public GameObject leftRagdollUpperBody, rightRagdollUpperBody, leftInstantiater, rightInstantiater, Arrow;
    public GameObject leftStringRenderer, leftStringRendererStartPos, leftStringRendererMiddlePos, leftStringRendererEndPos;
    public GameObject rightStringRenderer, rightStringRendererStartPos, rightStringRendererMiddlePos, rightStringRendererEndPos;
    public GameObject leftRagdollName, rightRagdollName;
    LineRenderer leftRenderer, rightRenderer;
    public Canvas loadingCanvas, gameplayCanvas;
    public Text debug, loading;
    bool play = false;

    // MULTIPLAYER TYPE
    public static string multiplayerType;

    float f_lastX = 0.0f;
    float f_difX = 0.5f;
    float f_steps = 0.0f;
    int i_direction = 1;

    Vector2 initialTouch, latestTouch;
    String myside;
    float lastTouchDist = 0.0f;
    float force;

    // POOLING TO REDUCE LATENCY
    Touch touch;
    String[] msg;
    string data;
    byte[] message;
    String[] pos;

    // Use this for initialization
    void Start ()
    {
        leftRenderer = leftStringRenderer.GetComponent<LineRenderer>();
        rightRenderer = rightStringRenderer.GetComponent<LineRenderer>();

        if (multiplayerType == "random")
            PlayGamesPlatform.Instance.RealTime.CreateQuickGame(1, 1, 0, this);
        else if (multiplayerType == "sendInvitation")
            PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen(1, 1, 1, this);
        else if (multiplayerType == "receiveInvitation")
        {
            PlayGamesPlatform.Instance.RealTime.AcceptFromInbox(this);
            PlayGamesPlatform.Instance.RealTime.GetInvitation();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        leftRenderer.SetPosition(0, leftStringRendererStartPos.transform.position);
        leftRenderer.SetPosition(1, leftStringRendererMiddlePos.transform.position);
        leftRenderer.SetPosition(2, leftStringRendererEndPos.transform.position);

        rightRenderer.SetPosition(0, rightStringRendererStartPos.transform.position);
        rightRenderer.SetPosition(1, rightStringRendererMiddlePos.transform.position);
        rightRenderer.SetPosition(2, rightStringRendererEndPos.transform.position);

        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            return;

        if (play)
        {
            // MYSIDE IS LEFT
            if(myside == "left")
            {
                // HANDLING THE X-BOW & POWER
                for (int i = 0; i < Input.touchCount; i++)
                {
                    touch = Input.touches[i];
                    if (touch.phase == TouchPhase.Began)
                    {
                        latestTouch = touch.position;
                        initialTouch = touch.position;
                        f_difX = 0.0f;      // X-BOW MOVEMENT
                    }
                    if (touch.phase == TouchPhase.Moved)
                    {
                        latestTouch = touch.position;

                        // X-BOW MOVEMENT
                        f_difX = Mathf.Abs(f_lastX - Input.GetAxis("Mouse Y"));

                        if (f_lastX < Input.GetAxis("Mouse Y"))
                        {
                            if (leftRagdollUpperBody.transform.eulerAngles.z < 60 | leftRagdollUpperBody.transform.eulerAngles.z > 330)
                            {
                                leftRagdollUpperBody.transform.Rotate(Vector3.forward, -f_difX);
                            }
                        }

                        if (f_lastX > Input.GetAxis("Mouse Y"))
                        {
                            if (leftRagdollUpperBody.transform.eulerAngles.z < 30 | leftRagdollUpperBody.transform.eulerAngles.z > 300)
                            {
                                leftRagdollUpperBody.transform.Rotate(Vector3.forward, f_difX);
                            }
                        }


                        float dist = Vector2.Distance(initialTouch, latestTouch) / 9000.0f;
                        if (lastTouchDist < dist && force < 2.5f)
                        {
                            force = Vector2.Distance(initialTouch, latestTouch) / 100;
                            debug.GetComponent<Text>().text = force.ToString();
                            lastTouchDist = dist;
                        }
                        else if (lastTouchDist > dist)
                        {
                            force = Vector2.Distance(initialTouch, latestTouch) / 100;
                            debug.GetComponent<Text>().text = force.ToString();
                        }
                        if (force == 2.5 || force > 2.5)
                            leftStringRendererMiddlePos.transform.localPosition = new Vector3(-1.5f, 0, 0);
                        else if (force < 2.5f && force > 2.25f)
                            leftStringRendererMiddlePos.transform.localPosition = new Vector3(-1.35f, 0, 0);
                        else if (force < 2.25f && force > 2f)
                            leftStringRendererMiddlePos.transform.localPosition = new Vector3(-1.2f, 0, 0);
                        else if (force < 2f && force > 1.75f)
                            leftStringRendererMiddlePos.transform.localPosition = new Vector3(-1.05f, 0, 0);
                        else if (force < 1.75f && force > 1.5f)
                            leftStringRendererMiddlePos.transform.localPosition = new Vector3(-0.9f, 0, 0);
                        else if (force < 1.5f && force > 1.25f)
                            leftStringRendererMiddlePos.transform.localPosition = new Vector3(-0.75f, 0, 0);
                        else if (force < 1.25f && force > 1f)
                            leftStringRendererMiddlePos.transform.localPosition = new Vector3(-0.6f, 0, 0);
                        else if (force < 1f && force > 0.75f)
                            leftStringRendererMiddlePos.transform.localPosition = new Vector3(-0.45f, 0, 0);
                        else if (force < 0.75f && force > 0.5f)
                            leftStringRendererMiddlePos.transform.localPosition = new Vector3(-0.3f, 0, 0);
                        else if (force < 0.5f && force > 0.25f)
                            leftStringRendererMiddlePos.transform.localPosition = new Vector3(-0.15f, 0, 0);
                        else if (force < 0.25f)
                            leftStringRendererMiddlePos.transform.localPosition = new Vector3(0, 0, 0);


                        // SENDING ROTATION DATA
                        data = "rotate|" + leftRagdollUpperBody.transform.rotation.eulerAngles.z + "|" + leftStringRendererMiddlePos.transform.localPosition.x;
                        message = Encoding.ASCII.GetBytes(data);
                        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, message);

                        debug.GetComponent<Text>().text = data;

                        f_lastX = -Input.GetAxis("Mouse Y");
                    }
                    if (touch.phase == TouchPhase.Ended)
                    {
                        // INSTANTIATING ARROW
                        GameObject arrow = Instantiate(Arrow, leftInstantiater.transform.position, leftInstantiater.transform.rotation) as GameObject;

                        // SENDING SHOOTING DATA
                        data = "shoot|" + leftRagdollUpperBody.transform.rotation.eulerAngles.z + "|" + force * 8 + "|" + leftInstantiater.transform.position.x + "?" + leftInstantiater.transform.position.y + "?" + leftInstantiater.transform.position.z;
                        message = Encoding.ASCII.GetBytes(data);
                        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, message);

                        // ADDING FORCE TO ARROW
                        arrow.GetComponent<Rigidbody2D>().velocity = leftInstantiater.transform.right.normalized * force * 8;

                        debug.GetComponent<Text>().text = data;

                        leftRagdollUpperBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                        leftStringRendererMiddlePos.transform.localPosition = new Vector3(0, 0, 0);

                        lastTouchDist = 0.0f;
                        force = 0.0f;
                    }
                }
            }
             
            else if(myside == "right")
            {
                // HANDLING THE X-BOW & POWER
                for (int i = 0; i < Input.touchCount; i++)
                {
                    touch = Input.touches[i];
                    if (touch.phase == TouchPhase.Began)
                    {
                        latestTouch = touch.position;
                        initialTouch = touch.position;
                        f_difX = 0.0f;      // X-BOW MOVEMENT
                    }
                    if (touch.phase == TouchPhase.Moved)
                    {
                        latestTouch = touch.position;

                        // X-BOW MOVEMENT
                        f_difX = Mathf.Abs(f_lastX - Input.GetAxis("Mouse Y"));

                        if (f_lastX < Input.GetAxis("Mouse Y"))
                        {
                            if (rightRagdollUpperBody.transform.eulerAngles.z < 30 | rightRagdollUpperBody.transform.eulerAngles.z > 300)
                            {
                                rightRagdollUpperBody.transform.Rotate(Vector3.forward, f_difX);
                            }
                        }

                        if (f_lastX > Input.GetAxis("Mouse Y"))
                        {
                            if (rightRagdollUpperBody.transform.eulerAngles.z < 60 | rightRagdollUpperBody.transform.eulerAngles.z > 330)
                            {
                                rightRagdollUpperBody.transform.Rotate(Vector3.forward, -f_difX);
                            }
                        }


                        float dist = Vector2.Distance(initialTouch, latestTouch) / 9000.0f;
                        if (lastTouchDist < dist && force < 2.5f)
                        {
                            force = Vector2.Distance(initialTouch, latestTouch) / 100;
                            debug.GetComponent<Text>().text = force.ToString();
                            lastTouchDist = dist;
                        }
                        else if (lastTouchDist > dist)
                        {
                            force = Vector2.Distance(initialTouch, latestTouch) / 100;
                            debug.GetComponent<Text>().text = force.ToString();
                        }
                        if (force == 2.5 || force > 2.5)
                            rightStringRendererMiddlePos.transform.localPosition = new Vector3(-1.5f, 0, 0);
                        else if (force < 2.5f && force > 2.25f)
                            rightStringRendererMiddlePos.transform.localPosition = new Vector3(-1.35f, 0, 0);
                        else if (force < 2.25f && force > 2f)
                            rightStringRendererMiddlePos.transform.localPosition = new Vector3(-1.2f, 0, 0);
                        else if (force < 2f && force > 1.75f)
                            rightStringRendererMiddlePos.transform.localPosition = new Vector3(-1.05f, 0, 0);
                        else if (force < 1.75f && force > 1.5f)
                            rightStringRendererMiddlePos.transform.localPosition = new Vector3(-0.9f, 0, 0);
                        else if (force < 1.5f && force > 1.25f)
                            rightStringRendererMiddlePos.transform.localPosition = new Vector3(-0.75f, 0, 0);
                        else if (force < 1.25f && force > 1f)
                            rightStringRendererMiddlePos.transform.localPosition = new Vector3(-0.6f, 0, 0);
                        else if (force < 1f && force > 0.75f)
                            rightStringRendererMiddlePos.transform.localPosition = new Vector3(-0.45f, 0, 0);
                        else if (force < 0.75f && force > 0.5f)
                            rightStringRendererMiddlePos.transform.localPosition = new Vector3(-0.3f, 0, 0);
                        else if (force < 0.5f && force > 0.25f)
                            rightStringRendererMiddlePos.transform.localPosition = new Vector3(-0.15f, 0, 0);
                        else if (force < 0.25f)
                            rightStringRendererMiddlePos.transform.localPosition = new Vector3(0, 0, 0);

                        // SENDING ROTATION DATA
                        data = "rotate|" + rightRagdollUpperBody.transform.rotation.eulerAngles.z + "|" + rightStringRendererMiddlePos.transform.localPosition.x;
                        message = Encoding.ASCII.GetBytes(data);
                        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, message);

                        debug.GetComponent<Text>().text = data;

                        f_lastX = -Input.GetAxis("Mouse Y");
                    }
                    if (touch.phase == TouchPhase.Ended)
                    {
                        // INSTANTIATING ARROW
                        GameObject arrow = Instantiate(Arrow, rightInstantiater.transform.position, rightInstantiater.transform.rotation) as GameObject;

                        // SENDING SHOOTING DATA
                        data = "shoot|" + rightRagdollUpperBody.transform.rotation.eulerAngles.z + "|" + force * 8 + "|" + rightInstantiater.transform.position.x + "?" + rightInstantiater.transform.position.y + "?" + rightInstantiater.transform.position.z;
                        message = Encoding.ASCII.GetBytes(data);
                        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, message);

                        // ADDING FORCE TO ARROW
                        arrow.GetComponent<Rigidbody2D>().velocity = rightInstantiater.transform.right.normalized * force * 8;
                        
                        debug.GetComponent<Text>().text = data;

                        rightRagdollUpperBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                        rightStringRendererMiddlePos.transform.localPosition = new Vector3(0, 0, 0);

                        lastTouchDist = 0.0f;
                        force = 0.0f;
                    }
                }
            }
        }
    }

    public void OnRoomConnected(bool success)
    {
        if (success)
        {
            if (isServer())
            {
                myside = "left";
                leftRagdollName.GetComponent<TextMesh>().text = "You";
                rightRagdollName.GetComponent<TextMesh>().text = "Opponent";
            }
            else
            {
                myside = "right";
                leftRagdollName.GetComponent<TextMesh>().text = "Opponent";
                rightRagdollName.GetComponent<TextMesh>().text = "You";
            }

            // NOTIFY THE BUTTON SCRIPT ABOUT MY SIDE
            Buttons.onlineMySide = myside;

            // START GAMEPLAY
            loadingCanvas.GetComponent<Canvas>().enabled = false;
            gameplayCanvas.GetComponent<Canvas>().enabled = true;
            play = true;// Room connected
        }
    }

    private bool isServer()
    {
        Participant myself = PlayGamesPlatform.Instance.RealTime.GetSelf();
        foreach (Participant p in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
        {
            if (p.ParticipantId != myself.ParticipantId)
            {
                if (p.ParticipantId.CompareTo(myself.ParticipantId) > 0)
                    return true;
            }
        }
        return false;
    }

    public void OnLeftRoom()
    {

    }

    public void OnParticipantLeft(Participant participant)
    {

    }

    public void OnPeersConnected(string[] participantIds)
    {
        throw new NotImplementedException();
    }

    public void OnPeersDisconnected(string[] participantIds)
    {

    }

    public void OnRoomSetupProgress(float percent)
    {
        if(percent >= 19.0f)
            loading.GetComponent<Text>().text = "Searching for opponent...";
    }

    
    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        msg = Encoding.ASCII.GetString(data).Split('|');

        debug.GetComponent<Text>().text = Encoding.ASCII.GetString(data);

        // shoot | z-rotation | velocity
        if (msg[0] == "shoot")
        {
            if (myside == "left")
            {
                pos = msg[3].Split('?');
                rightRagdollUpperBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, float.Parse(msg[1], System.Globalization.CultureInfo.InvariantCulture)));
                rightInstantiater.transform.position = new Vector3(float.Parse(pos[0], System.Globalization.CultureInfo.InvariantCulture), float.Parse(pos[1], System.Globalization.CultureInfo.InvariantCulture), float.Parse(pos[2], System.Globalization.CultureInfo.InvariantCulture));

                GameObject arrow = Instantiate(Arrow, rightInstantiater.transform.position, rightInstantiater.transform.rotation) as GameObject;
                arrow.GetComponent<Rigidbody2D>().velocity = rightInstantiater.transform.right.normalized * float.Parse(msg[2], System.Globalization.CultureInfo.InvariantCulture);

                rightRagdollUpperBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                rightStringRendererMiddlePos.transform.localPosition = new Vector3(0, 0, 0);
            }
            else if (myside == "right")
            {
                pos = msg[3].Split('?');
                leftRagdollUpperBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, float.Parse(msg[1], System.Globalization.CultureInfo.InvariantCulture)));
                leftInstantiater.transform.position = new Vector3(float.Parse(pos[0], System.Globalization.CultureInfo.InvariantCulture), float.Parse(pos[1], System.Globalization.CultureInfo.InvariantCulture), float.Parse(pos[2], System.Globalization.CultureInfo.InvariantCulture));

                GameObject arrow = Instantiate(Arrow, leftInstantiater.transform.position, leftInstantiater.transform.rotation) as GameObject;
                arrow.GetComponent<Rigidbody2D>().velocity = leftInstantiater.transform.right.normalized * float.Parse(msg[2], System.Globalization.CultureInfo.InvariantCulture);

                leftRagdollUpperBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                leftStringRendererMiddlePos.transform.localPosition = new Vector3(0, 0, 0);
            }
        }

        // rotate | z-rotation
        else if (msg[0] == "rotate")
        {
            if (myside == "left")
            {
                rightRagdollUpperBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, float.Parse(msg[1], System.Globalization.CultureInfo.InvariantCulture)));
                rightStringRendererMiddlePos.transform.localPosition = new Vector3(float.Parse(msg[2], System.Globalization.CultureInfo.InvariantCulture), 0, 0);
            }
            else if (myside == "right")
            {
                leftRagdollUpperBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, float.Parse(msg[1], System.Globalization.CultureInfo.InvariantCulture)));
                leftStringRendererMiddlePos.transform.localPosition = new Vector3(float.Parse(msg[2], System.Globalization.CultureInfo.InvariantCulture), 0, 0);
            }
        }
    }


    public static void outsideScriptControl(String task)
    {
        if (task == "leaveroom")
            PlayGamesPlatform.Instance.RealTime.LeaveRoom();
    }
}
