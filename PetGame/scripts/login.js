function tryLogin() {
    var usernameField = document.getElementById("username").value;
    var passwordField = document.getElementById("password").value;
    var resultsField = document.getElementById("results");
    var request = new XMLHttpRequest();
    request.open("POST", "/api/login", true);
    var body = {
        "username": usernameField,
        "password": passwordField
    };
    request.onreadystatechange = function () {
        if (request.readyState == XMLHttpRequest.DONE) {
            if (request.status >= 400 && request.status < 500) {
                alert('4xx error when trying to login');
            }
            else {
                console.log(request.responseText);
            }
        }
    };
    request.setRequestHeader('Content-Type', 'application/json');
    console.log("sending " + JSON.stringify(body));
    request.send(JSON.stringify(body));
}
//# sourceMappingURL=login.js.map