import React, { useState } from 'react';
import { User } from '../Model/User';
import { Login } from '../DTO/RequestModel/Login';
import { Service } from '../Service/Service';
import { Validator } from '../Shared/Validator';

const LoginForm = () => {
    const [loginData, setLoginData] = useState({
        username: '',
        password: '',
    });

    const service = new Service();
    const validator = new Validator();

    const handleChange = async (e) => {
        const { name, value } = e.target;
        setLoginData((prevState) => ({
            ...prevState,
            [name]: value,
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        // Create a User instance to validate form data
        const user = new User('', '', '', loginData.username, loginData.password);

        // Validation checks
        if (await validator.isEmptyLoginForm(user)) {
            alert('Username and password cannot be empty');
        } else if (await validator.containsSpecialCharacters(user.username) || await validator.containsSpecialCharacters(user.password)) {
            alert('"!@#$%^&*?" characters are not allowed in username and password');
        } else if (!await validator.isValidXSSInput(user.username) || !await validator.isValidXSSInput(user.password)) {
            alert('XSS attack detected!');
        } else {
            
            // Create a Login instance
            const login = new Login(loginData.username, loginData.password);

            const response = await service.GetAPIToken(login);

            if(
                response === 'Something went wrong.' ||
                response === 'Resource not found.' ||
                response === 'Internal error.' ||
                response === 'User not found.' ||
                response === 'Unauthorized access.'
            ){
                alert(response);
            }
            else{
                alert(response + 'Successfully logged in!');   
            }
        }
    };

    return (
        <form onSubmit={handleSubmit} className="form">
            <h2>Login</h2>
            <div>
                <label>Username:</label>
                <input 
                    type="text"
                    name="username"
                    value={loginData.username}
                    onChange={handleChange}
                    required
                    placeholder="Enter your username"
                />
            </div>
            <div>
                <label>Password:</label>
                <input 
                    type="password"
                    name="password"
                    value={loginData.password}
                    onChange={handleChange}
                    required
                    placeholder="Enter your password"
                />
            </div>
            <button type="submit">Login</button>
        </form>
    );
};

export default LoginForm;