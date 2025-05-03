import React from 'react';
import { HandleResponse } from '../Shared/HandleResponse';

export class Service {
    constructor() {
        this.baseUrl = 'http://localhost:5222/safevault/';
        this.handleResponse = new HandleResponse();
    }

    async GetAPIToken(Login) {
        // try {
        //     const response = await fetch(`${this.baseUrl}login`, {
        //         method: 'POST',
        //         headers: { 'Content-Type': 'application/json' },
        //         body: JSON.stringify(Login),
        //     });

        //     const data = await this.handleResponse.handleResponse(response);

        //     if (
        //         data === 'Something went wrong.' ||
        //         data === 'Resource not found.' ||
        //         data === 'Server error, please try again later.'
        //     ) {
        //         return data;
        //     }

        //     localStorage.setItem('authToken', data);

        //     return data;
        // } catch (error) {
        //     console.error('Login Error:', error.message);
        //     throw error;
        // }

        const response = await fetch(`${this.baseUrl}login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(Login),
        });
    
        const data = await this.handleResponse.handleResponse(response);
    
        if (
            data === 'Something went wrong.' ||
            data === 'Resource not found.' ||
            data === 'Internal error.' ||
            data === 'User not found.' ||
            data === 'Unauthorized access.'
        ) {
            return data;
        }
    
        localStorage.setItem('authToken', data);
    
        return data;

    }
}