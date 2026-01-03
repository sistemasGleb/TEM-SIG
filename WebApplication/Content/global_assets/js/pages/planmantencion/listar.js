var Plan = function () {

    var _componentSelect2 = function () {
        if (!$().select2) {
            console.warn('Warning - select2.min.js is not loaded.');
            return;
        }

        $('.select2').select2();
    };

    var _componentDataTable = function () {
        if (!$().DataTable) {
            console.warn('Warning - datatables.min.js is not loaded.');
            return;
        }

        $.extend($.fn.dataTable.defaults, {
            autoWidth: false,
            ordering: false,
            //columnDefs: [
            //    { width: 200, targets: 0 }
            //],
            fixedColumns: true,
            dom: '<"datatable-header"><"datatable-scroll"t><"datatable-footer"ip>',
            language: {
                info: "Mostrando página _PAGE_ de _PAGES_",
                zeroRecords: "No se encontraron registros.",
                infoEmpty: "No se encontraron registros.",
                emptyTable: "No hay información para los filtros indicados.",
                infoFiltered: "(Filtrando entre _MAX_ registros)",
                search: '_INPUT_',
                searchPlaceholder: 'Buscar...',
                lengthMenu: '<span>Mostrar:</span> _MENU_',
                paginate: { 'first': 'Primero', 'last': 'Ultimo', 'next': $('html').attr('dir') == 'rtl' ? '&larr;' : '&rarr;', 'previous': $('html').attr('dir') == 'rtl' ? '&rarr;' : '&larr;' }
            }
        });
    };

    var _componentValidation = function () {
        if (!$().validate) {
            console.warn('Warning - validate.min.js is not loaded.');
            return;
        }

        var validator = $('.form-validate-plan').validate({
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
                PlanningSelected: { sinPlanningSelected: true }
            },
            messages: {
                PlanningSelected: { sinPlanningSelected: "Este campo es requerido." }
            },
            submitHandler: function (form) { //si todos los controles cumplen con las validaciones, se ejecuta este codigo

                let idPlannerSelected = $('#PlanningSelected option:selected').val();
                console.log(idPlannerSelected);
                fnc_reload_planner(idPlannerSelected);
            }
        });

        jQuery.validator.addMethod("sinPlanningSelected",
            function (value, element) {
                if (value === '0') {
                    $('#observations').val(''); 
                    $('#tbl_plan_conservacion tbody').empty();
                    return false;
                } else {
                    return true;
                }
            });
    };

    return {
        init: function () {
            _componentSelect2();
            _componentDataTable();
            _componentValidation();
        }
    }
}();

document.addEventListener('DOMContentLoaded', function () {
    Plan.init();
});

function fnc_reload_planner(guidid) {

    let oDatos = { "inputString": guidid };

    $.ajax({
        url: $('#plan_grilla').val(),
        cache: false,
        type: 'POST',
        async: false,
        data: oDatos,
    }).done(function (result) {
        /*//console.log(result);*/
        $('#dvUserdetails').html(result);
    }).fail(function (jqXHR, status) {
        console.log(status);
    }).always(function (jqXHR, status) {
 
        $('#observations').val($('#some_property').val()); 
    });
}
