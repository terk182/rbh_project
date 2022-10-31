var clearMemberModal = function () {

    $('#member-register-active').show();
    $('#member-register-complete').hide();
    $('#loginError').html('');
    $('#memberLoginEmail').val('');
    $('#memberLoginPassword').val('');
    $('#forgotError').html('');
    $('#memberForgotEmail').val('');
    $('#registerError').html('');
    $('#memberRegisterEmail').val('');
    $('#memberRegisterPassword').val('');
    $('#memberRegisterRePassword').val('');
};

var memberLogin = function () {
    clearMemberModal();
    $('#memberModal').modal('show');
    $('#member-login').show();
    $('#member-register').hide();
    $('#member-forgot').hide();
    $('#member-logout').hide();
};

var memberRegister = function () {
    clearMemberModal();
    $('#memberModal').modal('show');
    $('#member-login').hide();
    $('#member-register').show();
    $('#member-forgot').hide();
    $('#member-logout').hide();
};

var memberForgot = function () {
    clearMemberModal();
    $('#memberModal').modal('show');
    $('#member-login').hide();
    $('#member-register').hide();
    $('#member-forgot').show();
    $('#member-logout').hide();
};

var memberLogout = function () {
    clearMemberModal();
    $('#memberModal').modal('show');
    $('#member-login').hide();
    $('#member-register').hide();
    $('#member-forgot').hide();
    $('#member-logout').show();
};

var isEmail = function (email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

var fbLogin = function (accessToken) {
    $.get($('#memberURL').val() + '/FBLogin?accessToken=' + accessToken, function (data) {
        if (data != 'OK') {
            $('#loginError').html(data);
        } else {
            location.reload();
        }
    });
};

$(document).ready(function () {
    var memberURL = $('#memberURL').val();
    $('#memberLogin').click(function () {
        if ($('#memberLoginEmail').val() == '') {
            $('#loginError').html($('#memberLoginEmail').data('required'));
            $('#memberLoginEmail').focus();
            return;
        }
        if (!isEmail($('#memberLoginEmail').val())) {
            $('#loginError').html($('#memberLoginEmail').data('validate'));
            $('#memberLoginEmail').focus();
            return;
        }
        if ($('#memberLoginPassword').val() == '') {
            $('#loginError').html($('#memberLoginPassword').data('required'));
            $('#memberLoginPassword').focus();
            return;
        }

        $.get(memberURL + '/Login?username=' + $('#memberLoginEmail').val() + '&password=' + $('#memberLoginPassword').val(), function (data) {
            if (data != 'OK') {
                $('#loginError').html(data);
            } else {
                location.reload();
            }
        });
    });

    $('#fbLogin').click(function () {
        FB.getLoginStatus(function (response) {
            console.log(response.status);
            if (response.status === 'connected') {
                fbLogin(response.authResponse.accessToken);
            } else {
                FB.login(function (response) {
                    if (response.authResponse) {
                        fbLogin(response.authResponse.accessToken);
                    }
                });
            }
        });
    });


    $('#memberForgot').click(function () {
        if ($('#memberForgotEmail').val() == '') {
            $('#forgotError').html($('#memberForgotEmail').data('required'));
            $('#memberForgotEmail').focus();
            return;
        }
        if (!isEmail($('#memberForgotEmail').val())) {
            $('#forgotError').html($('#memberForgotEmail').data('validate'));
            $('#memberForgotEmail').focus();
            return;
        }

        $.get(memberURL + '/Forgot?username=' + $('#memberForgotEmail').val(), function (data) {
            $('#forgotError').html(data);
        });
    });

    $('#memberRegister').click(function () {
        if ($('#memberRegisterEmail').val() == '') {
            $('#registerError').html($('#memberRegisterEmail').data('required'));
            $('#memberRegisterEmail').focus();
            return;
        }
        if (!isEmail($('#memberRegisterEmail').val())) {
            $('#registerError').html($('#memberRegisterEmail').data('validate'));
            $('#memberRegisterEmail').focus();
            return;
        }
        if ($('#memberRegisterPassword').val() == '' || $('#memberRegisterPassword').val().length < 6) {
            $('#registerError').html($('#memberRegisterPassword').data('required'));
            $('#memberRegisterPassword').focus();
            return;
        }
        if ($('#memberRegisterPassword').val() != $('#memberRegisterRePassword').val()) {
            $('#registerError').html($('#memberRegisterRePassword').data('required'));
            $('#memberRegisterRePassword').focus();
            return;
        }
        if (!document.getElementById('memberAgree').checked) {
            $('#registerError').html($('#memberAgree').data('required'));
            $('#memberAgree').focus();
            return;
        }

        $.get(memberURL + '/Register?username=' + $('#memberRegisterEmail').val() + '&password=' + $('#memberRegisterPassword').val(), function (data) {
            if (data != 'OK') {
                $('#registerError').html(data);
            } else {
                $('#member-register-active').hide();
                $('#member-register-complete').show();
            }
        });
    });

    $('#memberLogout').click(function () {
        $.get(memberURL + '/Logout', function (data) {
            location.reload();
        });
    });
});
