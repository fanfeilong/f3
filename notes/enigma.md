- [enigma](http://enigma.media.mit.edu/enigma_full.pdf)
- [Homomorphic Encryption](https://en.wikipedia.org/wiki/Homomorphic_encryption)

shortcut of bitcoin:
--------------------
The intense verification and public nature of the blockchain limits potential use cases, however.
Modern applications use huge amounts of data, and run extensive analysis on that data.

enigma
---------
- private
- scalable
- storage
- privacy-enforcing computation
- heavy processing
- off-chain storage

1. DHT
2. sMPC

sMPC
---------
1. privacy(passive adversaries)

threshold cryptosystem

tc(t+1,n)
when n is the number of parties and t+1 is the minimal number of parties required to dectypt a secret encrypted with threshold encryption.

a linear secret-sharing scheme(LSSS) partitions a secret to shares such that the shares are a linear combination of the secret. 

Shamir's secret sharing(SSS) is an example of a LSSS.
q(x)=a_0+a_1*x+...+a_t*x^t
     a_0 = s,a_i~U(0,p-1)
the shares are then given by
     any i belong to {1,...,n}:[s]_pi = q(i)
then, given any t+1 shares, q(x) could be trivially reconstrcted using Lagrange interpolation and the secret s recovered using s=q(0). since SSS is linear, it is also additively homomorphic, so addtion and multiplication by a scalar operations could be performed directly on the shares without interaction:
     c  x s  = reconstruct({c x [s]_pi}_{i}^{t+1})
     s1 + s2 = reconstruct({[s1]_pi+[s2]_pi}_{i}^{t+1})
     s1 x s2 = ...

2. Corectness(mailicious adversaries)


#### **Baum**, SPDZ(speedz)

A protocol secure against mailicious adversaries(with dishonest majority), providing corectness guarantees for MPC.
In essence, the protocol is comprised of an expensive offline(preprocessing) step that uses **somewhat homomorphic encryption**(SHE) to generate shared randomness. Then, int the online stage, the computation is similar to the passive case and there is no expensive public-key cryptography involved. In the online stage, every share is represented by the additive share and its MAC, as follows:

<s>_pi = ([s]_pi, [r(s)]_pi), s.t r(s) = alpha*s 

where alpha is a fixed secret shared MAC key and <·> denotes the modified secret sharing scheme which is also additively homomorphic. <·>-sharing works without opening the shares of the global MAC key alpha, so it can be reused.

As before, multiplication is more involved. multiplication consumes {<a>,<b>,<c=ab>}, that are generated in the pre-processing step(many sush triplets are generated). then, given two secrets s1 and s2, that are share using <·>-sharing the product s = s1xs2 is achived by consuming a triplet as follows:

<s> = <c=ab> + e<b> + d<a> + ed,
e = <s1> - <a>,
d = <s2> - <b>

As mentioned, generating the triplets is an expensive process based on SHE.

Verification is achieved by solving

r − alpha * s = 0,

O(n^2) + against up to n-1 active adversaries

#### Publicly verifiable SPDZ

[[s]] = (<s>,<r>,<g^s*h^r>)

where s is the secret, r is a random value and c=g^s*h^r is the Pedersen commitment, with g,h serving as generators.
[[·]]-sharing perservers addtive homomorphic properties, and with a slightly modified mulitiplication protocol we can re-use the same idea of generating triplets({[[a]], [[b]], [[c=ab]]}) offline.


3. Hierarchical secure MPC







