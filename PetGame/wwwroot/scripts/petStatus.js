"use strict";
var d = document;
var petHunger = d.getElementById("pet-hunger");
var petHappy = d.getElementById("pet-happiness");
var petStrength = d.getElementById("pet-strength");
var petEndurance = d.getElementById("pet-endurance");
var timeUntil = d.getElementById("time-until");
var timeoutAlert = d.getElementById("alert-timeout");
var btnFeed = d.getElementById("btn-feed");
var btnTrain = d.getElementById("btn-train");
var btnRace = d.getElementById("btn-race");
var statusApiPath = "/api/Pet/" + petid + "/status";
var activityApiPath = "/api/Pet/" + petid + "/Activity";
function SendActivityType(type) {
    var request = new XMLHttpRequest();
    request.addEventListener('load', function (ev) {
        UpdatePetStatus();
    });
    request.open('POST', activityApiPath + "/" + type[0]);
    request.setRequestHeader('Content-Type', 'application/json');
    request.send();
}
function SetupButtons() {
    var feed = btnFeed;
    feed.onclick = function (ev) {
        EnableButtons(false);
        SendActivityType('f');
    };
    var train = btnTrain;
    train.onclick = function (ev) {
        EnableButtons(false);
        SendActivityType('t');
    };
}
var currentPetStatus = null;
var nextTime = null;
function UpdatePetStatus() {
    var request = new XMLHttpRequest();
    request.addEventListener('load', PetStatusCallback);
    request.open('GET', statusApiPath);
    request.setRequestHeader('Content-Type', 'application/json');
    request.send();
}
function PetStatusCallback(e) {
    if (this.status >= 200 && this.status < 300) {
        var jsonObj = JSON.parse(this.response);
        currentPetStatus = jsonObj;
        UpdateView(currentPetStatus);
    }
}
function UpdateView(status) {
    if (status == null) {
        return;
    }
    var time = new Date(status.serverTime);
    var next = new Date(status.timeOfNextAction);
    var canPerformActions = time.getTime() >= next.getTime();
    var pet = status.pet;
    nextTime = next;
    UpdateAlert(next, canPerformActions);
    EnableButtons(canPerformActions);
    UpdateText(status);
}
function UpdateText(status) {
    var hunger = petHunger;
    var happy = petHappy;
    var stren = petStrength;
    var endu = petEndurance;
    var s = status.pet.strength;
    var e = status.pet.endurance;
    hunger.innerHTML = "" + Math.floor(status.hunger * 100);
    happy.innerHTML = "" + Math.floor(status.happiness * 100);
    stren.innerHTML = "" + s;
    endu.innerHTML = "" + e;
}
function EnableButtons(canPerformActions) {
    var feed = btnFeed;
    feed.disabled = !canPerformActions;
    var train = btnTrain;
    train.disabled = !canPerformActions;
    var race = btnRace;
    if (canPerformActions) {
        race.classList.remove("disabled");
    }
    else {
        race.classList.add("disabled");
    }
}
function UpdateAlert(timeRequired, canPerformActions) {
    var a = timeoutAlert;
    a.style.display = !canPerformActions ? "block" : "none";
    UpdateTime();
}
function UpdateTime() {
    if (nextTime == null)
        return;
    var now = new Date();
    if (nextTime > now) {
        var t = timeUntil;
        var mseconds = nextTime.getTime() - now.getTime();
        var seconds = Math.floor(mseconds / 1000);
        var min = Math.floor(seconds / 60);
        var sec = Math.floor(seconds % 60);
        t.innerHTML = min + " min. " + sec + " sec.";
    }
}
setInterval(UpdateTime, 1000);
setInterval(UpdatePetStatus, 5000);
UpdatePetStatus();
SetupButtons();
//# sourceMappingURL=petStatus.js.map