var Subir = function () {

    // Select2 for length menu styling
    var _componentSelect2 = function () {
        if (!$().select2) {
            console.warn('Warning - select2.min.js is not loaded.');
            return;
        }

        // Initialize
        $('.select2').select2();
    };;

    var _componentFileUpload = function () {
        if (!$().fileinput) {
            console.warn('Warning - fileinput.min.js is not loaded.');
            return;
        }

        console.log(JSON.parse($("#hiddenMaxFileSize").val()));

        $("#file_mot_adj").fileinput({
            language: 'es',
            allowedFileExtensions: JSON.parse($("#hiddenAllowedFileExtension").val()),
            showUpload: false,
            showRemove: false,
            uploadUrl: $("#hiddenUploadFile").val(),
            uploadAsync: false,
            overwriteInitial: false,
            minFileCount: 1,
            maxFileCount: 1,
            maxLength:150,
            maxFileSize: JSON.parse($("#hiddenMaxFileSize").val()),
            initialPreviewAsData: true,
            browseIcon: '<i class="icon-file-plus mr-2"></i>',
            browseClass: "btn btn-sm btn-primary",
            showCaption: false,
            mainClass: "d-grid",
            uploadExtraData: function(previewId, index) {
                var info = {
                    "strCategoria": $("#cod_categoria").val(),
                    "strTitulo": $("#doc_titulo").val(),
                    "strDescripcion": $("#doc_descripcion").val()
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


            $("#alert_upload_reclamos").html('');
            $("#alert_upload_reclamos").removeClass();

            if (response.fileName != null) {
                // alerta de archivo con detalle
                //$("#hddn_file64").val(response.downloadUrlBase64);
                //$("#hddn_file64_filename").val(response.fileName);
                $("#alert_upload_reclamos").html(response.message);


/*                $("#alert_upload_reclamos").addClass("alert alert-primary alert-styled-left alert-arrow-left alert-dismissible m-1");*/
                $("#alert_upload_reclamos").show();

            } else {
                    $("#alert_upload_reclamos").html(response.message);
/*                    $("#alert_upload_reclamos").removeClass();*/
/*                    $("#alert_upload_reclamos").addClass("alert alert-primary alert-styled-left alert-arrow-left alert-dismissible m-1");*/
                    $("#alert_upload_reclamos").show();
            }

/*            $("#tipo_archivo-error").remove();*/
           // $("#file_mot_adj").fileinput('clear');
            //$('#upload_data_row').addClass('d-none');

        }).on('filebatchuploaderror', function (event, data, msg) {// La carga no pudo invocar el método
            console.log(event);
            console.log(data);
            console.log(msg);
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
                cod_categoria: {
                    sinCategoria: true
                },
                doc_titulo: {
                    required: true,
                    sinEspacios: true,
                    maxlength: 50,                  //indicamos que debe de tener maximo de caracteres
                    minlength: 5                    //indicamos que debe de tener minimo de caracteres
                },
                doc_descripcion: {
                    maxlength: 100, //indicamos que debe de tener maximo de caracteres
                    minlength: 5 //indicamos que debe de tener minimo de caracteres
                },
                file_mot_adj: {
                    required: true,
                    maxlength: 150                 //indicamos que debe de tener maximo de caracteres
                }
            },
            messages: {
                cod_categoria: {
                    sinCategoria: "La CATEGORÍA es requerida."
                },
                doc_titulo: {
                    required: "El TÍTULO es requerido.",
                    sinEspacios: "El TÍTULO es inválido.",
                    maxlength: "El número máximo de caracteres permitidos es {0}.",
                    minlength: "El número mínimo de caracteres requeridos es {0}."
                },
                doc_descripcion: {
                    maxlength: "El número máximo de caracteres permitidos es {0}.",
                    minlength: "El número mínimo de caracteres requeridos es {0}."
                },
                file_mot_adj: {
                    required: "El ARCHIVO es requerido.",
                    maxlength: "El número máximo de caracteres permitidos es {0}."
                }
            },
            submitHandler: function (form) { //si todos los controles cumplen con las validaciones, se ejecuta este codigo  txt_cli_new_cupo_tcr

                if ($("#file_mot_adj").fileinput("getFilesCount") > 0)
                    $('#file_mot_adj').fileinput("upload");
                else
                    return false;

            }
        });

        jQuery.validator.addMethod("sinCategoria",
            function (value, element) {
                if (parseInt(value) == 0) {
                    return false;
                } else {
                    return true;
                }
            });

        jQuery.validator.addMethod("sinEspacios",
            function (value, element) {

                if (value.trim().length < 1) {
                    return false;
                } else {
                    return true;
                }
            });

        jQuery.validator.addMethod("required_file", function (value, element) {

            $('#cod_categoria-error').remove();

            if ($("#file_mot_adj").fileinput("getFilesCount") > 0)
                return true;
            else
                return false;
        });
    };

    return {
        init: function () {
            _componentFileUpload();
            _componentValidation();
            _componentSelect2();
        }
    };
}();

document.addEventListener('DOMContentLoaded', function () {
    Subir.init();
});

function fnc_volver() {

    window.location.href = $("#hiddenListFile").val();

}
 