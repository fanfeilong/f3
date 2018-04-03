const domain = require('domain');
const fs = require('fs');

process.on('uncaughtException', function(err) {
    console.error('Error caught in uncaughtException event:', err);
});
 
var d = domain.create();
 
d.on('error', function(err) {
    console.error('Error caught by domain:', err);
});
 
d.run(function() {
    process.nextTick(function() {
        fs.readFile('non_existent.js', function(err, str) {
            if(err) throw err;
            else console.log(str);
        });
    });
});