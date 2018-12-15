"use strict";
var minCooldown = 0;
var maxCooldown = 3000;
var minEndurance = 0;
var maxEndurance = 100;
var minStrength = 0;
var maxStrength = 100;
var enduranceRate = constrain(map(endurance, 0, 100, maxCooldown, minCooldown), maxCooldown, minCooldown);
;
var minDistance = 0.05;
var maxDistance = 0.3;
var minNormalRunSpeed = 0.01;
var maxNormalRunSpeed = 0.1;
var moveDistance = constrain(map(strength, minStrength, maxStrength, minDistance, maxDistance), minDistance, maxDistance);
var runSpeed = constrain(map(strength, minStrength, maxStrength, minNormalRunSpeed, maxNormalRunSpeed), minNormalRunSpeed, maxNormalRunSpeed);
var raceButton = document.getElementById("race-button");
var startButton = document.getElementById("start-race");
var beforerace = document.getElementById("before-race");
var duringrace = document.getElementById("during-race");
var afterRace = document.getElementById("after-race");
var finalScore = document.getElementById("final-score");
var cooldownActive = false;
var gameActive = false;
raceButton.addEventListener('click', function () {
    press();
});
function press() {
    if (gameActive) {
        if (!cooldownActive) {
            moveRight();
            cooldown();
        }
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
var myGamePiece = null;
var distance = 0;
function startGame() {
    myGamePiece = new PlayerObject(250, 250, "race_files/4.png", 10, 120);
    myGameArea.start();
    distance = 0;
    gameActive = true;
    startGameTimer();
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
};
function getTotalDistance() {
    return myGameArea.canvas.width - 150 - 50;
}
var PlayerObject = (function () {
    function PlayerObject(width, height, imagesrc, x, y) {
        this.width = width;
        this.height = height;
        this.image = new Image();
        this.image.src = imagesrc;
        this.x = x;
        this.y = y;
        this.ctx = null;
    }
    PlayerObject.prototype.update = function () {
        updateRunning();
        this.x = distance * getTotalDistance();
        this.ctx = myGameArea.canvas.getContext("2d");
        this.ctx.drawImage(this.image, this.x, this.y, this.width, this.height);
        this.ctx.font = "30px Arial";
        this.ctx.fillText("Time: " + getSeconds(), 10, 50);
    };
    return PlayerObject;
}());
function getMSeconds() {
    if (timeStart == null)
        return 0;
    if (timerIsOn) {
        return (new Date().getTime() - timeStart.getTime());
    }
    else {
        if (timeStop == null)
            return;
        return (timeStop.getTime() - timeStart.getTime());
    }
}
function getSeconds() {
    return Math.floor(getMSeconds() / 1000);
}
function updateGameArea() {
    myGameArea.clear();
    myGamePiece.update();
    if (distance > 1) {
        win();
    }
}
function win() {
    setCooldown(true);
    if (gameActive) {
        stopCount();
        gameActive = false;
        var score = 1 + Math.floor(map(getMSeconds(), 0, 120000, 1000000, 10));
        if (score < 1)
            score = 1;
        finalScore.innerText = "" + score;
        var to = "/api/pet/" + petid + "/race/" + score;
        afterRace.style.display = "block";
        duringrace.style.display = "none";
    }
}
var timer;
var timerIsOn = false;
var timeStart;
var timeStop;
function startGameTimer() {
    if (!timerIsOn) {
        timerIsOn = true;
        timeStart = new Date();
    }
}
function stopCount() {
    clearTimeout(timer);
    timerIsOn = false;
    timeStop = new Date();
}
function constrain(x, min, max) {
    if (x < min)
        return min;
    if (x > max)
        return max;
    return x;
}
function map(x, in_min, in_max, out_min, out_max) {
    return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}
function setCooldown(active) {
    cooldownActive = active;
    raceButton.disabled = active;
}
function cooldown() {
    var btn = raceButton;
    setCooldown(true);
    setTimeout(function () {
        setCooldown(false);
    }, enduranceRate);
}
var updateRate = 60;
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
var spaceDown = false;
document.addEventListener('keydown', function (ev) {
    if (ev.key == " ") {
        if (spaceDown)
            return;
        spaceDown = true;
        press();
    }
}, false);
document.addEventListener('keyup', function (ev) {
    if (ev.key == " ") {
        spaceDown = false;
    }
}, false);
startButton.addEventListener('click', function () {
    countdown();
    startButton.disabled = true;
}, false);
var countdowntext = document.getElementById("countdowntext");
var startingCountdown = false;
function countdown() {
    if (startingCountdown)
        return;
    startingCountdown = true;
    countdowntext.style.display = "block";
    countdowntext.innerHTML = "Race begins in 3...";
    setTimeout(function () {
        countdowntext.innerHTML = "Race begins in 2...";
    }, 1000);
    setTimeout(function () {
        countdowntext.innerHTML = "Race begins in 1...";
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
