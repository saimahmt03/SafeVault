export class User 
{
    constructor(firstname, lastname, email, username, password, type, applicationSignature) 
    {
        this.firstname = firstname;
        this.lastname = lastname;
        this.email = email;
        this.username = username;
        this.password = password;
        this.type = type; // 0 for user, 1 for admin
        this.applicationSignature = applicationSignature;
    } 
}   

