local log = { _version = "0.1.0" }

log.usecolor = false
log.outfile = false
log.level = "debug"


local modes = {
    { name = "trace", color = "\27[34m", },
    { name = "debug", color = "\27[36m", },
    { name = "info", color = "\27[32m", },
	{ name = "datainfo", color = "\27[34m", },
    { name = "warn", color = "\27[33m", },
    { name = "error", color = "\27[31m", },
    { name = "fatal", color = "\27[35m", },
}


local levels = {}
for i, v in ipairs(modes) do
    levels[v.name] = i
end


local round = function(x, increment)
    increment = increment or 1
    x = x / increment
    return (x > 0 and math.floor(x + .5) or math.ceil(x - .5)) * increment
end

local _tostring = tostring

function table_print(tt, indent, done)
    done = done or {}
    indent = indent or 0
    if type(tt) == "table" then
        local sb = {}
        for key, value in pairs(tt) do
            table.insert(sb, string.rep(" ", indent)) -- indent it
            if type(value) == "table" and not done[value] then
                done[value] = true
                if "number" ~= type(key) then
                    table.insert(sb, key .. " = ")
                end
                table.insert(sb, "{\n");
                table.insert(sb, table_print(value, indent + 2, done))
                table.insert(sb, string.rep(" ", indent)) -- indent it
                table.insert(sb, "}\n");
            elseif "number" == type(key) then
                table.insert(sb, string.format("\"%s\"\n", _tostring(value)))
            else
                if "number" == type(value) then
                    table.insert(sb, string.format("%s = %s\n", _tostring(key), _tostring(value)))
                else
                    table.insert(sb, string.format("%s = \"%s\"\n", _tostring(key), _tostring(value)))
                end
            end
        end
        return table.concat(sb)
    else
        return tt .. "\n"
    end
end

local __tostring = function(...)
    local t = {}
    for i = 1, select('#', ...) do
        local x = select(i, ...)
        if type(x) == "number" then
            x = round(x, .01)
        end
        t[#t + 1] = _tostring(x)
    end
    return table.concat(t, " ")
end

function tostring(tbl)
    if "nil" == type(tbl) then
        return __tostring(nil)
    elseif "table" == type(tbl) then
        return table_print(tbl)
    elseif "string" == type(tbl) then
        return tbl
    else
        return __tostring(tbl)
    end
end


for i, x in ipairs(modes) do
    local nameupper = x.name:upper()
    log[x.name] = function(...)

        -- Return early if we're below the log level
        if i < levels[log.level]then
            return
        end

        local msg = tostring(...)
        local info = debug.getinfo(2, "nSl")
        local lineinfo = info.source .. ":" .. info.currentline
        local funcname = info.name
		
		if x.name == "datainfo" then
			io.write(msg)
			return
		end

        -- Output to console
        print(msg)

        -- Output to log file
        if log.outfile then
            local fp = io.open(log.outfile, "a")
            local str = string.format("[%-6s%s]%s[%s]: %s\n",
            nameupper,
            os.date("%H:%M:%S"),
            lineinfo,
            funcname,
            msg)
            fp:write(str)
            fp:close()
        end
    end
end

function string.split(str, delimiter)
	if str==nil or str=='' or delimiter==nil then
		return nil
	end
	
    local result = {}
    for match in (str..delimiter):gmatch("(.-)"..delimiter) do
        table.insert(result, string.upper(match))
    end
    return result
end

function torange(word)
	return {
		start = string.byte(word,1,1)-65,
		finish = string.byte(word,string.len(word),string.len(word))-65
	}
end

function get(text)
	local words = string.split(text,' ')

	local values = {}
	for i,w in ipairs( words ) do
		local r = torange(w)
		--print(r.start)
		--print(r.finish)
		local list = values[r.start]
		if not list then
			list = {}
			values[r.start] = list
		end
		table.insert(list,{
			word = w,
			range = r
		})
	end

	local index = {}
	for i, p in pairs( values ) do 
		table.insert(index,i)
	end
	table.sort(index)

	local points = {}
	for j,i in ipairs( index ) do
		table.insert(points,{
			pos = i,
			list = values[i]
		})
	end

	local flats = {}
	for i, v in ipairs( points ) do 
		for j, p in pairs( v.list ) do
			table.insert(flats,{
				pos = v.pos,
				list = {p}
			})
		end
	end

	--log.info(flats)

	--return flats

	---[[
	--log.info(#flats)
	for i=#flats,1,-1 do 
		--log.info(i)
		local v = flats[i]
		local pos = v.pos
		local first = v.list[1]
		--log.info(v)
		for j=i-1,1,-1 do
			local nv = flats[j]
			if hint(nv.list,v.list) then
				if cad(nv.list,v.list) then
					for l,r in ipairs(v.list) do
						table.insert(nv.list,r)
					end
				else 
					local r = cat(nv.list,v.list) 
					if r then
						log.info(r)
						nv.list = r
					end
				end
			end
		end
	end



	return flats
	--]]
end

function hint(left,right)
	for i,p in ipairs( left ) do
		for j,q in ipairs( right ) do
			if p.word==q.word then
				return false
			end
		end
	end

	return true
end

function  cad(left,right)
	local first = right[1]
	local last = left[#left]
	if first.range.start~=last.range.finish then
		return false
	end
	return true
end

function cat(left,right)
	local first = right[#right]
	local len = 0
	local result = {}

	for i=#left,1,-1 do
		len = len + 1
		local p = left[i]
		if p.start==first.start then
			if len<#right then
				for j=1,i do
					result[j] = left[j]
				end
				for j=1,#right do
					result[j] = right[j] 
				end
				return result
			end
		end
	end
	return nil
end


function test( )
	local text = "Apple Administrator reson effect need deny yet Zoo Elephant Under Fox Dog Leaf Tree"
	local r = get(text)
	for i, v in ipairs( r ) do 
		io.write('--------------\n')
		io.write(string.format('%s:',v.pos))
		for j, p in ipairs( v.list ) do
			io.write(string.format('[%s,%s] ',p.range.start,p.range.finish))
		end
		io.write('\n')
		io.write(string.format('%s:',v.pos))
		for j, p in ipairs( v.list ) do
			io.write(string.format('%s ',string.lower(p.word)))
		end
		io.write('\n--------------\n\n')
	end
end

test()
