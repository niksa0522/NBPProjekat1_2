"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (message) {
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    window.alert(`${message}`);
});
connection.start();
document.getElementById("subscribeButton").addEventListener("click", function (event) {
    var user = document.getElementById("teamID").value;
    connection.invoke("AddToGroup", user).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
document.getElementById("unsubscribeButton").addEventListener("click", function (event) {
    var user = document.getElementById("teamID").value;
    connection.invoke("RemoveFromGroup", user).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});