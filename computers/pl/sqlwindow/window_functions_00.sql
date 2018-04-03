/**
 
 http://www.windowfunctions.com/questions/0

 A refresher on aggregate functions. You will need to know aggregate functions 
 before attempting the other questions. We would like to find the total weight 
 of cats grouped by age. But only return those group with a total weight larger 
 than 12.
	
 [table schema]:
 =======================
 name		varchar
 breed		varchar
 weight		float
 color		varchar
 age		int

 */

select * from (
  select 
  	age, 
  	sum(weight) 
  		as total_weight 
  from cats 
  group by age 
) as tmp 
where total_weight>12