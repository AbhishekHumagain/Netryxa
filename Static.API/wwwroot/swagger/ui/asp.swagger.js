var asp = asp || {};
(function () {

    /* Swagger */

    asp.swagger = asp.swagger || {};

    asp.swagger.addAuthToken = function () {
        var authToken = asp.auth.getToken();
        if (!authToken) {
            return false;
        }

        var cookieAuth = new SwaggerClient.ApiKeyAuthorization(asp.auth.tokenHeaderName, 'Bearer ' + authToken, 'header');
        swaggerUi.api.clientAuthorizations.add('bearerAuth', cookieAuth);
        return true;
    }

    function loginUserInternal(callback) {
        var emailAddress = document.getElementById('emailAddress').value;
        if (!emailAddress) {
            alert('Username or Email Address is required, please try with a valid value !');
            return false;
        }

        var rememberClient = true;

        var password = document.getElementById('password').value;
        if (!password) {
            alert('Password is required, please try with a valid value !');
            return false;
        }

        var xhr = new XMLHttpRequest();

        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    // var responseJSON = JSON.parse(xhr.responseText);
                    // var result = responseJSON;
                    // var expireDate = new Date(Date.now() + (result.expireInSeconds * 1000));
                    // asp.auth.setToken(result.result.accessToken, expireDate);
                    callback();
                } else {
                    alert('Login failed !');
                }
            }
        };

     

        var apiUrl = '/api/auth/login';
        
        xhr.open('POST', apiUrl, true);
        xhr.setRequestHeader('Content-type', 'application/json');
        xhr.setRequestHeader('x-bypass-2fa', 'True');
        xhr.send(JSON.stringify({ emailAddress: emailAddress, password: password , rememberClient : rememberClient }));
    }

    asp.swagger.login = function (callback) {
        loginUserInternal(callback)
    };

    asp.swagger.logout = function () {
        asp.auth.clearToken();
    }

    asp.swagger.closeAuthDialog = function () {
        if (document.getElementById('asp-auth-dialog')) {
            document.getElementsByClassName("swagger-ui")[1].removeChild(document.getElementById('asp-auth-dialog'));
        }
    }

    asp.swagger.openAuthDialog = function (loginCallback) {
        asp.swagger.closeAuthDialog();

        var aspAuthDialog = document.createElement('div');
        aspAuthDialog.className = 'dialog-ux';
        aspAuthDialog.id = 'asp-auth-dialog';

        document.getElementsByClassName("swagger-ui")[1].appendChild(aspAuthDialog);

        // -- backdrop-ux
        var backdropUx = document.createElement('div');
        backdropUx.className = 'backdrop-ux';
        aspAuthDialog.appendChild(backdropUx);

        // -- modal-ux
        var modalUx = document.createElement('div');
        modalUx.className = 'modal-ux';
        aspAuthDialog.appendChild(modalUx);

        // -- -- modal-dialog-ux
        var modalDialogUx = document.createElement('div');
        modalDialogUx.className = 'modal-dialog-ux';
        modalUx.appendChild(modalDialogUx);

        // -- -- -- modal-ux-inner
        var modalUxInner = document.createElement('div');
        modalUxInner.className = 'modal-ux-inner';
        modalDialogUx.appendChild(modalUxInner);

        // -- -- -- -- modal-ux-header
        var modalUxHeader = document.createElement('div');
        modalUxHeader.className = 'modal-ux-header';
        modalUxInner.appendChild(modalUxHeader);

        var modalHeader = document.createElement('h3');
        modalHeader.innerText = 'Authorize';
        modalUxHeader.appendChild(modalHeader);

        // -- -- -- -- modal-ux-content
        var modalUxContent = document.createElement('div');
        modalUxContent.className = 'modal-ux-content';
        modalUxInner.appendChild(modalUxContent);

        modalUxContent.onkeydown = function (e) {
            if (e.keyCode === 13) {
                //try to login when user presses enter on authorize modal
                asp.swagger.login(loginCallback);
            }
        };

        //Inputs
        createInput(modalUxContent, 'emailAddress', 'Username or email address');
        createInput(modalUxContent, 'password', 'Password', 'password');

        //Buttons
        var authBtnWrapper = document.createElement('div');
        authBtnWrapper.className = 'auth-btn-wrapper';
        modalUxContent.appendChild(authBtnWrapper);

        //Close button
        var closeButton = document.createElement('button');
        closeButton.className = 'btn modal-btn auth btn-done button';
        closeButton.innerText = 'Close';
        closeButton.style.marginRight = '5px';
        closeButton.onclick = asp.swagger.closeAuthDialog;
        authBtnWrapper.appendChild(closeButton);

        //Authorize button
        var authorizeButton = document.createElement('button');
        authorizeButton.className = 'btn modal-btn auth authorize button';
        authorizeButton.innerText = 'Login';
        authorizeButton.onclick = function() {
            asp.swagger.login(loginCallback);
        };
        authBtnWrapper.appendChild(authorizeButton);
    }

    function createInput(container, id, title, type) {
        var wrapper = document.createElement('div');
        wrapper.className = 'wrapper';
        container.appendChild(wrapper);

        var label = document.createElement('label');
        label.innerText = title;
        wrapper.appendChild(label);

        var section = document.createElement('section');
        section.className = 'block-tablet col-10-tablet block-desktop col-10-desktop';
        wrapper.appendChild(section);

        var input = document.createElement('input');
        input.id = id;
        input.type = type ? type : 'text';
        input.style.width = '100%';

        section.appendChild(input);
    }

})();