#lang racket
(define (rand seed)
  (let ([ans (modulo (* seed 16807) 2147483647)])
    (begin (set! seed ans)
           (cons ans ans))))
(define n (rand 1))
(rand (cdr n))

(define (make-point x y z)
  (cons x (cons y z)))

(define (rand-point seed)
  (let* ([r1 (rand seed)]
         [r2 (rand (cdr r1))]
         [r3 (rand (cdr r2))])
    (cons (make-point (car r1) (car r2) (car r3))
          (cdr r3))))
(define np (rand-point 2))
(rand-point (cdr np))

(define (make-segment x y)
  (cons x y))
(define (rand-segment seed)
  (let* ([r1 (rand-point seed)]
         [r2 (rand-point (cdr r1))])
    (cons (make-segment (car r1) (car r2))
          (cdr r2))))
(define ns (rand-segment 3))
(rand-segment (cdr ns))



