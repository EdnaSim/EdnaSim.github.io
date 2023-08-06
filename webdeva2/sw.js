self.addEventListener('activate', event => {
    clients.claim();
    console.log('Ready!');
});
self.addEventListener("fetch", function(event){

});