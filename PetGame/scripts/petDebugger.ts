// pet debugger util script

const api = '/api/Pet'

var results_section = document.getElementById('results-section');
var results_code = document.getElementById('results-code');

var btn_get = document.getElementById('submit_get') as HTMLButtonElement;
var btn_post = document.getElementById('submit_post') as HTMLButtonElement;
var btn_put = document.getElementById('submit_put') as HTMLButtonElement;
var btn_delete = document.getElementById('submit_delete') as HTMLButtonElement;

class Pet {
    petid: number;
    name: string;
    birthday: string;
    strength: number;
    endurance: number;
    isdead: boolean;
    userid: number;
}

function MakePetFromPost(): Pet {
    var p = new Pet();
    p.name = (document.getElementById('post_pet_name') as HTMLInputElement).value;
    // p.birthday = (document.getElementById('post_pet_birthday') as HTMLInputElement).valueAsDate;
    p.birthday = new Date().toJSON();
    p.strength = (document.getElementById('post_pet_strength') as HTMLInputElement).valueAsNumber;
    p.endurance = (document.getElementById('post_pet_endurance') as HTMLInputElement).valueAsNumber;
    // this value is being difficult for some reason
    // p.isdead = (document.getElementById('post_pet_isdead') as HTMLInputElement).value === "on";
    p.isdead = false;
    p.userid = (document.getElementById('post_pet_userid') as HTMLInputElement).valueAsNumber;
    // ignore
    p.petid = 0;
    return p;
}

function MakePetFromPut(): Pet {
    var p = new Pet();
    p.name = (document.getElementById('put_pet_name') as HTMLInputElement).value;
    // p.birthday = (document.getElementById('put_pet_birthday') as HTMLInputElement).valueAsDate;
    p.birthday = new Date().toJSON();
    p.strength = (document.getElementById('put_pet_strength') as HTMLInputElement).valueAsNumber;
    p.endurance = (document.getElementById('put_pet_endurance') as HTMLInputElement).valueAsNumber;
    //TODO this value is being difficult, always true for some reason
    // p.isdead = (document.getElementById('put_pet_isdead') as HTMLInputElement).value === "on";
    p.isdead = false;
    p.userid = (document.getElementById('put_pet_userid') as HTMLInputElement).valueAsNumber;
    // ignore
    p.petid = 0;
    return p;
}

function requestCallback() {
    console.log(this.responseText);
    // Todo: Add a basic XSS validation util script
    // WARNING, DOES NOT HANDLE XSS SANITIZATION
    results_section.innerHTML = this.responseText;
    results_code.innerHTML = `${this.status}`;
}

btn_get.addEventListener('click', function () {
    // on get click
    var id = (document.getElementById('get_pet_id') as HTMLInputElement).valueAsNumber;
    // make request
    var request = new XMLHttpRequest();
    request.addEventListener('load', requestCallback);
    request.open('GET', `${api}/${id}`);
    request.setRequestHeader("Content-Type", "application/json");
    request.send();
});

btn_post.addEventListener('click', function () {
    var pet = MakePetFromPost();
    console.log(pet);
    // make request
    var request = new XMLHttpRequest();
    request.addEventListener('load', requestCallback);
    request.open('POST', `${api}`);
    request.setRequestHeader("Content-Type", "application/json");
    var x = JSON.stringify(pet);
    request.send(x);
});

btn_put.addEventListener('click', function () {
    var id = (document.getElementById('put_pet_id') as HTMLInputElement).valueAsNumber;
    var pet = MakePetFromPut();
    console.log(pet);
    // make request
    var request = new XMLHttpRequest();
    request.addEventListener('load', requestCallback);
    request.open('PUT', `${api}/${id}`);
    request.setRequestHeader("Content-Type", "application/json");
    var x = JSON.stringify(pet);
    request.send(x);
});

btn_delete.addEventListener('click', function () {
    // on get click
    var id = (document.getElementById('delete_pet_id') as HTMLInputElement).valueAsNumber;
    // make request
    var request = new XMLHttpRequest();
    request.addEventListener('load', requestCallback);
    request.open('DELETE', `${api}/${id}`);
    request.setRequestHeader("Content-Type", "application/json");
    request.send();
});