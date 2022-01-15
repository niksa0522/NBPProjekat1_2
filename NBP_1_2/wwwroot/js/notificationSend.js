"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    window.alert(`${user} says ${message}`);
});

connection.start().then(function () {
    document.getElementById("buttonSend").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("buttonSend").addEventListener("click", function (event) {
    var user = document.getElementById("teamID").value;
    var message = document.getElementById("message").value;
    connection.invoke("MessageGroup", user,message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});