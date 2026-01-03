var Subir = function () {

    var _componentDatepicker = function () {

        $('.datepicker').datepicker();

        $('#education-range input').each(function () {
            $(this).datepicker({
                autoclose: true,
                format: "yyyy",
                viewMode: "years",
                minViewMode: "years",
            });
            $(this).datepicker('clearDates');
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
                startDate: { required: true, min: 1 },
                file_mot_adj: { adjuntos :true}
            },
            messages: {
                startDate: { required: "Este campo es requerido.", min: "Periodo Incorrecto." },
                file_mot_adj: { adjuntos: "Este campo es requerido."  }
            },
            submitHandler: function (form) { //si todos los controles cumplen con las validaciones, se ejecuta este codigo

                console.log('Si todos los controles cumplen con las validaciones, se ejecuta este codigo');

                if ($("#file_mot_adj").fileinput("getFilesCount") > 0) {
                    $('#file_mot_adj-error').remove();
                    $('#file_mot_adj').fileinput("upload");
                }
                else {
                    return false;
                }

            }
        });
        jQuery.validator.addMethod("adjuntos", function (value, element) {

            $('#file_mot_adj-error').remove();

            if ($("#file_mot_adj").fileinput("getFilesCount") > 0) {
                $('#file_mot_adj-error').remove();
                return true;
            }
            else {
                $('#group_file_mot_adj').after("<label id='file_mot_adj-error' class='validation-invalid-label' for='file_mot_adj'>Requerido.</label>");
                return false;
            }
        });
    };
 
    var _componentFileUpload = function () {
        if (!$().fileinput) {
            console.warn('Warning - fileinput.min.js is not loaded.');
            return;
        }

        $("#file_mot_adj").fileinput({
            language: 'es',
            allowedFileExtensions: JSON.parse($("#hddn_AllowedFileExtension").val()),
            showUpload: false,
            showRemove: false,
            uploadUrl: $("#hddn_UploadFile").val(),
            uploadAsync: false,
            overwriteInitial: false,
            minFileCount: 1,
            maxFileCount: 1,
            maxLength: 250,
            maxFileSize: JSON.parse($("#hddn_nMaxFileSize").val()),
            initialPreviewAsData: true,
            browseIcon: '<i class="icon-file-plus mr-2"></i>',
            browseClass: "btn btn-sm btn-primary",
            showCaption: false,
            mainClass: "d-grid",
            uploadExtraData: function (previewId, index) {
                var info = {
                    "inputString1": $('#startDate').val(),
                    "inputString2": $('#version').val(),
                    "inputString3": $('#observations').val(),
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

            console.log('filebatchuploadsuccess-1');

            if (response.fileName != null) {
                // alerta de archivo con detalle
            } else {
                // archivo sin detalle
            }
        }).on('filebatchuploaderror', function (event, data, msg) {// La carga no pudo invocar el método
            console.log(event);
            console.log(data);
            console.log(msg);
        });
    };
 
    return {
        init: function () {
            _componentValidation();
            _componentFileUpload();
            _componentDatepicker();
        }
    }
}();

document.addEventListener('DOMContentLoaded', function () {
    Subir.init();
});
