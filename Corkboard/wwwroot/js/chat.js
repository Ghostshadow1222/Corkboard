"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("send-btn").disabled = true;

// Store channelId globally so send button can use it
let currentChannelId = null;

// Start the connection.
async function start() {
    const container = document.getElementById("chat-container");
    if (!container) {
        console.error("Chat container not found.");
        return;
    }

    // Parse as int (your hub expects int channelId)
    const channelIdStr = container.getAttribute("data-channel-id");
    if (!channelIdStr) {
        console.error('data-channel-id not found on #chat-container');
        return;
    }
    currentChannelId = parseInt(channelIdStr, 10);
    if (isNaN(currentChannelId)) {
        console.error('data-channel-id is not a valid number:', channelIdStr);
        return;
    }

    await connection.start();
    await connection.invoke("JoinChannel", currentChannelId);
}
start().then(function () {
    document.getElementById("send-btn").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

// Receive message from the hub
connection.on("ReceiveMessage", function (messageDto) {
    var messagesDiv = document.getElementById("messages");
    var p = document.createElement("p");
    messagesDiv.appendChild(p);
    
    // SignalR / ASP.NET Core serializes to camelCase by default on the client
    p.innerHTML = "<strong>" + messageDto.senderUsername + "</strong>: " + messageDto.text;
});

// Send message to the hub
document.getElementById("send-btn").addEventListener("click", function (event) {
    const messageText = document.getElementById("message-input").value;
    if (!messageText.trim()) return;

    // Hub expects: SendMessage(int channelId, string messageContent)
    connection.invoke("SendMessage", currentChannelId, messageText).catch(function (err) {
        return console.error(err.toString());
    });
    
    // Clear input after sending
    document.getElementById("message-input").value = "";
    event.preventDefault();
});