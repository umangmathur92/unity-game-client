﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ChatPanel : MonoBehaviour {
    // Use this for initialization
    Button btnSend;
    ChatMessage messagePrefab;
    RectTransform messages;
    RectTransform scrollRect;
    RectTransform rectTransform;
    bool allowEnter;
    InputField txtMessage;

    private void Start () {
        // initialize some variables.
        btnSend = this.transform.Find("BtnSend").GetComponent<Button>();
        txtMessage = this.transform.Find("TxtInput").GetComponent<InputField>();
        messagePrefab = this.transform.Find("MessagePrefab").GetComponent<ChatMessage>();
        scrollRect = this.transform.Find("ScrollContent").GetComponent<RectTransform>();
        messages = scrollRect.Find("Messages").GetComponent<RectTransform>();

        // get the height from the parent, and then make the chatpanel the same height.
       // RectTransform parentTransform = this.transform.parent.gameObject.GetComponent<RectTransform>();
       // RectTransform transform = this.GetComponent<RectTransform>();
       // transform.sizeDelta = new Vector2(transform.sizeDelta.x, parentTransform.sizeDelta.y);
        txtMessage.ActivateInputField();
        btnSend.onClick.AddListener(sendChatMessage);
    }

    void Awake() {
      try {
        Game.networkManager.Listen(
          NetworkCode.CHAT,
          onChatMessage
        );
      } catch (Exception ex) {
      }
    }

    public void onChatMessage(NetworkResponse response) {
      var args = response as ResponseChat;
      var time = DateTime.Now;
      // clone the prefab message
      var clone = (ChatMessage)Instantiate(messagePrefab);

      // append the new message to the scrollable
      clone.GetComponent<RectTransform>().SetParent(messages, false);

      // set the text
      clone.setText(args.message);
      // set the time and the sender
      clone.setTime(time.ToString("h:mm:ss tt"));
      clone.setUsername(args.username);

      // scroll to the bottom.
      scrollRect.GetComponent<ScrollRect>().velocity=new Vector2(0f,1000f);
  }

    void sendChatMessage() {
        Game.networkManager.Send(ChatProtocol.Prepare (GameState.player.GetName(), this.txtMessage.text));
        this.txtMessage.text = "";
    }

    // Update is called once per frame
    void Update () {
        // check if the enter key is pressed, if it is and the focused control is the input field, we'll send the chat message.
        if (allowEnter && (txtMessage.text.Length > 0) && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))) {
          sendChatMessage();
          allowEnter = false;
        } else {
          allowEnter = txtMessage.isFocused;
        }
     }
}
