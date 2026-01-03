var Agregar = function () {

    var _componentValidation = function () {
        if (!$().validate) {
            console.warn('Warning - validate.min.js is not loaded.');
            return;
        }

        var validator = $('.form-validate').validate({
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
                else {
                    error.insertAfter(element);
                }
            },
            rules: {
                usr_rut_com: { required: true, rut: true },
                usr_lgn: {
                    required: true,
                    minlength: 5,
                    maxlength: 15,
                    remote: {
                        url: $("#hddn_Valid_User").val(),
                        type: "post",
                        dataType: "json",
                        data: {
                            username: function () { return $("#usr_lgn").val(); }
                        }
                    }
                },
                usr_car_cod: { min_cargo: true },
                usr_nom: { required: true, sinEspacios: true },
                usr_ape_pat: { required: true },
                usr_mail: { required: true, isEmail: true },
                usr_tel: {
                    required: true,
                    minlength: 9,
                    maxlength: 9
                },
                usr_psw: {
                    required: true,
                    minlength: 8,
                    maxlength: 12,
                    pwdcheck: true
                },
                usr_new_psw: {
                    required: true,
                    minlength: 8,
                    maxlength: 12,
                    equalTo: '[name="usr_psw"]',
                    pwdcheck: true
                }
            },
            messages: {
                usr_rut_com: { required: "El RUT es requerido." },
                usr_lgn: {
                    required: "USUARIO es requerido.",
                    minlength: "Favor ingrese un USUARIO válido. (Min 5 Caracteres)",
                    maxlength: "Favor ingrese un USUARIO válido. (Max 15 Caracteres)",
                    remote:"El USUARIO ya existe"
                },
                usr_car_cod: {
                    required: "CARGO es requerido.",
                    min_cargo: "CARGO es inválido."
                },
                usr_nom: {
                    required: "NOMBRE es requerido.",
                    sinEspacios: "NOMBRE es inválido."
                },
                usr_ape_pat: {
                    required: "APELLIDO PATERNO es requerido."
                },
                usr_mail: {
                    required: "EMAIL es requerido.",
                    isEmail: "Favor ingrese un EMAIL válido."
                },
                usr_tel: {
                    required: "TELÉFONO es requerido.", minlength: "Favor ingrese un TELÉFONO válido.", maxlength: "Favor ingrese un TELÉFONO válido."
                },
                usr_psw: {
                    required: "CONTRASEÑA es requerido.",
                    minlength: "CONTRASEÑA debe tener minimo 8 caracteres.",
                    maxlength: "CONTRASEÑA debe tener máximo 12 caracteres.",
                    pwdcheck:"CONTRASEÑA debe contener una letra mayúscula, un carácter numérico y un carácter especial"
                },
                usr_new_psw: {
                    required: "CONFIRMAR CONTRASEÑA es requerido.",
                    minlength: "CONFIRMAR CONTRASEÑA  debe tener minimo 8 caracteres.",
                    maxlength: "CONFIRMAR CONTRASEÑA debe tener máximo 12 caracteres.",
                    pwdcheck: "CONFIRMAR CONTRASEÑA debe contener una letra mayúscula, un carácter numérico y un carácter especial",
                    equalTo:"Ambas contraseñas deben ser iguales"
                }
            }
        });

        jQuery.validator.addMethod("pwdcheck", function (value, element) {

            console.log(value);
            return this.optional(element) || /^(?=.*\d)(?=.*[A-Z])(?=.*\W).*$/i.test(value);
        });

        jQuery.validator.addMethod("rut", function (value, element) {
            return this.optional(element) || $.Rut.validar(value);
        }, 'El RUT ingresado no es válido');

        jQuery.validator.addMethod("sinEspacios", function (value, element) {

            console.log(value.indexOf(" "));

            return value.indexOf(" ") < 0 && value != "";
        }, "El texto no puede tener espacios en blanco.");

        jQuery.validator.addMethod("min_cargo", function (value, element) {

            if (!fnc_isnull(value)) {
                return false;
            }

            if (parseInt(value) == 0) {
                return false;
            } else {
                return true;
            }
        });
        jQuery.validator.addMethod("isEmail", function (value, element) {

            if (!fnc_isnull(value)) {
                return false;
            }

            var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            return regex.test(value);
        });

        //jQuery.validator.addMethod("isValiduser", function (value, element) {

        //    if (!fnc_isnull(value)) {
        //        return false;
        //    }

        //    let Root = { user: value };

        //    //Call Ajax Method
        //    $.ajax({
        //        url: $("#hddn_Valid_User").val(),
        //        type: "POST",
        //        data: JSON.stringify(Root),
        //        dataType: "json",
        //        contentType: "application/json"
        //    }).done(function (response) {

        //        console.log(JSON.stringify(response));


        //        return response;
 
        //    }).fail(function (jqXHR, textStatus) {
        //        console.log("fnc_validar_usuario / Request failed: " + textStatus);
        //    }).always(function () {

        //    });
        //    // Fin Call Ajax Method
        //});
    }

    var _componentSelect2 = function () {
        if (!$().select2) {
            console.warn('Warning - select2.min.js is not loaded.');
            return;
        }

        $('.select2').select2();

        $('#usr_car_cod').on('select2:select', function (e) {
            $("#usr_car_cod-error").remove();
        });

    };

    var _componentBootstrapSwitch = function () {
        if (!$().bootstrapSwitch) {
            console.warn('Warning - switch.min.js is not loaded.');
            return;
        }
        $('.form-check-input-switch').bootstrapSwitch({
            onText: 'Sí',
            onColor: 'success',
            offText: 'No',
            offColor: 'danger',
            size: 'small'
        });

        $('.form-check-input-switch').bootstrapSwitch();
    };


    return {
        init: function () {
            _componentSelect2();
            _componentBootstrapSwitch();
            _componentValidation();
        }
    }
}();

document.addEventListener('DOMContentLoaded', function () {
    Agregar.init();
});

function fnc_isnull(str) {
    if (typeof str == 'undefined' || !str || str.length === 0 || str === "" || !/[^\s]/.test(str) || /^\s*$/.test(str) || str.replace(/\s/g, "") === "")
        return false;
    else
        return true;
}

function fnc_viewPassword() {


    if ('password' == $('#usr_psw').attr('type')) {
        $('#usr_psw').prop('type', 'text');
        $('#txt_icon_pass').removeClass('icon-eye-blocked').addClass('icon-eye');
    } else {
        $('#usr_psw').prop('type', 'password');
        $('#txt_icon_pass').removeClass('icon-eye-blocked').addClass('icon-eye-blocked');
    }
}
function fnc_viewConfirmPassword() {

    if ('password' == $('#usr_new_psw').attr('type')) {
        $('#usr_new_psw').prop('type', 'text');
        $('#txt_icon_confirmpass').removeClass('icon-eye-blocked').addClass('icon-eye');
    } else {
        $('#usr_new_psw').prop('type', 'password');
        $('#txt_icon_confirmpass').removeClass('icon-eye-blocked').addClass('icon-eye-blocked');
    }
}
