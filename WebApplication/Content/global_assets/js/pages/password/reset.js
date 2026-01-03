var Reset = function () {

    var _componentValidation = function () {
        if (!$().validate) {
            console.warn('Warning - validate.min.js is not loaded.');
            return;
        }

        var validator = $('.form-validate-reset').validate({
            ignore: 'input[type=hidden], .select2-search__field', // ignore hidden fields
            errorClass: 'validation-invalid-label',
            successClass: 'validation-valid-label',
            validClass: 'validation-valid-label',
            highlight: function (element, errorClass) {
                $(element).removeClass(errorClass);
            },
            unhighlight: function (element, errorClass) {
                $(element).removeClass(errorClass);
            },
            errorPlacement: function (error, element) {
                if (element.parents().hasClass('form-check')) {
                    error.appendTo(element.parents('.form-check').parent());
                }
                else if (element.parents().hasClass('form-group-feedback') || element.hasClass('select2-hidden-accessible')) {
                    error.appendTo(element.parent());
                }
                else if (element.parent().is('.uniform-uploader, .uniform-select') || element.parents().hasClass('input-group')) {
                    error.appendTo(element.parent().parent());
                }
                else if (element.parents().hasClass('multiselect-native-select')) {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.insertAfter(element);
                }
            },
            rules: {
                recovery_new_pswd: {
                    required: true,
                    minlength: 8,
                    maxlength: 12,
                    pwdcheck: true
                },
                recovery_confirm_pswd: {
                    required: true,
                    minlength: 8,
                    maxlength: 12,
                    equalTo: '[name="recovery_new_pswd"]',
                    pwdcheck: true
                }
            },
            messages: {
                recovery_new_pswd: {
                    required: "Este campo es requerido. es requerido.",
                    minlength: "Este campo es requerido. debe tener minimo 8 caracteres.",
                    maxlength: "Este campo es requerido. debe tener máximo 12 caracteres.",
                    pwdcheck: "Este campo debe contener una letra mayúscula, un carácter numérico y un carácter especial"
                },
                recovery_confirm_pswd: {
                    required: "Este campo es requerido. es requerido.",
                    minlength: "Este campo es requerido.  debe tener minimo 8 caracteres.",
                    maxlength: "Este campo es requerido. debe tener máximo 12 caracteres.",
                    pwdcheck: "Este campo debe contener una letra mayúscula, un carácter numérico y un carácter especial",
                    equalTo: "Ambas contraseñas deben ser iguales"
                }
            },
            submitHandler: function (form) {
                //si todos los controles cumplen con las validaciones, se ejecuta este codigo
                fnc_reset();
                // fin Metodo
            }
        });
        jQuery.validator.addMethod("pwdcheck", function (value, element) {
 
            return this.optional(element) || /^(?=.*\d)(?=.*[A-Z])(?=.*\W).*$/i.test(value);
        });
    };

    return {
        init: function () {
            _componentValidation();
        }
    }
}();

document.addEventListener('DOMContentLoaded', function () {
    Reset.init();
});

// Functions module
// -----------------------------

function fnc_SendReset() {
    $("#form_reset_password").submit();
}

function fnc_viewPassword() {


    if ('password' == $('#recovery_new_pswd').attr('type')) {
        $('#recovery_new_pswd').prop('type', 'text');
        $('#txt_icon_pass').removeClass('icon-eye-blocked').addClass('icon-eye');
    } else {
        $('#recovery_new_pswd').prop('type', 'password');
        $('#txt_icon_pass').removeClass('icon-eye-blocked').addClass('icon-eye-blocked');
    }
}
function fnc_viewConfirmPassword() {

    if ('password' == $('#recovery_confirm_pswd').attr('type')) {
        $('#recovery_confirm_pswd').prop('type', 'text');
        $('#txt_icon_confirmpass').removeClass('icon-eye-blocked').addClass('icon-eye');
    } else {
        $('#recovery_confirm_pswd').prop('type', 'password');
        $('#txt_icon_confirmpass').removeClass('icon-eye-blocked').addClass('icon-eye-blocked');
    }
}

function fnc_reset() {

    $("#cargando").fadeIn();

    let swalInit = swal.mixin({
        buttonsStyling: false,
        confirmButtonClass: 'btn btn-primary',
        cancelButtonClass: 'btn btn-light'
    });

    let _recovery_user_lgn = $('#hddn_user_lgn').val();
    let _recovery_token = $('#hddn_guid').val();
    let _recovery_new_pswd = $('#recovery_new_pswd').val();
    let _recovery_confirm_pswd = $('#recovery_confirm_pswd').val();

    let oDatos = {
        "recovery_user_lgn": _recovery_user_lgn,
        "recovery_token": _recovery_token,
        "recovery_new_pswd": _recovery_new_pswd,
        "recovery_confirm_pswd": _recovery_confirm_pswd
    };
 
    $.ajax({
        url: $('#hddn_PasswordReset').val(),
        cache: false,
        type: 'POST',
        async: false,
        data: oDatos,
    }).done(function (response) {
 
        if (response.status) {
 
            swalInit.fire({
                title: response.title,
                html: response.message,
                type: 'success',
                showCancelButton: false,
                confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
                confirmButtonClass: 'btn btn-primary',
                buttonsStyling: false
            }).then(function (result) {
                if (result.value) {
                    $("#recovery_new_pswd").val('');
                    $("#recovery_confirm_pswd").val('');
                    window.location.href = $("#hddn_Login").val();
                }
            });
        } else {
 
            swalInit.fire({
                title: 'Restablecer contraseña',
                html: 'ERROR :' + response.message,
                type: 'error',
                showCancelButton: false,
                confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
                confirmButtonClass: 'btn btn-primary',
                buttonsStyling: false
            }).then(function (result) {
                if (result.value) {
                    $("#recovery_new_pswd").val('');
                    $("#recovery_confirm_pswd").val('');
                    window.location.href = $("#hddn_Login").val();
                }
            });

        }

    }).fail(function (jqXHR, status) {
        console.log('fnc_reset / Request failed:' + status);
    }).always(function (jqXHR, status) {
        $("#cargando").fadeOut();
    });
}