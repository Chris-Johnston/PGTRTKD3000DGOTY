// declared by the main page, set by razor
declare const petid: number;
declare const petimageid: number;
declare const endurance: number;
declare const strength: number;

var button = document.getElementById("race-button");

button.addEventListener('click', function () {
    moveRight();
    startCount();
    cooldown();
});

var myGamePiece: any = null;

function startGame() {
    myGamePiece = component(20, 20, `/api/image/${petimageid}`, 10, 120);
    myGameArea.start();
}

var myGameArea = {
    canvas: document.getElementById("race-canvas"),
    start: function () {
        this.context = this.canvas.getContext("2d");
        this.interval = setInterval(updateGameArea, 60);
    },
    clear: function () {
        this.context.clearRect(0, 0, this.canvas.width, this.canvas.height);
    }
}

function component(width: number, height: number, color: any, x: any, y: any) {
    this.width = width;
    this.height = height;
    this.speedX = 0;
    this.speedY = 0;
    this.x = x;
    this.y = y;
    var ctx = null;
    this.update = function () {
        ctx = (myGameArea.canvas as HTMLCanvasElement).getContext("2d");
        ctx.fillStyle = color;
        ctx.fillRect(this.x, this.y, this.width, this.height);

        // checks if gamePiece crosses finish line
        if (this.x >= (myGameArea.canvas as HTMLCanvasElement).width) {
            ctx.fillStyle = "#1828D3";
            ctx.fillRect(this.x, this.y, this.width, this.height);
            clearInterval(60);
            stopCount();
            var score = Math.floor(1000 / seconds);

            // assign score value from seconds
            if (seconds <= 0) {
                score = 1000;
            }
            else {
                score = Math.floor(1000 / seconds);
            }

            //document.write("You finished the race in " + seconds + " seconds! <br> Score: " + score);
            alert("You finished the race in " + seconds + " seconds! \n Score: " + score);
        }
    }
    this.newPos = function () {
        this.x = this.speedX;
    }
}

function updateGameArea() {
    myGameArea.clear();
    myGamePiece.newPos();
    myGamePiece.update();
}



var seconds = 0;
var timer : number;
var timer_is_on = 0;

function timedCount() {
    seconds++;
    //document.getElementById("txt").value = seconds;
    timer = setTimeout(timedCount, 1000);
}

function startCount() {
    if (!timer_is_on) {
        timer_is_on = 1;
        timedCount();
    }
}

function stopCount() {
    clearTimeout(timer);
    timer_is_on = 0;
}

// cooldown for button after each press, based off of pet endurance
function cooldown() {
    var btn = document.getElementById("race");

    // rate that race button can be pressed 0-1000ms
    var bestRate = 0;
    var worstRate = 1000;
    var enduranceRate = worstRate;

    // assumes positive values between 0-100
    if (endurance >= 100) {
        enduranceRate = bestRate;
    } else if (endurance <= 0) {
        enduranceRate = worstRate;
    } else {
        // ranges values from 1000-0ms of button press wait time
        enduranceRate = 1000 - (endurance * 10);
    }

    // disable button for enduranceRate length
    (btn as HTMLButtonElement).disabled = true;
    setTimeout(function () {
        (btn as HTMLButtonElement).disabled = false;
    }, enduranceRate);
}
function moveRight() {
    var distance = (myGameArea.canvas as HTMLCanvasElement).width * (strength / 500);
    myGamePiece.speedX += (distance);

}

startGame();