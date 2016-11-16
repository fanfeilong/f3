var MAJORITY = 2;
function Acceptor() {
  this.promised = {};
  this.accepted = {};
  this.promise = function(key, ballot) {
    this.promised[key] = ballot;
    return ok(this.accepted[key]);
  };
  this.accept = function(key, ballot, value) {
    if (this.promised[key] <= ballot) {
      this.promised[key] = ballot;
      this.accepted[key] = {ballot: ballot, value: value};
      return ok();
    }
    return fail(this.promised[key]);
  };
}

function Proposer(acceptors, proposer_id, n) {
  this.proposer_id = proposer_id;
  this.n = n;
  this.execute = function(key, action, msg) {
    var ballot = 100*(++this.n) + this.proposer_id;
    proposers.update_ballot({promised_ballot: ballot});
    var q = acceptors.promise(key, ballot);
    q.on(x => x.is_fail).do(this.update_ballot.bind(this));
    var a = q.on(x => x.is_ok).at_least(MAJORITY).wait();
    var curr = a.max(x => x.accepted.ballot).accepted.value;
    var next = action(curr, msg);
    q = acceptors.accept(key, ballot, next);
    q.on(x => x.is_fail).do(this.update_ballot.bind(this));
    q.on(x => x.is_ok).at_least(MAJORITY).wait();
    return next;
  };
  this.update_ballot = function(fail) {
    this.n = Math.max(this.n, fail.promised_ballot / 100);
  };
}

function sign(value, general) {
  value.signOffs[general] = true;
  if (len(value.signOffs) == 2) {
    value.isSent = true;
  }
  return value;
}

function unsign(value, general) {
  if (value == null) {
    value = {signOffs: {}, isSent: false};
  }
  if (!value.isSent) {
    delete value.signOffs[general];
  }
  return value;
}

while (true) {
  try {
    var x = proposer.execute("ICBM", unsign, name);
    if (x.isSent) {
      console.info("LAUNCHED!");
    } else {
      x = proposer.execute("ICBM", sign, name);
      console.info(x.isSent ? "LAUNCHED":"STEADY");
    }
  } catch(e) {
    console.info(e);
  }
}