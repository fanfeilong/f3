----------------------------------------
-- Lua FAQ
-- Collector: fanfeilong@outlook.com
-- Create Time: 2016-11-16
----------------------------------------

----------------------------------------
-- Q: how to sort dict 
-- A: use array indexs and dict
-- C: 
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