var d = document;

// set up refs to buttons and spans in the page
var petHunger = d.getElementById("pet-hunger");
var petHappy = d.getElementById("pet-happiness");
var petStrength = d.getElementById("pet-strength");
var petEndurance = d.getElementById("pet-endurance");

var timeUntil = d.getElementById("time-until");
var timeoutAlert = d.getElementById("alert-timeout");

// buttons
var btnFeed = d.getElementById("btn-feed");
var btnTrain = d.getElementById("btn-train");
// race button will redirect to the race page, so a reference to that does
// not have to be hooked up for onclick handling
// but still needs to be able to be disabled
var btnRace = d.getElementById("btn-race");

// api path to hit to check for an updated status
const statusApiPath = `/api/Pet/${petid}/status`;
const activityApiPath = `/api/Pet/${petid}/Activity`;

// represents a pet, but also derived attributes
// like happiness, hunger, and when it can perform actions next
interface PetStatus {
    pet: any;
    happiness: number;
    hunger: number;
    timeOfNextAction: string;
    serverTime: string;
}

function SendActivityType(type: string) {
    var request = new XMLHttpRequest();
    request.addEventListener('load', (ev: ProgressEvent) => {
        // despite the results of this, just call the update method
        // when there is a callback from the feed button
        UpdatePetStatus();
    });
    request.open('POST', `${activityApiPath}/${type[0]}`);
    request.setRequestHeader('Content-Type', 'application/json');
    request.send();
}

// sets up the buttons on this page
function SetupButtons() {
    var feed = (btnFeed as HTMLButtonElement);
    feed.onclick = (ev) => {
        // send the Feed activity
        EnableButtons(false);
        SendActivityType('f');
    };

    var train = (btnTrain as HTMLButtonElement);
    // choo choo
    train.onclick = (ev) => {
        // send the Train activity
        EnableButtons(false);
        SendActivityType('t');
    };
}

// the current status of the pet
// on initial load, this will be null and defer to using the values that are generated
// by Razor pages
var currentPetStatus = null;
var nextTime: Date = null;

// hits the statusApiPath to get the updated state of the pet, as a PetStatus
// which will be set in the request callback
function UpdatePetStatus() {
    var request = new XMLHttpRequest();
    request.addEventListener('load', PetStatusCallback);
    request.open('GET', statusApiPath);
    request.setRequestHeader('Content-Type', 'application/json');
    // authorization.. shouldn't? be an issue?
    // honestly not sure
    request.send();
}

function PetStatusCallback(this: XMLHttpRequest, e: ProgressEvent)
{
    // status is 2xx
    if (this.status >= 200 && this.status < 300) {
        // convert the response into PetStatus
        let jsonObj: any = JSON.parse(this.response);
        currentPetStatus = <PetStatus>jsonObj;
        UpdateView(currentPetStatus);
    }
}

// updates the state of the view with the results from hitting the api
function UpdateView(status: PetStatus)
{
    // ignore when status is null
    if (status == null) {
        return;
    }

    var time = new Date(status.serverTime);
    var next = new Date(status.timeOfNextAction);

    var serverTimeS = Math.floor((time.getTime() + time.getTimezoneOffset() * 60 * 1000) / 1000);
    // next time does not have timezone, so skip it
    var nextTimeS = Math.floor((next.getTime()) / 1000);

    var canPerformActions = serverTimeS >= nextTimeS;
    var pet = status.pet;
    nextTime = next;
    
    // update the alert box
    UpdateAlert(next, canPerformActions);
    EnableButtons(canPerformActions);
    UpdateText(status);
}

function UpdateText(status: PetStatus) {
    var hunger = (petHunger as HTMLSpanElement);
    var happy = (petHappy as HTMLSpanElement);
    var stren = (petStrength as HTMLSpanElement);
    var endu = (petEndurance as HTMLSpanElement);

    var s = status.pet.strength;
    var e = status.pet.endurance;

    // hunger and happy are doubles (UGH javascript floats and their crappy precision)
    hunger.innerHTML = `${Math.floor(status.hunger * 100)}`;
    happy.innerHTML = `${Math.floor(status.happiness * 100)}`;
    stren.innerHTML = `${s}`;
    endu.innerHTML = `${e}`;

    // TODO apply styles to happy and hunger depending on the range of values they are at
    // shouldn't be too hard.
}

// enables or disables the buttons
function EnableButtons(canPerformActions: boolean)
{
    var feed = (btnFeed as HTMLButtonElement);
    feed.disabled = !canPerformActions;

    var train = (btnTrain as HTMLButtonElement);
    train.disabled = !canPerformActions;

    // btn-race is actually an anchor tag
    var race = (btnRace as HTMLAnchorElement);
    // manipulate the classes applied to this
    if (canPerformActions) {
        race.classList.remove("disabled");
    }
    else {
        race.classList.add("disabled");
    }
}

// we do not handle timezones here, or inaccurate clients
//TODO: Have the server send it's time as a value in PetStatus, so that this cannot be tricked as easily
function UpdateAlert(timeRequired: Date, canPerformActions: boolean)
{
    // show or hide the alert
    // based on if actions can be performed right now or not
    var a = (timeoutAlert as HTMLDivElement);
    a.style.display = !canPerformActions ? "block" : "none";

    UpdateTime(canPerformActions);
}

function UpdateTime(canPerformActions: boolean) {
    if (nextTime == null || canPerformActions)
        return;

    var now: number = Date.now() + (new Date().getTimezoneOffset() * 60 * 1000);
    if (true || Math.floor(nextTime.getTime() / 1000) >= Math.floor(now / 1000)) {
        var t = (timeUntil as HTMLSpanElement);

        var mseconds: number = nextTime.getTime() - now;
        var seconds = Math.floor(mseconds / 1000);
        var min = Math.floor(seconds / 60);
        var sec = Math.floor(seconds % 60);
        t.innerHTML = `${min} min. ${sec} sec.`;
    }
}

// these will be set up on the page load
setInterval(UpdateTime, 1000);
setInterval(UpdatePetStatus, 5000);
// call this on load, because it also performs actions like setting up data
UpdatePetStatus();
SetupButtons();