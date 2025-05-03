export class HandleResponse
{
    // Helper method to handle API responses and errors
    async handleResponse(response) {
        const data = await response.json();

        if (response.ok) {
            return data;
        }

        if(response.status === 400){
            return 'User not found.';
        }

        if(response.status === 401){
            return 'Unauthorized access.';
        }

        if (response.status === 404) {
            return 'Resource not found.';
        }

        if (response.status === 500) {
            return 'Internal error.';
        }

        return 'Something went wrong.';
    }
}