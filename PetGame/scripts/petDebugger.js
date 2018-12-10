"use strict";
var api = '/api/Pet';
var results_section = document.getElementById('results-section');
var results_code = document.getElementById('results-code');
var btn_get = document.getElementById('submit_get');
var btn_post = document.getElementById('submit_post');
var btn_put = document.getElementById('submit_put');
var btn_delete = document.getElementById('submit_delete');
function MakePetFromPost() {
    var p = new Pet();
    p.Name = document.getElementById('post_pet_name').value;
    p.Birthday = new Date().toJSON();
    p.Strength = document.getElementById('post_pet_strength').valueAsNumber;
    p.Endurance = document.getElementById('post_pet_endurance').valueAsNumber;
    p.IsDead = false;
    p.UserId = document.getElementById('post_pet_userid').valueAsNumber;
    p.PetId = 0;
    return p;
}
function MakePetFromPut() {
    var p = new Pet();
    p.Name = document.getElementById('put_pet_name').value;
    p.Birthday = new Date().toJSON();
    p.Strength = document.getElementById('put_pet_strength').valueAsNumber;
    p.Endurance = document.getElementById('put_pet_endurance').valueAsNumber;
    p.IsDead = false;
    p.UserId = document.getElementById('put_pet_userid').valueAsNumber;
    p.PetId = 0;
    return p;
}
function requestCallback() {
    console.log(this.responseText);
    results_section.innerHTML = this.responseText;
    results_code.innerHTML = "" + this.status;
}
btn_get.addEventListener('click', function () {
    var id = document.getElementById('get_pet_id').valueAsNumber;
    var request = new XMLHttpRequest();
    request.addEventListener('load', requestCallback);
    request.open('GET', api + "/" + id);
    request.setRequestHeader("Content-Type", "application/json");
    request.send();
});
btn_post.addEventListener('click', function () {
    var pet = MakePetFromPost();
    console.log(pet);
    var request = new XMLHttpRequest();
    request.addEventListener('load', requestCallback);
    request.open('POST', "" + api);
    request.setRequestHeader("Content-Type", "application/json");
    var x = JSON.stringify(pet);
    request.send(x);
});
btn_put.addEventListener('click', function () {
    var id = document.getElementById('put_pet_id').valueAsNumber;
    var pet = MakePetFromPut();
    console.log(pet);
    var request = new XMLHttpRequest();
    request.addEventListener('load', requestCallback);
    request.open('PUT', api + "/" + id);
    request.setRequestHeader("Content-Type", "application/json");
    var x = JSON.stringify(pet);
    request.send(x);
});
btn_delete.addEventListener('click', function () {
    var id = document.getElementById('delete_pet_id').valueAsNumber;
    var request = new XMLHttpRequest();
    request.addEventListener('load', requestCallback);
    request.open('DELETE', api + "/" + id);
    request.setRequestHeader("Content-Type", "application/json");
    request.send();
});
//# sourceMappingURL=petDebugger.js.map