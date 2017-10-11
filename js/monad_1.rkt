#lang racket
(define (make-point x y z)
  (list x y z))
(define (make-segment x y)
  (list x y))

;; rand:->(number->(number x number))
(define (rand)
  (lambda (seed)
    (let ([ans (modulo (* seed 16807) 2147483647)])
      (cons ans ans))))

;; rand-point:->(number->(point x number))
(define (rand-point)
  (lambda (seed)
    (let* ([r1 ((rand) seed)]
            [r2 ((rand) (cdr r1))]
            [r3 ((rand) (cdr r2))])
      (cons (make-point (car r1) (car r2) (car r3))
            (cdr r2)))))

;; rand-segment:->(number->(segment x number))
(define (rand-segment)
  (lambda (seed)
    (let* ([r1 ((rand-point) seed)]
           [r2 ((rand-point) (cdr r1))])
      (cons (make-segment (car r1) (car r2))
            (cdr r2)))))

((rand-segment) 10)