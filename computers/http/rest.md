## what-is-idempotency-in-http-methods 
https://stackoverflow.com/questions/45016234/what-is-idempotency-in-http-methods

A request method is considered idempotent if the intended effect on the server of multiple identical requests with that method is the same as the effect for a single such request. And it's worthwhile to mention that idempotency is about the effect produced on the state of the resource on the server and not about the response status code received by the client.

To illustrate this, consider the DELETE method, which is defined as idempotent. Now consider a client performs a DELETE request to delete a resource from the server. The server processes the request, the resource gets deleted and the server returns 204. Then the client repeats the same DELETE request and, as the resource has already been deleted, the server returns 404.

