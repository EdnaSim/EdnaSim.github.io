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
    //cloning method adapted from: https://stackoverflow.com/questions/58170892/javascript-add-remove-animation-class-only-animates-once
    let bars = document.querySelectorAll(".bar div");
    for (let b of bars){
        //for each bar, clone it
        let newb = b.cloneNode(true);
        //replace the old one, which resets the animation
        b.replaceWith(newb);
    }
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
            //reset coords back to the "original"
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
page2btn.addEventListener("click", function () {showpage(allpages, 2)});
page3btn.addEventListener("click", function () {showpage(allpages, 3)});
hamBtn.addEventListener("click",toggleMenus);
backtotopBtn.addEventListener("click", backtotop);
    