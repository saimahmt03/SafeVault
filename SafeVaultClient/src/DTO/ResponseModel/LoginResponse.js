export class LoginResponse
{
    constructor(Username, Role, ApplicationName, Status)
    {
        this.Username = Username;
        this.Role = Role;
        this.ApplicationName = ApplicationName;
        this.Status = Status;
    }
}