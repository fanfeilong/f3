#lang racket
(define (make-point x y z)
  (list x y z))
(define (make-segment x y)
  (list x y))

;; get-seed:->(number->(number x number))
(define (get-seed)
  (lambda (seed)
    (cons seed seed)))

;; set-seed:number->(number->(void x number))
(define (set-seed new)
  (lambda (old)
    (cons (void) new)))

(define old ((get-seed) 10))
((set-seed 10) old)