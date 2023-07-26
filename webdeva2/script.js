const page1btn = document.querySelectorAll("nav ul li")[0];
const page2btn = document.querySelectorAll("nav ul li")[1];
const page3btn = document.querySelectorAll("nav ul li")[2];
var allpages = document.querySelectorAll("div section");
/*for hamMenu */
const hamBtn=document.querySelector("#hamMenu");
const menuItemsList=document.querySelector("nav ul");
const backtotopBtn = document.querySelector("#BackToTop");
menuItemsList.classList.toggle("toggleDisplay");

// https://stackoverflow.com/questions/8634875/how-to-put-border-on-area
// get areas of the pigeon image map and create a border of the areas when hovered
var areas = document.getElementsByTagName("area");
for(let area of areas) {    
    area.addEventListener("mouseover", function () {this.focus();}, false );
    //area.addEventListener("mouseover", function(){ResizeMap(area.parentElement, area.parentElement.previousSibling.width);});
    area.addEventListener("mouseout", function () {this.blur();}, false );
};
const pigeonInfo = document.querySelectorAll(".pigeoninfo");
const falcInfo = document.querySelectorAll(".FalcLeft, .FalcRight");
const speedgraph = document.querySelectorAll("#speedGraph div")[1];
speedgraph.style.display = "none";

//form
function hideForm(){
    document.getElementById("speedGuessForm").style.display = "none";
    speedgraph.style.display = "inline";
    var p = document.querySelector("#speedGraph div p");
    var n =document.querySelector(".form form input").value;
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

hidepages(pigeonInfo);
hidepages(falcInfo);
hidepages(allpages);
showpage(allpages, 2);

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
    hidepages(pigeonInfo);
    pigeonInfo[num].style.display = "inline-block";
}
function hideFalcInfo(side){
    let left = document.querySelectorAll(".FalcLeft");
    let right = document.querySelectorAll(".FalcRight");
    if (side == 0){
        for (let l of left){
            l.style.display = "none";
        }
    }
    else{
        for (let r of right){
            r.style.display = "none";
        }
    }
}
function showFalcInfo(num){
    //get side using class. if FalcLeft, then side=0, else side=1
    let side = (falcInfo[num].className == "FalcLeft" ? 0 : 1);
    //hide the same side so no overlap
    hideFalcInfo(side);
    falcInfo[num].style.display = "inline";
}

function toggleMenus(){ /*open and close menu*/
    menuItemsList.classList.toggle("toggleCloseDisplay");
    menuItemsList.classList.toggle("toggleDisplay");
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

//resize coords of imagemap
//reference: https://stackoverflow.com/questions/13321067/dynamically-resizing-image-maps-and-images
var previousWidth = 1; //start at 1 for the first call (x * 1 = x)
//width - original pixel size of the image
function ResizeMap(map, width){
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
            //reset coords back to the "original" so changes dont accumalate
            coords[i][j] /= previousWidth;
            //then change the coords based on newWidth
            coords[i][j] *= newWidth;
        }
        areas[i].coords = coords[i].join(","); //insert new coords
    }
    previousWidth = newWidth;
}
ResizeMap(document.querySelector("#falcdiag"), 693); //immediately resize
//when window is resized, update the image map(s)
window.addEventListener("resize", function() {ResizeMap(document.querySelector("#falcdiag"), 693);});

//listen for click
page1btn.addEventListener("click", function () {showpage(allpages, 1)});
page2btn.addEventListener("click", function () {showpage(allpages, 2), ResizeMap(document.querySelector("#falcdiag"), 693);});
page3btn.addEventListener("click", function () {showpage(allpages, 3)});
hamBtn.addEventListener("click",toggleMenus);
backtotopBtn.addEventListener("click", backtotop);

document.querySelector(".barFalc").addEventListener("animationend", function(){
    //bar moving anims end at the same time, so just call one of them
    //to change th <p> text once anim ends
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
})
    