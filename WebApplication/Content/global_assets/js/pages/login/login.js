var Login = function () {

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
                else if (element.parents().hasClass('multiselect-native-select')) {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.insertAfter(element);
                }
            },
            rules: {
                Usuario: { required: true },
                Password: { required: true }
            },
            messages: {
                Usuario: { required: "Usuario es requerido." },
                Password: { required: "Contraseña es requerido." },
            }
        });
    };
    return {
        init: function () {
            _componentValidation();
        }
    }
}();


document.addEventListener('DOMContentLoaded', function () {
    Login.init();
});
 
//function CloseModalPopup() {
//    $('#modal_login').modal().hide();
//}

function fnc_viewPassword() {


    if ('password' == $('#Password').attr('type')) {
        $('#Password').prop('type', 'text');
        $('#txt_icon_pass').removeClass('icon-eye-blocked').addClass('icon-eye');
    } else {
        $('#Password').prop('type', 'password');
        $('#txt_icon_pass').removeClass('icon-eye-blocked').addClass('icon-eye-blocked');
    }
}