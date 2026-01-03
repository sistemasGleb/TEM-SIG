var SubirImagen = function () {

    var _componentUniform = function () {
        if (!$().uniform) {
            console.warn('Warning - uniform.min.js is not loaded.');
            return;
        }
        $('.form-control-uniform').uniform({
            fileButtonHtml: "Examinar",
            fileDefaultHtml: "FAVOR SELECCIONE UN ARCHIVO   ",
            fileClass: "uniform-uploader col-form-label-sm"
        });
    };
    var _componentFileUpload = function () {
        if (!$().fileinput) {
            console.warn('Warning - fileinput.min.js is not loaded.');
            return;
        }

        $("#file_mot_adj").fileinput({
            language: 'es',
            allowedFileExtensions: JSON.parse($("#hiddenAllowedFileExtension").val()),
            showUpload: false,
            showRemove: false,
            uploadUrl: $("#hiddenUploadFile").val(),
            uploadAsync: false,
            overwriteInitial: false,
            minFileCount: 1,
            maxFileCount: JSON.parse($("#hiddenMaxFileCount").val()),
            maxLength: 150,
            maxFileSize: JSON.parse($("#hiddenMaxFileSize").val()),
            initialPreviewAsData: true,
            browseIcon: '<i class="icon-file-plus mr-2"></i>',
            browseClass: "btn btn-sm btn-primary",
            showCaption: false,
            mainClass: "d-grid",
            uploadExtraData: function (previewId, index) {
                var info = {
                    "id_tipo_elemento": $("#id_tipo_elemento").val(),
                    "id_elemento": $("#id_elemento").val(),
                    "strObservacion": $("#observacion_imagen_elemento").val()
                };
                return info;
            },
            fileActionSettings: {
                showZoom: false,
                showUpload: false,
                removeIcon: '<i class="icon-bin"></i>',
                removeClass: '',
                downloadIcon: '<i class="icon-download"></i>',
                downloadClass: '',
                uploadIcon: '<i class="icon-upload"></i>',
                uploadClass: '',
                indicatorNew: '<i class="icon-file-plus text-success"></i>',
                indicatorSuccess: '<i class="icon-checkmark3 file-icon-large text-success"></i>',
                indicatorError: '<i class="icon-cross2 text-danger"></i>',
                indicatorLoading: '<i class="icon-spinner2 spinner text-muted"></i>'
            }
        }).on('filebatchuploadsuccess', function (event, data) {
            var response = data.response;

            console.log(JSON.stringify(response));

            let swalInit = swal.mixin({
                buttonsStyling: false,
                confirmButtonClass: 'btn btn-primary',
                cancelButtonClass: 'btn btn-light'
            });

            if (response.error == 'error') {
                //error
                swalInit.fire({
                    title: 'Atencion!',
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
                        fnc_volver_editar();
                    }
                });
            }
            else {
                // success
                swalInit.fire({
                    title: 'Bien Hecho!',
                    html: response.message,
                    type: 'success',
                    showCancelButton: false,
                    confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
                    confirmButtonClass: 'btn btn-primary',
                    buttonsStyling: false
                }).then(function (result) {
                    if (result.value) {
                        fnc_volver_editar();
                    }
                });
            }

            $('#group_adjunto-error').remove();
            $("#file_mot_adj").fileinput('clear');


        }).on('filebatchuploaderror', function (event, data, msg) {// La carga no pudo invocar el método
            console.log('filebatchuploaderror');
            console.log(event);
            console.log(data);
            console.log(msg);


            let swalInit = swal.mixin({
                buttonsStyling: false,
                confirmButtonClass: 'btn btn-primary',
                cancelButtonClass: 'btn btn-light'
            });

            // error
            swalInit.fire({
                title: 'Atencion!',
                html: 'ERROR :' + msg,
                type: 'error',
                showCancelButton: false,
                confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
                confirmButtonClass: 'btn btn-primary',
                buttonsStyling: false
            }).then(function (result) {
                if (result.value) {
                    $("#observacion_imagen_elemento").val('');
                    $("#file_mot_adj").fileinput('clear');
                 
                }
            });
            // /.error
        });
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
                    error.appendTo(element.parent().parent().parent().parent());
                }
                else {
                    error.insertAfter(element);
                }
            },
            rules: {
                observacion_imagen_elemento: {
                    required: true,
                    maxlength: 1000, //indicamos que debe de tener maximo de caracteres
                    minlength: 5 //indicamos que debe de tener minimo de caracteres
                },
                file_mot_adj: { adjuntos: true }
            },
            messages: {
                observacion_imagen_elemento: {
                    required: "El campo es requerido.",
                    maxlength: "El número máximo de caracteres permitidos es {0}.",
                    minlength: "El número mínimo de caracteres requeridos es {0}."
                },
                file_mot_adj: { adjuntos: "Archivo es Requerido." }
            },
            submitHandler: function (form) { //si todos los controles cumplen con las validaciones, se ejecuta este codigo  txt_cli_new_cupo_tcr

                if ($("#file_mot_adj").fileinput("getFilesCount") > 0)
                    $('#file_mot_adj').fileinput("upload");
                else
                    return false;
            }
        });

        jQuery.validator.addMethod("adjuntos", function (value, element) {

            $('#group_adjunto-error').remove();
            let obs = $('#observacion_imagen_elemento').val();

            console.log('Observaciones: ' + fnc_isnull(obs));
            console.log('Nro adjuntos: ' + $("#file_mot_adj").fileinput("getFilesCount"));

            if ($("#file_mot_adj").fileinput("getFilesCount") > 0 && fnc_isnull(obs)) {

                $('#file_mot_adj').fileinput("upload");

                return true;
            }
            else {

                $('#group_adjunto').append("<label id='group_adjunto-error' class='validation-invalid-label' for='file_mot_adj'>Requerido.</label>");

                return false;
            }
        });

    };

    return {
        init: function () {
            _componentUniform();
            _componentValidation();
            _componentFileUpload();

        }
    };
}();

document.addEventListener('DOMContentLoaded', function () {
    SubirImagen.init();
});

function fnc_volver_editar() {

    let idTipoElemento = $('#id_tipo_elemento').val();
    let idElemento = $('#id_elemento').val();

    window.location.href = $("#hiddenEditarElemento").val() + '?id_elemento=' + idElemento + '&id_tipo_elemento=' + idTipoElemento;
}

function fnc_isnull(str) {
    if (typeof str == 'undefined' || !str || str.length === 0 || str === "" || !/[^\s]/.test(str) || /^\s*$/.test(str) || str.replace(/\s/g, "") === "")
        return false;
    else
        return true;
}
