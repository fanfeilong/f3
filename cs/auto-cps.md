

function calc(l,r){
	return l+r;
}

function vist(tree){
	if(tree==null){
		return 0;
	}else{
		var l = vist(tree.left);
		var r = vist(tree.right);
		return calc(l,r);
	}
}

function vist(tree, k){
	if(tree==null){
		k(0);
	}else{
		// var l = vist(tree.left);
		// var r = visit(tree.right);
		k(calc(l,r));
	}
}

function visit(tree,k){
	if(tree==null){
		k(0);
	}else{
		// var l = vist(tree.left);
		function kRight(r){
			k(calc(l,r));
		}
		visit(tree.right, kRight);
	}
}


function visit(tree, k){
	if(tree==null){
		k(0);
	}else{
		function kLeft(l){
			function kRight(r){
				k(calc(l,r));
			}
			visit(tree.right, kRight);
		}
		visit(tree.left, kLeft);
	}
}

function visit(args){
	if(args.tree==null){
		args.k(0);
	}else{
		function kLeft(l){
			function kRight(r){
				args.k(calc(l,r));
			}
			visit(args.tree.right, kRight);
		}
		visit(args.tree.left, kLeft);
	}
}



var continuation = null;
function createContinuation(newFun,newArgs){
	continuation = {func: newFun, args: newArgs};
}

function visit(args){
	if(args.tree==null){
		createContinuation(k, 0);
	}else{
		function kLeft(l){
			function kRight(r){
				//args.k(calc(l,r));
				createContinuation(k, calc(l,r));
			}
			//visit(args.tree.right, kRight);
			createContinuation(visit, {tree:args.tree.right,k:kRight});
		}
		//visit(args.tree.left, kLeft);
		createContinuation(visit, {tree:args.tree.left,k:kLeft});
	}
}

function runIterate(){
	while(continuation!=null){
		var f = continuation.func;
		var args = continuation.args;
		continuation = null;
		f(args);
	}
}

function runRecursive(){
	if(continuation){
		var f = continuation.func;
		var args = continuation.args;
		continuation = null;
		f(args);
		runRecursive();
	}
}

function iterate(){
	yield return 1;
	yield return 2;
}

class Iterator{
	constructor(){
		this.state = -1;
		this.values = [1,2];
	}

	reset(){
		this.state = 1;
		this.i = -1;
		this.current = null;
	}

	current(){
		return this.current;
	}

	next(){
		switch(this.state){
			case 1:
				this.i = 0;
				this.current = this.values[this.i];
				this.state = 2;
				return true;
			case 2:
				this.i++;
				if(this.i<=values.length){
					this.current = this.values[this.i];
					return true;
				}else{
					return false;
				}
			default:
				break;
		}
	}
}









