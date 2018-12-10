// declared by the main page, set by razor
declare const petid: number;
declare const petimageid: number;
declare const endurance: number;
declare const strength: number;

// rate that race button can be pressed 0-1000ms
const minCooldown = 0;
const maxCooldown = 3000;

// range of possible values for endurance
const minEndurance = 0;
const maxEndurance = 100;
const minStrength = 0;
const maxStrength = 100;

// determine the cooldown length for the endurance value, constrained to be between 
// the worst and the best rate
const enduranceRate = constrain(map(endurance, 0, 100, maxCooldown, minCooldown), maxCooldown, minCooldown);;

// percentages of the total distance to move with each keypress
const minDistance = 0.05;
const maxDistance = 0.3;

// how far the pet will run all the time
const minNormalRunSpeed = 0.01;
const maxNormalRunSpeed = 0.1;

// how far across the total distance to move
const moveDistance = constrain(map(strength, minStrength, maxStrength, minDistance, maxDistance), minDistance, maxDistance);
// how far the pet will run in a second
const runSpeed = constrain(map(strength, minStrength, maxStrength, minNormalRunSpeed, maxNormalRunSpeed), minNormalRunSpeed, maxNormalRunSpeed);

var raceButton = document.getElementById("race-button");
var startButton = document.getElementById("start-race") as HTMLButtonElement;

var beforerace = document.getElementById("before-race") as HTMLDivElement;
var duringrace = document.getElementById("during-race") as HTMLDivElement; 
var afterRace = document.getElementById("after-race") as HTMLDivElement;

var finalScore = document.getElementById("final-score") as HTMLSpanElement;

var cooldownActive = false;

var gameActive = false;

raceButton.addEventListener('click', function () {
    press();
});

function press() {
    if (gameActive) {
        if (!cooldownActive) {
            moveRight();
            startCount();
            cooldown();
        }
        // already in cooldown
        else {
            errorShake();
        }
    }
}

var shake = true;
function errorShake() {
    if (shake) {
        shake = false;
        raceButton.classList.add("shaker");
        setTimeout(function () {
            raceButton.classList.remove("shaker");
            shake = true;
        }, 300);
    }
}

var myGamePiece: any = null;
// hey if you are looking to hack the game, you should change this variable :^)
var distance = 0;

function startGame() {
    myGamePiece = new PlayerObject(250, 250, `/api/image/${petimageid}`, 10, 120);
    myGameArea.start();
    distance = 0;
    gameActive = true;
}

var myGameArea = {
    canvas: document.getElementById("race-canvas"),
    start: function () {
        this.context = this.canvas.getContext("2d");
        this.interval = setInterval(updateGameArea, updateRate);
    },
    clear: function () {
        this.context.clearRect(0, 0, this.canvas.width, this.canvas.height);
    }
}

function getTotalDistance(): number {
    // the total distance, minus the size of the image, and some padding
    return (myGameArea.canvas as HTMLCanvasElement).width - 150 - 50;
}

class PlayerObject {
    width: number;
    height: number;
    x: number;
    y: number;
    image: any;
    ctx: any;
    constructor(width: number, height: number, imagesrc: string, x: any, y: any) {
        this.width = width;
        this.height = height;
        this.image = new Image();
        this.image.src = imagesrc;
        this.x = x;
        this.y = y;
        this.ctx = null;
    }
    update() {
        updateRunning();
        this.x = distance * getTotalDistance();
        // draw the image on the canvas
        this.ctx = (myGameArea.canvas as HTMLCanvasElement).getContext("2d");
        this.ctx.drawImage(this.image, this.x, this.y, this.width, this.height);

        // draw the current time
        this.ctx.font = "30px Arial";
        this.ctx.fillText(`Time: ${seconds}`, 10, 50);
    }
}

function updateGameArea() {
    myGameArea.clear();
    myGamePiece.update();

    // check if distance >1
    if (distance > 1) {
        stopCount();
        // placeholer
        win();
    }
}

function win()
{
    setCooldown(true);
    if (gameActive)
    {
        gameActive = false;
        // determine the score
        var score = Math.floor(map(seconds, 0, 60, 1000000, 10));
        if (score < 0)
            score = 0;
        // set the final score
        finalScore.innerText = `${score}`;

        // post the score
        // this is very insecure, but I don't really care atm
        const to = `/api/pet/${petid}/race/${score}`;
        var request = new XMLHttpRequest();
        request.open('POST', to);
        request.addEventListener('load', function (ev: ProgressEvent) {
            if (this.status >= 300) {
                console.log(this);
                console.log(ev);
                alert('got a non 200 status, uh oh.');
            }

        })
        request.send();

        // show the results text
        afterRace.style.display = "block";
    }
}


var seconds = 0;
var timer : number;
var timer_is_on : boolean = false;

function timedCount() {
    // would have preferred to use a DateTime here instead
    seconds++;
    timer = setTimeout(timedCount, 1000);
}

function startCount() {
    if (!timer_is_on) {
        timer_is_on = true;
        timedCount();
    }
}

function stopCount() {
    clearTimeout(timer);
    timer_is_on = false;
}


// constrains x to the range [min-max], inclusive
function constrain(x: number, min: number, max: number) : number {
    if (x < min) return min;
    if (x > max) return max;
    return x;
}

// equivalent to the arduino method
// https://www.arduino.cc/reference/en/language/functions/math/map/
function map(x: number, in_min: number, in_max: number, out_min: number, out_max: number) : number {
    return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}

// sets the state of the cooldown, and the buttons affected by it
function setCooldown(active: boolean) {
    cooldownActive = active;
    (raceButton as HTMLButtonElement).disabled = active;
}

// cooldown for button after each press, based off of pet endurance
function cooldown() {
    var btn = raceButton;
    // disable button for enduranceRate length
    setCooldown(true);
    setTimeout(function () {
        setCooldown(false);
    }, enduranceRate);
}

const updateRate = 60;

function updateRunning() {
    if (gameActive) {
        distance += runSpeed / updateRate;
    }
}

function moveRight() {
    if (gameActive) {
        distance += moveDistance;
    }
}
// prevent the user from being able to hold the spacebar down
var spaceDown = false;
document.addEventListener('keydown', function (ev: KeyboardEvent) {
    if (ev.key == " ") {
        if (spaceDown) return;
        spaceDown = true;
        press();
    }
}, false);

document.addEventListener('keyup', function (ev: KeyboardEvent) {
    if (ev.key == " ") {
        spaceDown = false;
    }
}, false);

// button for starting the game, that hides itself when clicked
startButton.addEventListener('click', function () {
    countdown();
    startButton.disabled = true;
}, false);

var countdowntext = document.getElementById("countdowntext") as HTMLHeadingElement;

var startingCountdown = false;
function countdown() {
    if (startingCountdown) return;
    startingCountdown = true;
    countdowntext.style.display = "block";
    countdowntext.innerHTML = `Race begins in 3...`;
    setTimeout(function () {
        countdowntext.innerHTML = `Race begins in 2...`;
    }, 1000);
    setTimeout(function () {
        countdowntext.innerHTML = `Race begins in 1...`;
    }, 2000);
    setTimeout(function () {
        actuallyStart();
    }, 3000);
}


function actuallyStart() {
    startGame();
    duringrace.style.display = "block";
    beforerace.style.display = "none";
}
 
