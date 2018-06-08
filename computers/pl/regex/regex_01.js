// The World's Shortest Regex Compiler?: 
// https://jasonhpriestley.com/regex
//
// seems that typescript is a better choice, 
// missing the type information, 
// it is not easy to understand.
// 

// type: nfa
let zero = {
    done: () => false, 
    next: (chr) => new Set()
};

// type: nfa
let one = {
    done: () => true, 
    next: (chr) => new Set()
};

// type: (nfa,nfa)->nfa
let plus = (lnfa, rnfa) => ({
    done: () => lnfa.done() || rnfa.done(),
    next: (chr) => new Set([...lnfa.next(chr), ...rnfa.next(chr)])
});

// type: ()->nfa->nfa
// since there are so many token named as `next`,
// i think it is better to name the input argument as `nfa`
let eps = () => nfa => nfa;

// type: char->nfa->nfa
let lit = (chr) => nfa => ({
      done: () => false, 
      next: (ch) => new Set(ch === chr ? [nfa] : [])
});

// type: (nfa->nfa, nfa->nfa)->nfa->nfa
// type of l and r: nfa->nfa
let seq = (l,r) => nfa => l(r(nfa));

// type: (nfa->nfa, nfa->nfa)->nfa->nfa
// type of l and r: nfa->nfa
let alt = (l,r) => nfa => plus(l(nfa), r(nfa));

// type: char->nfa->nfa
// type of x: nfa->nfa
let star = (x) => nfa => {
    let cycle = false;

    let self = null;

    let back = {
        done: () => {
            if(cycle) return false;
            cycle = true;
            let result = self.done();
            cycle = false;
            return result;
        },
        next: (chr) => {
            if(cycle) return new Set([]);
            cycle = true;
            let result = self.next(chr);
            cycle = false;
            return result;
        }
    };

    let loop = plus(nfa,back);

    self = x(loop);

    return loop;
};

// transform characters of str into a linked list of nfa.
// infact, the result is a linked list of nfa, which is like:
// {
//    done: ture|false,
//    next: (char c)->{
//      match(c);
//      return [nfa]; // array of successor nfa
//    }
// }
// 
// when call the next method, we do:
// 1. match the character `c`
// 2. return an array of succesor nfa
// 
let parse = str => {

    // stack is type of: [item]
    // where item is type of: {alts:[nfa->nfa], next: nfa->nfa};
    let stack = [];

    //     {alts:[nfa->nfa, nfa->nfa], next: nfa->nfa}
    //  => {alts:[seq(nfa->nfa, nfa->nfa), nfa->nfa], next: null}
    //  => {alts:[nfa->nfa, nfa->nfa]}
    //  => [l:nfa->nfa, r:nfa->nfa]
    //  => nfa->plus(l(nfa),r(nfa))
    //
    let finalize = ({alts, next}) =>{
        let first = seq(alts[0], next);
        let rest = alts.slice(1);
        let union = [first, ...rest];
        let a = union.reduce(alt);
        return a;
    };

    //           [{alts:[nfa->nfa, nfa->nfa], next: nfa->nfa},{alts:[nfa->nfa, nfa->nfa], next: nfa->nfa},...]
    //        => [  ,{alts:[nfa->nfa, nfa->nfa], next: nfa->nfa},...]
    //   
    // finalize: first = nfa->plus(l(nfa),r(nfa))
    //   addSeq: [  ,{alts:[seq(nfa->nfa,nfa->nfa), nfa->nfa], next: first},...]
    //   addSeq: [  ,{alts:[nfa->nfa, nfa->nfa], next: nfa->nfa},...]
    //
    //        => [   {alts:[nfa->nfa, nfa->nfa], next: nfa->nfa},...]
    let pop = () => {
        addSeq(finalize(stack.shift()));
    };

    // [{alts:[nfa->nfa, nfa->nfa], next: nfa->nfa},...]
    // [{alts:[eps()], next: eps()},{alts:[nfa->nfa, nfa->nfa], next: nfa->nfa},...]
    // [{alts:[nfa=>nfa], next: nfa=>nfa},{alts:[nfa->nfa, nfa->nfa], next: nfa->nfa},...]
    let push = () => {
        stack.unshift({alts: [eps()], next: eps()});
    };

    //    [{alts:[nfa->nfa, nfa->nfa], next: nfa->nfa},...]
    // => [{alts:[seq(nfa->nfa,nfa->nfa), nfa->nfa], next: null},...]
    // => [{alts:[nfa->nfa, nfa->nfa], next: null},...]
    // => [{alts:[nfa->nfa, nfa->nfa], next: successor},...]
    // => [{alts:[nfa->nfa, nfa->nfa], next: nfa->nfa},...]
    let addSeq = (successor) => {
        stack[0].alts[0] = seq(stack[0].alts[0], stack[0].next);
        stack[0].next = successor;
    };

    //    [{alts:[nfa->nfa, nfa->nfa], next: nfa->nfa},...]
    // => [{alts:[seq(nfa->nfa,nfa->nfa) nfa->nfa], next: null},...]
    // => [{alts:[nfa->nfa, nfa->nfa], next: null},...]
    // => [{alts:[eps(), nfa->nfa, nfa->nfa], next: eps()},...]
    // => [{alts:[nfa=>nfa, nfa->nfa, nfa->nfa], next: nfa=>nfa},...]
    let addAlt = () => {
        stack[0].alts[0] = seq(stack[0].alts[0], stack[0].next);
        stack[0].alts.unshift(eps());
        stack[0].next = eps();
    };

    push();

    for(let chr of str.split('')) {
        if(chr === '*') stack[0].next = star(stack[0].next);
        else if(chr === ')') pop();
        else if(chr === '(') push();
        else if(chr === '|') addAlt();
        else addSeq(lit(chr));
    }

    let r = finalize(stack[0]);

    return r(one);
};

// as descripted at `parser`,
// when call the next method, we do:
// 1. match the character `c`
// 2. return an array of succesor nfa
// 
// then the `while` statements will 
// 3. combinate all the array of succesor nfa into a new nfa by 
//    reduce with `plus`, use `zero` nfa as first nfa
// 4. goto `1` to repeat.
// 
// from this point, the lined list of nfa is equals to a 
// sequence of assembly commands, the match is indeed a `assembly executor`.
//
let match = (nfa, str, idx=0) => {
    while(idx < str.length){
        nfa = [...nfa.next(str.charAt(idx++))].reduce(plus, zero);
    };
    return nfa.done();
};

// first, parse regex into a linked list nfa tree.
let regex = parse('ab|cd');

// second, use the linked list nfa tree to match the input string.
let m = match(regex,'cd');
