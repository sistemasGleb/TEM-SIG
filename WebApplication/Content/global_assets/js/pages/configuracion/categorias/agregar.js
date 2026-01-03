var Agregar = function () {
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
                doc_cat_nom: {
                    soloEspacios: true
                }
            },
            messages: {
                doc_cat_nom: {
                    required: "El NOMBRE DE BIBLIOTECA es inválido."
                }
            }
        });

        jQuery.validator.addMethod("soloEspacios", function (value, element) {

            let _largo = value.trim().length;
            if (_largo == 0) { return false } else { return true }
        }, "El NOMBRE DE BIBLIOTECA es inválido.");
    }

    return {
        init: function () {
            _componentBootstrapSwitch();
            _componentValidation();
        }
    }
}();

document.addEventListener('DOMContentLoaded', function () {
    Agregar.init();
});