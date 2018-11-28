// pet debugger util script

const api = '/api/Pet'

var results_section = document.getElementById('results-section');
var results_code = document.getElementById('results-code');

var btn_get = document.getElementById('submit_get') as HTMLButtonElement;
var btn_post = document.getElementById('submit_post') as HTMLButtonElement;
var btn_put = document.getElementById('submit_put') as HTMLButtonElement;
var btn_delete = document.getElementById('submit_delete') as HTMLButtonElement;

declare class Pet {
    Name: string;
    Birthday: Date;
    Strength: number;
    Endurance: number;
    IsDead: boolean;
    UserId: number;
}

function MakePetFromPost() : Pet {
    var p = new Pet();
    p.Name = document.getElementById('post_pet_name').innerText;
    p.Birthday = new Date(document.getElementById('post_pet_birthday').nodeValue);
    p.Strength = parseInt(document.getElementById('post_pet_strength').innerText);
    p.Endurance = parseInt(document.getElementById('post_pet_endurance').innerText);
    p.IsDead = Boolean(document.getElementById('post_pet_isdead').innerText);
    p.UserId = parseInt(document.getElementById('post_pet_userid').innerText);
    return p;
}

function requestCallback() {
    console.log(this.responseText);
    // WARNING, DOES NOT HANDLE XSS SANITIZATION
    results_section.innerHTML = this.responseText;
    results_code.innerHTML = `${this.status}`;
}

btn_get.addEventListener('click', function () {
    // on get click
    var id = (document.getElementById('get_pet_id') as HTMLFormElement).valueAsNumber;
    // make request
    var request = new XMLHttpRequest();
    request.addEventListener('load', requestCallback);
    request.open('GET', `${api}/${id}`);
    request.send();
});