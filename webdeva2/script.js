const page1btn = document.querySelectorAll("nav div ul li")[0];
const page2btn = document.querySelectorAll("nav div ul li")[1];
const page3btn = document.querySelectorAll("nav div ul li")[2];
var allpages = document.querySelectorAll("div section");
/*for hamMenu */
const hamBtn=document.querySelector("#hamMenu");
const menuItemsList=document.querySelector("nav ul");
const backtotopBtn = document.querySelector("#BackToTop");

const pigeonInfo = document.querySelectorAll(".pigeoninfo");
var pigeonGameBtns = document.querySelectorAll(".pigeonGameBtn");
var scoreboard = document.getElementById("pigeonScoreboard");
var gamecontainer = document.getElementById("pigeonGame");
var gamebg = document.getElementById("gameBG");
//using canvas because it makes things disappear once out of canvas
var ctx = gamebg.getContext("2d");

const falcInfo = document.querySelectorAll(".FalcLeft, .FalcRight");
var falcleft = document.querySelectorAll(".FalcLeft");
var falcright = document.querySelectorAll(".FalcRight");
const speedgraph = document.querySelectorAll("#speedGraph div")[1];

//set stuff at start
menuItemsList.classList.toggle("toggleDisplay");
//make the ham menu start closed (only if width < 800)
if (window.matchMedia("(max-width: 800px)").matches){
    menuItemsList.classList.toggle("toggleCloseDisplay");
    menuItemsList.classList.toggle("toggleDisplay");
}
hidepages(pigeonInfo);
hidepages(falcInfo);
hidepages(allpages);
showpage(allpages, 3); //start page
speedgraph.style.display = "none";
var previousWidth1 = 1; //start at 1 for the first call (x * 1 = x)
ResizePigeonMap();
var previousWidth2 = 1;
ResizeFalcMap();
resizeGameBG();
pigeonGameBtns[0].style.display = "none";

var installPromptEvent;
window.addEventListener('beforeinstallprompt', event => {
    // Prevent Chrome <= 67 from automatically showing the prompt
    event.preventDefault();
    // Stash the event so it can be triggered later.
    installPromptEvent = event;
});
if (installPromptEvent != null){
    installPromptEvent.prompt();
    installPromptEvent.userChoice.then(choice => {
        if (choice.outcome === 'accepted') {
        console.log('User accepted the A2HS prompt');
        } else {
        console.log('User dismissed the A2HS prompt');
        }
        // Clear the saved prompt since it can't be used again
        installPromptEvent = null;
    });
}
window.addEventListener("scroll", function() {
    //if user scrolled down a certain amount, show the BackToTopBtn
    //amt scrolled > scrollable height
    if (window.scrollY + (window.innerHeight*1.5) > this.document.body.scrollHeight){
        //scrolled about halfway through the content
        backtotopBtn.style.display = "inline";
    }
    else{
        backtotopBtn.style.display = "none";
    }
})

// https://stackoverflow.com/questions/8634875/how-to-put-border-on-area
// get areas of all image maps and create a border of the areas when hovered
var areas = document.getElementsByTagName("area");
for(let area of areas) {
    area.addEventListener("mouseover", function () {this.focus({preventScroll:true});});
    //area.addEventListener("mouseover", function(){ResizeMap(area.parentElement, area.parentElement.previousSibling.width);});
    area.addEventListener("mouseout", function () {this.blur();});

    //prevent clicking area from moving the screen
    area.addEventListener("click", function(e){e.preventDefault();});
};

window.addEventListener("resize", function(){
    //when resize, close the menu (prevent menu being open while ham isnt X)
    if (window.matchMedia("(max-width: 800px)").matches){
        if (menuItemsList.classList.contains("toggleDisplay"))
            toggleMenus();
        if (menuItemsList.classList.contains("toggleCloseDisplay") && hamBtn.classList.contains("hamMenuSpanAnim")){
            //check if menu is closed but ham is still X (happens when entering from laptop to mobile size through inspect)
            hamBtn.classList.toggle("hamMenuSpanAnim");
        }
    }
    else{
        //resizing to > 800, show the navbar
        if (menuItemsList.classList.contains("toggleCloseDisplay")){
            toggleMenus();
        }
    }
    //resize image map areas
    ResizeFalcMap();
    ResizePigeonMap();
    //resize pigeon game canvas
    resizeGameBG();
});
//listen for click
page1btn.addEventListener("click", function () {showpage(allpages, 1), ResizePigeonMap();});
page2btn.addEventListener("click", function () {showpage(allpages, 2), ResizeFalcMap();});
page3btn.addEventListener("click", function () {showpage(allpages, 3)});
hamBtn.addEventListener("click",toggleMenus);
backtotopBtn.addEventListener("click", backtotop);
//falcon speed graph bars finish aniamtion
document.querySelector(".barFalc").addEventListener("animationend", function(){
    //bar moving anims end at the same time, so just call one of them
    //to change the <p> text once anim ends
    var diff = 390 - document.querySelector(".form form input").value;
    //convert neg to pos
    if (diff < 0)
        diff = diff * -1;
    if (diff != 0)
        var countDiffHTML = "Your guess was " + diff + " off!" ;
    else //guess was perfect, no need show difference
        var countDiffHTML= "";
    //change html to show the stuff
    document.querySelector("#speedGraph div p").innerHTML = countDiffHTML + "<br><b>The falcon tops out at 390 km/h!</b>";
});
//on mouse click in canvas
gamebg.addEventListener("mousedown", function(event){
    var mousepos = getMouseInCanvas(gamebg, event);
    //https://web.archive.org/web/20161220195326/http://simonsarris.com/blog/510-making-html5-canvas-useful
    for (var i=0; i < objects.length; i++){
        //check if object was clicked
        if (objects[i].x <= mousepos.x && objects[i].x + objects[i].w >= mousepos.x
            && objects[i].y <= mousepos.y && objects[i].y + objects[i].h >= mousepos.y){
            //check type
            if (objects[i].type == "yellow"){
                //if obj not clicked before, give score
                if (objects[i].clicked == false){
                    score++;
                    UpdatePigeonSB();
                    objects[i].clicked = true;
                }
            }
            else{
                //if obj not clicked before, remove score
                if (objects[i].clicked == false){
                    score--;
                    UpdatePigeonSB();
                    objects[i].clicked = true;
                }
            }
        }
    }
});

//form
function hideForm(){
    document.getElementById("speedGuessForm").style.display = "none";
    speedgraph.style.display = "inline";
    var p = document.querySelector("#speedGraph div p");
    var n =document.querySelector(".form form input").value;
    //message for how good the player's guesses were
    if (n >= 250 && n <= 300 ){
        p.innerHTML = "<b>Close guess! It's actually faster!</b>"
    }
    else if (n > 300 && n < 390){
        p.innerHTML = "<b>Great guess!</b>";
    }
    else if (n == 390){
        p.innerHTML ="<b>Right on!</b>";
    }
    else if (n > 390 && b < 450){
        p.innerHTML = "<b>Close guess! It's actually slower!</b>"
    }
}

function backtotop(){
    window.scrollTo(0,0);
}

function hidepages(list){
    for (let item of list){ //cycle through allpages
        item.style.display = "none";
    }
}
function showpage(list, num){
    hidepages(list);
    //select the page based on the parameter passed in
    let item=list[num-1];
    //show the page
    item.style.display="block";
}

function showPigeonInfo(num){
    //scrollTo(pigeonInfo[num]);
    hidepages(pigeonInfo);
    pigeonInfo[num].style.display = "inline-block";
    
    if (window.matchMedia("(max-width: 800px)").matches){
        //for mobile, the info shows below, so scroll down-
        //- till info box is at the center of screen
        pigeonInfo[num].scrollIntoView({behavior:"smooth", block:"center"});
    }
}
function hideFalcInfo(side){
    if (side == 0){
        for (let l of falcleft){
            l.style.display = "none";
        }
    }
    else{
        for (let r of falcright){
            r.style.display = "none";
        }
    }
}
function showFalcInfo(num){
    //get side using class. if FalcLeft, then side=0, else side=1
    let side = (falcInfo[num].className == "FalcLeft" ? 0 : 1);
    
    //if mobile, there is no left/right
    if (window.matchMedia("(max-width: 800px)").matches){
        if (falcInfo[num].style.display == "none"){
            //if not showing, show
            hideFalcInfo(0);
            hideFalcInfo(1);
            falcInfo[num].style.display = "block";
            falcInfo[num].scrollIntoView({behavior:"smooth", block:"center"});
        }
        else{
            //if showing, hide
            hideFalcInfo(0);
            hideFalcInfo(1);
        }
    }
    else{
        //hide the same side so no overlap
        if (falcInfo[num].style.display == "none"){
            //if hidden, show
            hideFalcInfo(side);
            falcInfo[num].style.display = "inline";
        }
        else{
            //if showing, hide
            hideFalcInfo(side);
        }
    }
}

function toggleMenus(){ /*open and close ham menu*/
    menuItemsList.classList.toggle("toggleCloseDisplay");
    menuItemsList.classList.toggle("toggleDisplay");
    hamBtn.classList.toggle("hamMenuSpanAnim");
}
function RestartAnim(){ 
    document.getElementById("speedGuessForm").style.display = "inline";
    speedgraph.style.display = "none";
    //cloning method adapted from: https://stackoverflow.com/questions/58170892/javascript-add-remove-animation-class-only-animates-once
    // let bars = document.querySelectorAll(".bar div");
    // for (let b of bars){
    //     //for each bar, clone it
    //     let newb = b.cloneNode(true);
    //     //if alr have, toggle off, so that when toggled again, anim immediately plays
    //     if (newb.classList.contains("barAnim"))
    //         newb.classList.toggle("barAnim");
    //     newb.classList.toggle("barAnim");
    //     //replace the old one, which resets the animation
    //     b.replaceWith(newb);
    // }
}

function playAudio(id){
    var a = document.getElementById(id);
    a.play();
}

//resize area coords of imagemap
//reference: https://stackoverflow.com/questions/13321067/dynamically-resizing-image-maps-and-images
//width - original pixel size of the image
function ResizePigeonMap(){
    var map = document.querySelector("#pigeonMap");
    var width = 204;
    let areas = map.getElementsByTagName("area"),
    image = map.previousElementSibling, 
    len = areas.length,
    coords = [];

    for (let i=0; i < len; i++){
        coords[i] = areas[i].coords.split(","); //get coords
    }
    let newWidth = image.width / width;
    for (let i=0; i < len; i++){ //i - coords of an area
        let cLen = coords[i].length;
        for (let j=0; j < cLen; j++){ //j - each (x1, y1, x2, y2) of the coord[i]
            //reset coords back to the "original" so changes dont accumulate
            coords[i][j] /= previousWidth1;
            //then change the coords based on newWidth
            coords[i][j] *= newWidth;
        }
        areas[i].coords = coords[i].join(","); //insert new coords
    }
    previousWidth1 = newWidth;
}

function ResizeFalcMap(){
    var map = document.querySelector("#falcdiag");
    var width = 693;
    let areas = map.getElementsByTagName("area"),
    image = map.previousElementSibling, 
    len = areas.length,
    coords = [];
    
    for (let i=0; i < len; i++){
        coords[i] = areas[i].coords.split(","); //get coords
    }
    let newWidth = image.width / width;
    for (let i=0; i < len; i++){ //i - coords of an area
        let cLen = coords[i].length;
        for (let j=0; j < cLen; j++){ //j - each (x1, y1, x2, y2) of the coord[i]
            //reset coords back to the "original" so changes dont accumulate
            coords[i][j] /= previousWidth2;
            //then change the coords based on newWidth
            coords[i][j] *= newWidth;
        }
        areas[i].coords = coords[i].join(","); //insert new coords
    }
    previousWidth2 = newWidth;
}

//min included, max included. Floor rounds the number to an Integer
function RandInt(min, max){
    return Math.floor(Math.random() * (max - min+1)) + min;
}
//inclusive
function RandFloat(min, max){
    return (Math.random() * (max-min+1)) + min;
}

//Pigeon game stuff
var score =0;
//store vars for accuracy display
var totalObjects =0;
function resizeGameBG(){
    if (window.matchMedia("(max-width: 800px)").matches){
        gamebg.width= screen.width-5;
        //dont let gamebg width go past 400
        if (gamebg.width > 400){
            gamebg.width = 400;
        }
        //if screen height too small to fit the container
        if (gamecontainer.offsetHeight >= screen.height){
            //resize gamebg
            gamebg.height = screen.height - scoreboard.offsetHeight*2;
        }
        else{
            gamebg.height = 400;
        }
    }
    else{
        gamebg.width = 400;
        gamebg.height = 400;
    }
}

function UpdatePigeonSB(){
    //if-else to prevent "infinity" accuracy due to division by 0
    if (totalObjects == 0){
        scoreboard.innerHTML = "Score: " + score + "<br>Accuracy: -";
    }
    else{
        scoreboard.innerHTML = "Score: " + score + "<br>Accuracy: " + ((score*100)/totalObjects).toFixed(1) + "%"; //1 dp
    }
}
//drawing canvas objects: https://jsfiddle.net/m1erickson/RCLtR/
//in ms
var baseSpawnRate = 1500;
var spawnRate = baseSpawnRate;
var width = 30;
var height = 30;
//when was the last object spawned
var lastSpawn = -1;
var objects = [];
//save the starting time (used to calc elapsed time)
var startTime = Date.now();

function spawnRandomObject() {
    //randomise spawn rate
    spawnRate = RandInt(baseSpawnRate*0.7, baseSpawnRate*1.4);
    //Math.Random gets from 0(inc) to 1(exc)
    //select a random type for this new object
    var t;
    if (Math.random() < 0.50) {
        t = "yellow";
        totalObjects++; //count yellow boxes
    } else {
        t = "rgb(120, 93, 35)";
    }
    //random spawn point and dir
    var spawnlineX,dx;
    var spawnlineY,dy;
    if (Math.random() < 0.25){
        //spawn top of box, random x
        spawnlineY =0;
        //randomly between width-i and i
        spawnlineX = RandFloat(width, gamebg.width-width);
        //move down
        dx = 0;
        dy = 1;
    }
    else if (Math.random() < 0.5){
        //spawn bottom of box, random x
        spawnlineY = gamebg.height;
        spawnlineX = RandFloat(width, gamebg.width-width);
        //move up
        dx = 0;
        dy = -1;
    }
    else if (Math.random() < 0.75){
        //spawn left, random y
        spawnlineY = RandFloat(height, gamebg.height-height);
        spawnlineX = 0;
        //move right
        dx = 1;
        dy = 0;
    }
    else{
        //spawn right, random y
        spawnlineY = RandFloat(height, gamebg.height-height);
        spawnlineX = gamebg.width;
        //move left
        dx = -1;
        dy = 0;
    }

    // create the new object
    var object = {
        // set this objects type
        type: t,
        //x and y: where the object spawns
        x: spawnlineX,
        y: spawnlineY,
        //dir: which ways the object moves
        dirX: dx,
        dirY: dy,
        //size of object
        w: width,
        h: height,
        //to record if this box has been clicked
        clicked: false,
    }
    //console.log(object.x + " , " + object.y + "| dir: " + object.dirX + ","+object.dirY);
    // add the new object to the objects[] array
    objects.push(object);
}
//continuously call animate
var gameAnimateID;//to get interval ID for clearing
function stopPigeonGame(){
    //stop calling the animate func
    clearInterval(gameAnimateID);
    //clear canvas context
    ctx.clearRect(0, 0, gamebg.width, gamebg.height);
    //display other button to Start game
    pigeonGameBtns[0].style.display = "none";
    pigeonGameBtns[1].style.display = "inline";
    //return back to original place
    document.querySelector("#pigeonGame table tr td").classList.toggle("sticky");

    if (window.matchMedia("(max-width: 800px)").matches){
        hamBtn.style.display = "inline";
    }
}
function startPigeonGame(){
    //start continuous animate calls
    gameAnimateID = setInterval(animate, 1);
    //display the Stop button instead
    pigeonGameBtns[0].style.display = "inline";
    pigeonGameBtns[1].style.display = "none";
    //stop-btn sticky so it doesnt go off screen when scrolling is locked
    document.querySelector("#pigeonGame table tr td").classList.toggle("sticky");
    //scroll to center of gamebg. Due to disabling the scroll, smooth wont work
    gamebg.scrollIntoView({behavior:"instant", block:"center"});

    if (window.matchMedia("(max-width: 800px)").matches){
        //remove to prevent possibility of the menu overlapping the stop btn
        hamBtn.style.display = "none";
    }
}
function disableScrolling(){
    //https://www.geeksforgeeks.org/how-to-disable-scrolling-temporarily-using-javascript/
    // get curr scroll pos. using or (||) in case one of them returns 0
    scrollTop = window.pageYOffset || document.documentElement.scrollTop;
  
    //if window scrolls, set to the prev y, nullifying the scroll
    window.onscroll = function() {
        window.scrollTo(0, scrollTop);
    };
}
function enableScrolling(){
    //reset the func to return back to normal
    window.onscroll = function() {};
}
function animate() {
    // get the elapsed time
    var time = Date.now();
    // see if its time to spawn a new object
    if (time > (lastSpawn + spawnRate)) {
        lastSpawn = time;
        spawnRandomObject();
    }
    // clear the canvas so objects dont look like a trail
    ctx.clearRect(0, 0, gamebg.width, gamebg.height);
    // move each object down the canvas
    for (var i = 0; i < objects.length; i++) {
        var object = objects[i];
        //top left: 0,0
        //move objects at varying speed, based on the predetermined direction
        object.y += object.dirY*(Math.random() + 0.5);
        object.x += object.dirX*(Math.random() + 0.5);
        //draw object as a rect with colour defined earlier by type
        ctx.fillStyle = object.type;
        ctx.fillRect(object.x, object.y, object.w, object.h);
        ctx.closePath();
        //object left canvas area
        if (object.y > gamebg.height || object.y < 0
            || object.x > gamebg.width || object.x < 0){
            objects.splice(i, 1); //remove this 1 object from arr
            UpdatePigeonSB();
        }
    }
}

function getMouseInCanvas(canvas, event){
    var cRect = canvas.getBoundingClientRect();
    var scaleX = canvas.width / cRect.width;
    var scaleY = canvas.height / cRect.height;
    return {
        x: (event.clientX - cRect.left) / (cRect.right - cRect.left) * canvas.width,
        y: (event.clientY - cRect.top) / (cRect.bottom - cRect.top) * canvas.height
      }
}