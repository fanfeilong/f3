/**
 
 http://www.windowfunctions.com/questions/2

 The cats must by ordered first by breed and second by name. They are about to 
 enter an elevator one by one. When all the cats of the same breed have entered 
 they leave. We would like to know what the running total weight of the cats is.

 [table schema]:
 =======================
 name		varchar
 breed		varchar
 weight		float
 color		varchar
 age		int

 */

select 
  name, 
  breed, 
  sum(weight) 
     over( 
     	partition by breed       -- use partition by to move window
     	order by ( breed, name) 
     ) 
     as running_total_weight
from cats order by ( breed, name)
