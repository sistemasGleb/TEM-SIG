var Agregar = function () {

    var _componentValidation = function () {
        if (!$().validate) {
            console.warn('Warning - validate.min.js is not loaded.');
            return;
        }

        var validator = $('.form-validate-agregar').validate({
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
                MaintenanceTypeSelected: { sinMaintenanceType: true },
                ActivitySelected: { sinActividadSelected: true },
                MaintenanceDate: { required: true },
                ResponsibleSelected: { sinResponsibleSelected: true },
                quantity: { required: true }
            },
            messages: {
                MaintenanceTypeSelected: { sinMaintenanceType: "Este campo es requerido." },
                ActivitySelected: { sinActividadSelected: "Este campo es requerido." },
                MaintenanceDate: { required: "Este campo es requerido." },
                ResponsibleSelected: { sinResponsibleSelected: "Este campo es requerido." },
                quantity: { required: "Este campo es requerido." }
            }
            //},
            //submitHandler: function (form) {
            //    //si todos los controles cumplen con las validaciones, se ejecuta este codigo
            //    fnc_guardarNuevaMantencion();
            //}
        });

        jQuery.validator.addMethod("sinActividadSelected",
            function (value, element) {

                if (value === '0') {
                    return false;
                } else {
                    return true;
                }
            });

        jQuery.validator.addMethod("sinMaintenanceType",
            function (value, element) {
                if (value === '0') {
                    return false;
                } else {
                    return true;
                }
            });

        jQuery.validator.addMethod("sinElementTypeSelected",
            function (value, element) {
                if (value === '0') {
                    return false;
                } else {
                    return true;
                }
            });

        jQuery.validator.addMethod("sinElementCodeSelected",
            function (value, element) {
                if (value === '0') {
                    return false;
                } else {
                    return true;
                }
            });

        jQuery.validator.addMethod("sinResponsibleSelected",
            function (value, element) {
                if (value === '0') {
                    return false;
                } else {
                    return true;
                }
            });
    };
    var _componentSelect2 = function () {
        if (!$().select2) {
            console.warn('Warning - select2.min.js is not loaded.');
            return;
        }

        $('.select2').select2();
    };

    var _componentDaterangeSingle = function () {
        if (!$().daterangepicker) {
            console.warn('Warning - daterangepicker.js is not loaded.');
            return;
        }

        $('.daterange-single-add').daterangepicker({
            singleDatePicker: true,
            autoUpdateInput: false,
            locale: {
                format: 'DD/MM/YYYY',
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                firstDay: 1
            },
            startDate: new Date(),
        }).on('apply.daterangepicker', function (ev, picker) {
            $("#MaintenanceDate-error").remove();
            $("#MaintenanceDate").val(picker.endDate.format('DD/MM/YYYY'));
            $(this).val(picker.endDate.format('DD/MM/YYYY'));
        });
    };
 
    return {
        init: function () {
            _componentSelect2();
            _componentDaterangeSingle();
            _componentValidation();
        }
    }
}();

document.addEventListener('DOMContentLoaded', function () {
    Agregar.init();
});

// A $( document ).ready() block.
$(document).ready(function () {
});

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;

    return true;
}

// - Indicates whether the specified string is null or an empty string ("").
function fnc_IsNullOrEmpty(str) {

    if (typeof str == 'undefined' || !str || str.length === 0 || str === "" || !/[^\s]/.test(str) || /^\s*$/.test(str) || str.replace(/\s/g, "") === "")
        return false;
    else
        return true;
}

$(document).on('change', '#ElementTypeSelected', function () {
 
    $('#ElementCodeSelected').html('');
    $('#ElementCodeSelected').append('<option value="0">Seleccione ...</option>');

    let elementType = $(this).val();

    if (elementType == "0")
        return;

    let Root = { "inputString": elementType };

    //Call Ajax Method
    $.ajax({
        url: $("#hddn_BuscarElementoByType").val(),
        type: "POST",
        data: JSON.stringify(Root),
        dataType: "json",
        contentType: "application/json"
    }).done(function (response) {

        if (response.status) {

            let listado_elementos = response.list;

            listado_elementos = listado_elementos.map(
                function (listado_elementos) {
                    $('#ElementCodeSelected').append('<option value="' + listado_elementos.Value + '">' + listado_elementos.Text.toUpperCase() + '</option>');
                });
        }
        else {
            console.log("ElementTypeSelected / Request error: " + response.responseText);
        }
    }).fail(function (jqXHR, textStatus) {
        console.log("change_ElementTypeSelected / Request failed: " + textStatus);
    }).always(function () {

    });
    // Fin Call Ajax Method
});
$(document).on('change', '#ActivitySelected', function () {

    let elementType = $(this).val();

    if (elementType == "0")
        return;

    let Root = { "inputString": elementType };

    //Call Ajax Method
    $.ajax({
        url: $("#hddn_BuscarActivityByCode").val(),
        type: "POST",
        data: JSON.stringify(Root),
        dataType: "json",
        contentType: "application/json"
    }).done(function (response) {
        if (response.status) {

            $('#unit').val(response.filteredActivity.unidad_id);
            $('#quantity_label').text(response.filteredActivity.unidad_nom);
        }
        else {
            console.log("ActivitySelected_change / Request error: " + response.responseText);
            $('#unit').val(0);
            $('#quantity_label').text("-");
        }
    }).fail(function (jqXHR, textStatus) {
        console.log("ActivitySelected_change / Request failed: " + textStatus);
    }).always(function () {

    });
    // Fin Call Ajax Method

});
