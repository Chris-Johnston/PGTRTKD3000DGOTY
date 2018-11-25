// Login code
function tryLogin()
{
    const usernameField = (document.getElementById("username") as HTMLInputElement).value;
    const passwordField = (document.getElementById("password") as HTMLInputElement).value;
    var resultsField = document.getElementById("results") as HTMLParagraphElement;

    var request = new XMLHttpRequest();
    request.open("POST", "/api/login", true);
    var body = {
        "username": usernameField,
        "password": passwordField
    };
    // register the callback to print out the request body
    request.onreadystatechange = function () {
        if (request.readyState == XMLHttpRequest.DONE) {
            if (request.status >= 400 && request.status < 500) {
                alert('4xx error when trying to login');
            }
            else {
                // got a good response from the api login endpoint
                console.log(request.responseText);
            }
        }
    }
    // send json body for the username and password
    request.setRequestHeader('Content-Type', 'application/json');
    console.log(`sending ${JSON.stringify(body)}`);
    request.send(JSON.stringify(body));
}