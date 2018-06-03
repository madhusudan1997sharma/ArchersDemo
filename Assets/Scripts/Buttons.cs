using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;
using System.Net;
using System.IO;
using System;

public class Buttons : MonoBehaviour {

    // OFFLINE ASSETS
    public Canvas offlineGameplayCanvas, offlineGameplayMenuCanvas;
    public Text offlineGameplayEventText;
    public GameObject offlineLeftRagdollHead, offlineRightRagdollHead;

    // ONLINE ASSETS
    public Canvas onlineGameplayCanvas, onlineGameplayMenuCanvas;
    public Text onlineGameplayEventText;
    public GameObject onlineLeftRagdollHead, onlineRightRagdollHead;
    bool offlineGameover = false, onlineGameover = false;

    public Text internetConnection;
    public string active = "";

    public static string onlineMySide;

    // Use this for initialization
    void Start()
    {
        PlayGamesPlatform.Activate();
        
        new Thread(new ThreadStart(new CheckForInternet(this).isInternetAvailable)).Start();

        // authenticate user:
        Social.localUser.Authenticate((bool success) => { });
    }

    private void Update()
    {
        try
        {
            if (active == "inactive")
                internetConnection.GetComponent<Text>().text = "No Internet Connection";
            else if (active == "active")
                internetConnection.GetComponent<Text>().text = "Active Internet Connection";
            else if(active == "")
                internetConnection.GetComponent<Text>().text = "Checking...";
        } catch (System.Exception e) { }

        try
        {
            // OFFLINE GAMEPLAY RESULTS
            if (!offlineGameover && offlineLeftRagdollHead.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
            {
                offlineGameplayEventText.GetComponent<Text>().text = "Player Two Won!";
                offlineGameplayMenuCanvas.GetComponent<Canvas>().enabled = true;
                offlineGameover = true;
            }
            else if (!offlineGameover && offlineRightRagdollHead.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
            {
                offlineGameplayEventText.GetComponent<Text>().text = "Player One Won!";
                offlineGameplayMenuCanvas.GetComponent<Canvas>().enabled = true;
                offlineGameover = true;
            }
        } catch (System.Exception e) { }

        try
        {
            // ONLINE GAMEPLAY RESULTS
            if (!onlineGameover && onlineLeftRagdollHead.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic && onlineMySide == "left")
            {
                OnlinePlayer.outsideScriptControl("leaveroom");
                onlineGameplayEventText.GetComponent<Text>().text = "Opponent Won!";
                onlineGameplayMenuCanvas.GetComponent<Canvas>().enabled = true;
                onlineGameover = true;
            }
            else if (!onlineGameover && onlineLeftRagdollHead.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic && onlineMySide == "right")
            {
                OnlinePlayer.outsideScriptControl("leaveroom");
                onlineGameplayEventText.GetComponent<Text>().text = "You Won!";
                onlineGameplayMenuCanvas.GetComponent<Canvas>().enabled = true;
                onlineGameover = true;
            }
            if (!onlineGameover && onlineRightRagdollHead.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic && onlineMySide == "left")
            {
                OnlinePlayer.outsideScriptControl("leaveroom");
                onlineGameplayEventText.GetComponent<Text>().text = "You Won!";
                onlineGameplayMenuCanvas.GetComponent<Canvas>().enabled = true;
                onlineGameover = true;
            }
            else if (!onlineGameover && onlineRightRagdollHead.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic && onlineMySide == "right")
            {
                OnlinePlayer.outsideScriptControl("leaveroom");
                onlineGameplayEventText.GetComponent<Text>().text = "Opponent Won!";
                onlineGameplayMenuCanvas.GetComponent<Canvas>().enabled = true;
                onlineGameover = true;
            }
        } catch (System.Exception e) { }
    }

    public void OfflineMultiplayerClick()
    {
        SceneManager.LoadScene("offlinegameplay");
    }

    public void OnlineMultiplayerClick()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
            SceneManager.LoadScene("onlineselection");
        else
        {
            Social.localUser.Authenticate((bool success) => {
                if(success)
                    SceneManager.LoadScene("onlineselection");
            });
        }
    }

    public void OnlineSelectionRandomOpponentClick()
    {
        OnlinePlayer.multiplayerType = "random";
        SceneManager.LoadScene("onlinegameplay");
    }

    public void OnlineSelectionSendInvitationClick()
    {
        OnlinePlayer.multiplayerType = "sendInvitation";
        SceneManager.LoadScene("onlinegameplay");
    }

    public void OnlineSelectionReceiveInvitationClick()
    {
        OnlinePlayer.multiplayerType = "receiveInvitation";
        SceneManager.LoadScene("onlinegameplay");
    }

    public void OfflineGameplayMenuClick()
    {
        offlineGameplayEventText.GetComponent<Text>().text = "Game Paused";
        offlineGameplayMenuCanvas.GetComponent<Canvas>().enabled = true;
    }

    public void OnlineGameplayMenuClick()
    {
        onlineGameplayEventText.GetComponent<Text>().text = "";
        onlineGameplayMenuCanvas.GetComponent<Canvas>().enabled = true;
    }

    public void GameplayHomeClick()
    {
        SceneManager.LoadScene("mainmenu");
    }

    public void OfflineGameplayReplayClick()
    {
        SceneManager.LoadScene("offlinegameplay");
    }

    public void OfflineGameplayPausedMenuCanvasClick()
    {
        if(offlineGameplayEventText.GetComponent<Text>().text == "Game Paused")
            offlineGameplayMenuCanvas.GetComponent<Canvas>().enabled = false;
    }

    public void OnlineGameplayPausedMenuCanvasClick()
    {
        if (onlineGameplayEventText.GetComponent<Text>().text == "")
            onlineGameplayMenuCanvas.GetComponent<Canvas>().enabled = false;
    }
}

public class CheckForInternet
{
    Buttons b;
    public CheckForInternet(Buttons b)
    {
        this.b = b;
    }
    public void isInternetAvailable()
    {
        string html = "";
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.microscopy-uk.org.uk/mag/wimsmall/x_smal1.html");
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        char[] cs = new char[2];    // ARRAY TO DOWNLOAD
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch { }
        if (html == "")
            b.active = "inactive";
        else if (html != "")
            b.active = "active";

        Thread.Sleep(1000);
        isInternetAvailable();
    }
}