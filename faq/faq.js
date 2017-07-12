/**************************************
 * JavaScript FAQ
 * Collector: fanfeilong@outlook.com
 * Create Time: 2016-11-16
 **************************************/

/**************************************
 * Q: How to zip/unzip (1)
 * A: zip.js;
 * C: http://gildas-lormeau.github.io/zip.js/
 **************************************/

//<script type="text/javascript" src="/lib/zip.js"></script>
// create the blob object storing the data to compress
var blob = new Blob([ "Lorem ipsum dolor sit amet, consectetuer adipiscing elit..." ], {
  type : "text/plain"
});
// creates a zip storing the file "lorem.txt" with blob as data
// the zip will be stored into a Blob object (zippedBlob)
zipBlob("lorem.txt", blob, function(zippedBlob) {
  // unzip the first file from zipped data stored in zippedBlob
  unzipBlob(zippedBlob, function(unzippedBlob) {
    // logs the uncompressed Blob
    console.log(unzippedBlob);
  });
});

function zipBlob(filename, blob, callback) {
  // use a zip.BlobWriter object to write zipped data into a Blob object
  zip.createWriter(new zip.BlobWriter("application/zip"), function(zipWriter) {
    // use a BlobReader object to read the data stored into blob variable
    zipWriter.add(filename, new zip.BlobReader(blob), function() {
      // close the writer and calls callback function
      zipWriter.close(callback);
    });
  }, onerror);
}

function unzipBlob(blob, callback) {
  // use a zip.BlobReader object to read zipped data stored into blob variable
  zip.createReader(new zip.BlobReader(blob), function(zipReader) {
    // get entries from the zip file
    zipReader.getEntries(function(entries) {
      // get data from the first file
      entries[0].getData(new zip.BlobWriter("text/plain"), function(data) {
        // close the reader and calls callback function with uncompressed data as parameter
        zipReader.close();
        callback(data);
      });
    });
  }, onerror);
}

function onerror(message) {
  console.error(message);
}

/**************************************
 * Q: How to zip/unzip (2)
 * A: jszip
 * C: http://stuk.github.io/jszip/
 **************************************/
var zip = new JSZip();
zip.file("Hello.txt", "Hello World\n");
var img = zip.folder("images");
img.file("smile.gif", imgData, {base64: true});
zip.generateAsync({type:"blob"})
.then(function(content) {
    // see FileSaver.js
    saveAs(content, "example.zip");
});

/**************************************
 * Q: How to zip/unzip (3)
 * A: adm-zip
 * C: https://github.com/cthackers/adm-zip
 **************************************/
var AdmZip = require('adm-zip');

// reading archives
var zip = new AdmZip("./my_file.zip");
var zipEntries = zip.getEntries(); // an array of ZipEntry records

zipEntries.forEach(function(zipEntry) {
    console.log(zipEntry.toString()); // outputs zip entries information
    if (zipEntry.entryName == "my_file.txt") {
         console.log(zipEntry.data.toString('utf8')); 
    }
});

// outputs the content of some_folder/my_file.txt
console.log(zip.readAsText("some_folder/my_file.txt")); 

// extracts the specified file to the specified location
zip.extractEntryTo(/*entry name*/"some_folder/my_file.txt", /*target path*/"/home/me/tempfolder", /*maintainEntryPath*/false, /*overwrite*/true);

// extracts everything
zip.extractAllTo(/*target path*/"/home/me/zipcontent/", /*overwrite*/true);


// creating archives
var zip = new AdmZip();

// add file directly
zip.addFile("test.txt", new Buffer("inner content of the file"), "entry comment goes here");

// add local file
zip.addLocalFile("/home/me/some_picture.png");

// get everything as a buffer
var willSendthis = zip.toBuffer();

// or write everything to disk
zip.writeZip(/*target file name*/"/home/me/files.zip");


/**************************************
 * Q: How to concat post chunks
 * A: https://cnodejs.org/topic/4faf65852e8fb5bc65113403
 * C: https://github.com/JacksonTian/bufferhelper
 **************************************/
var http = require('http');
var BufferHelper = require('bufferhelper');

http.createServer(function (request, response) {
  var bufferHelper = new BufferHelper();

  request.on("data", function (chunk) {
    bufferHelper.concat(chunk);
  });

  request.on('end', function () {
    var html = bufferHelper.toBuffer().toString();
    response.writeHead(200);
    response.end(html);
  });
}).listen(8001);

/**************************************
 * Q: How to encode and decode json
 * A: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/JSON
 *    http://stackoverflow.com/questions/34557889/how-to-deserialize-a-nested-buffer-using-json-parse
 * C: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/JSON/parse
 *    https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/JSON/stringify
 **************************************/
JSON.stringify({ [Symbol.for('foo')]: 'foo', data: new Buffer(1024) }, function(k, v) {
  if (typeof k === 'symbol') {
    return 'a symbol';
  }
});

var parse = JSON.parse(stringify, (k, v) => {
  if (
    v !== null            &&
    typeof v === 'object' && 
    'type' in v           &&
    v.type === 'Buffer'   &&
    'data' in v           &&
    Array.isArray(v.data)) {
    return new Buffer(v.data);
  }
  return v;
});

/**************************************
 * Q: How to validate json (1)
 * A: using jsonschema
 * C: https://github.com/tdegrunt/jsonschema
 **************************************/
var Validator = require('jsonschema').Validator;
var v = new Validator();

// Address, to be embedded on Person
var addressSchema = {
	"id": "/SimpleAddress",
	"type": "object",
	"properties": {
		"lines": {
			"type": "array",
			"items": {"type": "string"}
		},
		"zip": {"type": "string"},
		"city": {"type": "string"},
		"country": {"type": "string"}
	},
	"required": ["country"]
};

// Person
var schema = {
	"id": "/SimplePerson",
	"type": "object",
	"properties": {
		"name": {"type": "string"},
		"address": {"$ref": "/SimpleAddress"},
		"votes": {"type": "integer", "minimum": 1}
	}
};

var p = {
	"name": "Barack Obama",
	"address": {
		"lines": [ "1600 Pennsylvania Avenue Northwest" ],
		"zip": "DC 20500",
		"city": "Washington",
		"country": "USA"
	},
	"votes": "lots"
};

v.addSchema(addressSchema, '/SimpleAddress');
console.log(v.validate(p, schema));

/**************************************
 * Q: How to validate json (2)
 * A: using z-schema
 * C: https://github.com/zaggino/z-schema
 **************************************/
var ZSchema = require("z-schema");
var options = {}
var validator = new ZSchema(options);
var schemas = [
	{
	    id: "personDetails",
	    type: "object",
	    properties: {
	        firstName: { type: "string" },
	        lastName: { type: "string" }
	    },
	    required: ["firstName", "lastName"]
	},
	{
        id: "addressDetails",
        type: "object",
        properties: {
            street: { type: "string" },
            city: { type: "string" }
        },
        required: ["street", "city"]
    },
    {
        id: "personWithAddress",
        allOf: [
            { $ref: "personDetails" },
            { $ref: "addressDetails" }
        ]
    }
];

var json = {
    firstName: "Martin",
    lastName: "Zagora",
    street: "George St",
    city: "Sydney"
};

validator.validate(json, schema, function (err, valid) {
	if(err){
		console.log("validate error:"+err);
		return;
	}
    validator.validate(json,schema[1],function(err,valid){
    	//...
    });
});

/**************************************
 * Q: How to sequence asynchronize callbacks (1)
 * A: step it
 * C: https://github.com/creationix/step
 **************************************/
Step(
  function readSelf() {
    fs.readFile(__filename, this);
  },
  function capitalize(err, text) {
    if (err) throw err;
    return text.toUpperCase();
  },
  function showIt(err, newText) {
    if (err) throw err;
    console.log(newText);
  }
);

/**************************************
 * Q: How to sequence asynchronize callbacks (2)
 * A: async it
 * C: http://caolan.github.io/async/
 *    https://github.com/caolan/async
 **************************************/
async.map(['file1','file2','file3'], fs.stat, function(err, results){
    // results is now an array of stats for each file
});

async.filter(['file1','file2','file3'], function(filePath, callback) {
  fs.access(filePath, function(err) {
    callback(null, !err)
  });
}, function(err, results){
    // results now equals an array of the existing files
});

async.parallel([
    function(callback){ ... },
    function(callback){ ... }
], function(err, results) {
    // optional callback
});

async.series([
    function(callback){ ... },
    function(callback){ ... }
]);

/**************************************
 * Q: How to sequence asynchronize callbacks (3)
 * A: Wind.js
 * C: https://github.com/JeffreyZhao/wind
 *    https://github.com/caolan/async
 **************************************/
var Wind = require("../../../src/wind");

var fib = eval(Wind.compile("async", function () {

    $await(Wind.Async.sleep(1000));
    console.log(0);
    
    $await(Wind.Async.sleep(1000));
    console.log(1);

    var a = 0, current = 1;
    while (true) {
        var b = a;
        a = current;
        current = a + b;

        $await(Wind.Async.sleep(1000));
        console.log(current);
    }
}));

fib().start();

/**************************************
 * Q: How to kill child processes when process exit
 * A: 1. listen on process's exit event
 *    2. listen on process's ctrl-c and kill signal
  **************************************/
let children = [];
process.on('exit', function() {
  self.children.forEach(function(child) {
    child.kill();
  });
});

let cleanExit = function() { process.exit() };
process.on('SIGINT', cleanExit); // catch ctrl-c
process.on('SIGTERM', cleanExit); // catch kill



/**************************************
 * Q: How to read and write Uint64
 * A: 1. read the buffer
 *    2. write back the buffer
  **************************************/
function readUint64(buf,offset){
    return buf.slice(offset,offset+8);
}

function writeUint64(dest,src,offset){
    for(let i=0;i<8;i++){
        dest[offset+i]=src[i];
    }
}








