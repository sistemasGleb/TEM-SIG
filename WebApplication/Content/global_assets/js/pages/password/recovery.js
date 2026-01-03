// Setup module
// -----------------------------
var Recovery = function () {

    var _componentValidation = function () {
        if (!$().validate) {
            console.warn('Warning - validate.min.js is not loaded.');
            return;
        }

        var validator = $('.form-validate-recovery').validate({
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
                recovery_user_email: { required: true }
            },
            messages: {
                recovery_user_email: { required: "Este campo es requerido.", email:"Por favor, introduce una dirección de correo electrónico válida." }
            },
            submitHandler: function (form) {
                //si todos los controles cumplen con las validaciones, se ejecuta este codigo
                fnc_recovery();
                // fin Metodo
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
    Recovery.init();
});

// Functions module
// -----------------------------

function fnc_Send()
{
    $("#form_recovery").submit();
}

function fnc_recovery() {

    $("#cargando").fadeIn();

    let Root = {
        "recovery_user_email": $('#recovery_user_email').val(),
    };
 
    $.ajax({
        url: $('#hddn_PasswordRequest').val(),
        type: "POST",
        data: JSON.stringify(Root),
        dataType: "json",
        contentType: "application/json"
    }).done(function (response) {

        if (response.status)
        {
            let swalInit = swal.mixin({
                buttonsStyling: false,
                confirmButtonClass: 'btn btn-primary',
                cancelButtonClass: 'btn btn-light'
            });

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
                    $("#recovery_user_email").val('');
                    window.location.href = $("#hddn_Login").val();
                }
            });
        } else {
            let swalInit = swal.mixin({
                buttonsStyling: false,
                confirmButtonClass: 'btn btn-primary',
                cancelButtonClass: 'btn btn-light'
            });

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
                    $("#recovery_user_email").val('');
                }
            });
        }
    }).fail(function (jqXHR, status) {
        console.log('fnc_recovery / Request failed:' + status);
    }).always(function (jqXHR, status) {
        $("#cargando").fadeOut();
    });
}