var Editar = function () {

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
                usr_nom: { required: true, sinEspacios: true },
                usr_ape_pat: { required: true },
                usr_car_cod: { min_cargo: true },
                usr_mail: { required: true, isEmail: true },
                usr_tel: {
                    required: true,
                    minlength: 9,
                    maxlength: 9
                }
            },
            messages: {
                usr_nom: {
                    required: "NOMBRE es requerido.",
                    sinEspacios: "NOMBRE es inválido."
                },
                usr_ape_pat: {
                    required: "APELLIDO PATERNO es requerido."
                },
                usr_car_cod: {
                    required: "CARGO es requerido.",
                    min_cargo: "CARGO es inválido."
                },
                usr_mail: {
                    required: "EMAIL es requerido.",
                    isEmail: "Favor ingrese un EMAIL válido."
                },
                usr_tel: {
                    required: "TELÉFONO es requerido.", matches: "[0-9]+",  minlength: "Favor ingrese un TELÉFONO válido.", maxlength: "Favor ingrese un TELÉFONO válido."
                }
            }
        });

        jQuery.validator.addMethod("sinEspacios", function (value, element) {

            let _largo = value.trim().length;
            if (_largo == 0) { return false } else { return true }
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
            size: 'large'
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
    Editar.init();
});

function fnc_isnull(str) {
    if (typeof str == 'undefined' || !str || str.length === 0 || str === "" || !/[^\s]/.test(str) || /^\s*$/.test(str) || str.replace(/\s/g, "") === "")
        return false;
    else
        return true;
}