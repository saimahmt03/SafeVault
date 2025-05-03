import React, { useState } from 'react';
import { User } from '../Model/User';
import { Service } from '../Service/Service';
import { Validator } from '../Shared/Validator';

const RegisterForm = () => {
    const [registerData, setRegisterData] = useState({
        firstname: '',
        lastname: '',
        email: '',
        username: '',
        password: '',
        type: 0, // 0 for user, 1 for admin
        applicationSignature: '',
    });

    // Initialize Service instance
    const service = new Service();
    const validator = new Validator();

    // Handle form input changes
    const handleChange = async (e) => {
        const { name, value } = e.target;
        setRegisterData((prevState) => ({
            ...prevState,
            [name]: value,
        }));
    };

    const handleUserTypeChange = async (event) => {
        const selectedValue = event.target.value;
        setRegisterData((prevState) => ({
            ...prevState,
            type: selectedValue === '1' ? 1 : 0, // Map 1 to admin, 0 to user
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        const user = new User(
            registerData.firstname,
            registerData.lastname,
            registerData.email,
            registerData.username,
            registerData.password,
            registerData.type, // Include the user type
            registerData.applicationSignature
        );

        // Validate the registration form data
        if (await validator.isEmptyRegisterForm(user)) {
            alert('All fields are required!');
        } else if (await validator.containsSpecialCharacters(user.firstname) || await validator.containsSpecialCharacters(user.lastname) || await validator.containsSpecialCharacters(user.email) || await validator.containsSpecialCharacters(user.username) || await validator.containsSpecialCharacters(user.password)) {
            alert('"!@#$%^&*?" characters are not allowed in firstname, lastname, email, username and password');
        } else if (!await validator.isValidXSSInput(user.firstname) || !await validator.isValidXSSInput(user.lastname) || !await validator.isValidXSSInput(user.email) || !await validator.isValidXSSInput(user.username) || !await validator.isValidXSSInput(user.password)) {
            alert('XSS attack detected!');
        } else {

            //var response = await service.AddNewUser(user);
            
            alert('Successfully registered!');
        }
    };

    return (
        <form onSubmit={handleSubmit} className="form">
            <h2>Register</h2>
            <div>
                <label>First Name:</label>
                <input 
                    type='text'
                    name='firstname'
                    value={registerData.firstname}
                    onChange={handleChange}
                    required
                    placeholder='Enter your first name'
                />
            </div>
            <div>
                <label>Last Name:</label>
                <input 
                    type='text'
                    name='lastname'
                    value={registerData.lastname}
                    onChange={handleChange}
                    required
                    placeholder='Enter your last name'
                />
            </div>
            <div>
                <label>Email:</label>
                <input 
                    type='email'
                    name='email'
                    value={registerData.email}
                    onChange={handleChange}
                    required
                    placeholder='Enter your email'
                />
            </div>
            <div>
                <label>User Type:</label>
                <select onChange={handleUserTypeChange} value={registerData.type}>
                    <option value="0">User</option>
                    <option value="1">Admin</option>
                </select>
            </div>
            <div>
                <label>Username:</label>
                <input 
                    type='text'
                    name='username'
                    value={registerData.username}
                    onChange={handleChange}
                    required
                    placeholder='Enter your username'
                />
            </div>
            <div>
                <label>Password:</label>
                <input 
                    type='password'
                    name='password'
                    value={registerData.password}
                    onChange={handleChange}
                    required
                    placeholder='Enter your password'
                />
            </div>
            <button type="submit">Register</button>
        </form>
    );
};

export default RegisterForm;