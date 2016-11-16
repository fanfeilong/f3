----------------------------------------
-- Lua FAQ
-- Collector: fanfeilong@outlook.com
-- Create Time: 2016-11-16
----------------------------------------

----------------------------------------
-- Q: how to sort dict (1)
-- A: use array indexs and dict
-- C: 1. sort before loop
--    2. inverse remove indexes after loop
----------------------------------------
local indexs = {}
local dict = {}

-- insert
dict[10] = "how"
table.insert(indexs,10)

dict[30] = "to"
table.insert(indexs,30)

dict[15] = "sort"
table.insert(indexs,15)

dict[45] = "dict"
table.insert(indexs,15)

-- loop
for i,key in ipairs(indexs) do
	local value = dict[key]
	print(string.format("<%s,%s>",i,v)) 
end

-- sort
table.sort(indexs)
for i,key in ipairs(indexs) do
	local value = dict[key]
	print(string.format("<%s,%s>",i,v)) 
end

-- remove
local removeIndexs = {}
table.sort(indexs)
for i,key in ipairs(indexs) do
	local value = dict[key]
	if value=="how" or value=="dict" then
		dict[key] = nil
		table.insert(removeIndexs,i)
	end
end

for j=#removeIndexs,1,-1 do
	table.remove(indexs,removeIndexs[j])
end

----------------------------------------
-- Q: how to sort dict (2)
-- A: use binary indexs and dict
-- C: http://lua-users.org/wiki/OrderedAssociativeTable
----------------------------------------
t2= table.ordered( "reverse" )
t2["A"] = 1
t2.B = 2
t2.C = 3
t2.D = 4
t2.E = 5
t2.F = 6
t2.G = 7
t2.H = 8

print("Ordered Iteration of Reverse")
for i,index,v in orderedPairs(t2) do
  print(index, v)
end

print("Set one Letter nil")
t2.E = nil
print("Ordered Iteration of Reverse")
for i,index,v in orderedPairs(t2) do
  print(index, v)
end

print("Update one value")
t2.F = "updated"
print("Ordered Iteration of Reverse")
for i,index,v in orderedPairs(t2) do
  print(index, v)
end

print("Add with a no confirm key")
-- will simply be not added
t2[6] = "add"

print( "Add a key" )
t2.a = "new key1"
t2.Z = "new key 2"
t2.d = "??"

print( "Ordered Iteration of Reverse" )
for i,index,v in orderedPairs( t2 ) do
	print( index, v )
end

-- get a key
print( t2.Z )