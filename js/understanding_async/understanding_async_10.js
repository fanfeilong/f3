async function asyncReduce(array, handler, startingValue) {
  let result = startingValue;

  for (value of array) {
    // `await` will transform result of the function to the promise,
    // even it is a synchronous call
    result = await handler(result, value);
  }

  return result;
}

function createLinks(links) {
  return asyncReduce(
    links,
    // 异步回调
    async (resolvedLinks, link) => {
      return resolvedLinks.concat(link);
    },
    []
  );
}

const links = ['a', 'b'];
createLinks(links).then(v=>console.log(v));