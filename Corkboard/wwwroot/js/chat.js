"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("send-btn").disabled = true;

// Store channelId globally so send button can use it
let currentChannelId = null;

// Track the oldest message timestamp for pagination
let oldestMessageTimestamp = null;

// Flag to prevent multiple simultaneous load requests
let isLoadingMessages = false;

// Flag to track if there are more messages to load
let hasMoreMessages = true;

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

    try {
        await connection.start();
        await connection.invoke("JoinChannel", currentChannelId);
        document.getElementById("send-btn").disabled = false;
    } catch (err) {
        console.error(err.toString());
        alert("Failed to connect to chat. Please refresh the page.");
    }
}
start().then(async function () {
    document.getElementById("send-btn").disabled = false;
    
    // Scroll to bottom when page loads
    var messagesDiv = document.getElementById("messages");
    if (messagesDiv) {
        messagesDiv.scrollTop = messagesDiv.scrollHeight;
        
        // Initialize oldest message timestamp from existing messages
        initializeOldestTimestamp();
        
        // Add scroll event listener for infinite scroll
        await new Promise(resolve => setTimeout(resolve, 500));
        messagesDiv.addEventListener("scroll", handleScroll);
    }
}).catch(function (err) {
    return console.error(err.toString());
});

// Receive message from the hub
connection.on("ReceiveMessage", function (messageDto) {
    var messagesDiv = document.getElementById("messages");
    var p = document.createElement("p");
    p.setAttribute("data-timestamp", messageDto.timestamp);
    messagesDiv.appendChild(p);
    
    // SignalR / ASP.NET Core serializes to camelCase by default on the client
    p.innerHTML = "<strong>" + messageDto.senderUsername + "</strong>: " + messageDto.text;
    
    // Auto-scroll to bottom when new message arrives
    messagesDiv.scrollTop = messagesDiv.scrollHeight;
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

// Send message on Enter key press
document.getElementById("message-input").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        document.getElementById("send-btn").click();
    }
});

// Initialize the oldest message timestamp from server-rendered messages
function initializeOldestTimestamp() {
    var messagesDiv = document.getElementById("messages");
    var firstMessage = messagesDiv.querySelector("p[data-timestamp]");
    if (firstMessage) {
        oldestMessageTimestamp = firstMessage.getAttribute("data-timestamp");
    }
}

// Handle scroll event for infinite scroll
function handleScroll() {
    var messagesDiv = document.getElementById("messages");
    
    // Check if user scrolled near the top (within 100px)
    if (messagesDiv.scrollTop < 100 && !isLoadingMessages && hasMoreMessages) {
        loadMoreMessages();
    }
}

// Load more (older) messages
async function loadMoreMessages() {
    if (!oldestMessageTimestamp || isLoadingMessages || !hasMoreMessages) {
        return;
    }
    
    isLoadingMessages = true;
    var messagesDiv = document.getElementById("messages");
    
    // Store current scroll height to maintain scroll position
    var oldScrollHeight = messagesDiv.scrollHeight;
    
    try {
        // Call SignalR hub method
        var olderMessages = await connection.invoke("LoadMoreMessages", currentChannelId, oldestMessageTimestamp);
        
        if (olderMessages && olderMessages.length > 0) {
            // Prepend messages to the top
            olderMessages.forEach(function(messageDto) {
                var p = document.createElement("p");
                p.setAttribute("data-timestamp", messageDto.timestamp);
                p.innerHTML = "<strong>" + messageDto.senderUsername + "</strong>: " + messageDto.text;
                messagesDiv.insertBefore(p, messagesDiv.firstChild);
            });
            
            // Update oldest timestamp
            oldestMessageTimestamp = olderMessages[olderMessages.length - 1].timestamp;
            
            // Restore scroll position (maintain user's view)
            var newScrollHeight = messagesDiv.scrollHeight;
            messagesDiv.scrollTop = newScrollHeight - oldScrollHeight;
        } else {
            // No more messages to load
            hasMoreMessages = false;
        }
    } catch (err) {
        console.error("Failed to load more messages:", err);
    } finally {
        isLoadingMessages = false;
    }
}