function Task(fn) {  
	var deferred = null;
	var state = 'pending';
	var value = null;

	function resolve(v) {
		value = v;
		state='resolve';
		if(deferred){
			handle(deferred);
		}
	}

	function handle(handler){
		if(!handler.onResolved){
	    	handler.resolve(value);
	    }else{
	    	handler.resolve(handler.onResolved(value))
	    }
	}

	this.then = function(onResolved) {
		if (state === 'pending') {
            deferred = handler;
            return;
		}
	    return new Task((resolve)=>{
	    	handle({
	    		onResolved,
	    		resolve
	    	});
	    });
	};

  	fn(resolve);
}

function main(){
	// create 
	let v = new Task((resolve) => {
		resolve(1);
	});

	// run
	v.then((v)=>{
		console.log(v);
	});
}

main();