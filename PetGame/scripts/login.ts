// Login code
// bla bla bla
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
            console.log(request.responseText);
            alert(request.responseText);
        }
    }

    request.setRequestHeader('Content-Type', 'application/json');
    console.log(`sending ${JSON.stringify(body)}`);
    request.send(JSON.stringify(body));
}