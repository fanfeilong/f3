/**
 
 http://www.windowfunctions.com/questions/1

 The cats must by ordered by name and will line up about to enter an elevator 
 one by one. We would like to know what the running total weight is.

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
	sum(weight) 
		over (order by name)     --over clause to partial accumulate
		as running_total_weight 
from cats 
order by name
