export class Validator {
    
    // Check username and password is empty or not
    async isEmptyLoginForm(user) {
        return new Promise((resolve) => {
            resolve(
                !user.username || user.username.trim() === "" && 
                !user.password || user.password.trim() === ""
            );
        });
    }

    // Check user input is empty or not
    async isEmptyRegisterForm(user) {
        return new Promise((resolve) => {
            resolve(
                !user.firstname || user.firstname.trim() === "" && 
                !user.lastname || user.lastname.trim() === "" && 
                !user.email || user.email.trim() === "" && 
                !user.username || user.username.trim() === "" && 
                !user.password || user.password.trim() === ""
            );
        });
    }

    // Check username and password contains special characters
    allowedSpecialCharacters = ['!', '#', '$', '%', '^', '&', '*', '?'];

    async containsSpecialCharacters(input) {
        return new Promise((resolve) => {
            for (let i = 0; i < input.length; i++) {
                if (this.allowedSpecialCharacters.includes(input[i])) {
                    resolve(true);
                    return;
                }
            }
            resolve(false);
        });
    }

    // Check for XSS Attacks
    async isValidXSSInput(input) {
        return new Promise((resolve) => {
            if (!input || input.trim() === '') {
                resolve(true);
            } else if (input.toLowerCase().includes('<script') || input.toLowerCase().includes('<iframe')) {
                resolve(false);
            } else {
                resolve(true);
            }
        });
    }
}