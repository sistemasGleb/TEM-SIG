var Reportes = function () {

    var _componentDaterangeSingle = function () {
        if (!$().daterangepicker) {
            console.warn('Warning - daterangepicker.js is not loaded.');
            return;
        }

        $('.daterange-single-ini').daterangepicker({
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
            $("#startDate-error").remove();

            $("#startDate").val(picker.endDate.format('DD/MM/YYYY'));
            $(this).val(picker.endDate.format('DD/MM/YYYY'));
        });

        $('.daterange-single-fin').daterangepicker({
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
            $("#endDate-error").remove();

            $("#endDate").val(picker.endDate.format('DD/MM/YYYY'));
            $(this).val(picker.endDate.format('DD/MM/YYYY'));
        });
    };

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
            fixedColumns: true,
            dom: '<"datatable-header"fB><"datatable-scroll"t><"datatable-footer"ip>',
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
        var table = $('#tbl_plan_conservacion_ingreso').DataTable();

        table.buttons().remove();

        $("div.datatable-header").append('<div class="text-center"><h4>Reporte Mantenciones</h4></div>');
    }

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
                MaintenanceTypeSelected: { sinOpcionSelected: true },
                startDate: { required: true },
                endDate: { required: true }
            },
            messages: {
                MaintenanceTypeSelected: { sinOpcionSelected: "Este campo es requerido." },
                startDate: { required: "Este campo es requerido." },
                endDate: { required: "Este campo es requerido." }
            },
                submitHandler: function (form) { //si todos los controles cumplen con las validaciones, se ejecuta este codigo

                    fnc_buscarByFilter($('#MaintenanceTypeSelected').select2().val(), $('#startDate').val(), $('#endDate').val());

                }
            });

        jQuery.validator.addMethod("sinOpcionSelected",
            function (value, element) {
                if (value === '0') {
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
            _componentDaterangeSingle();
            _componentValidation();
        }
    }
}();

document.addEventListener('DOMContentLoaded', function () {
    Reportes.init();
});

function fnc_buscarByFilter(MaintenanceTypeSelected, startDate, endDate) {

    let oDatos = { "inputString": MaintenanceTypeSelected, "inputString2": startDate, "inputString3": endDate };

    $.ajax({
        url: $('#hddn_buscar').val(),
        cache: false,
        type: 'POST',
        async: false,
        data: oDatos,
    }).done(function (result) {
        $('#dvTableReporte').html(result);
    }).fail(function (jqXHR, status) {
        console.log('fnc_buscarByFilter / Request failed:' + status);
    }).always(function (jqXHR, status) {
        fnc_load_table_ingreso();
    });
}

function fnc_load_table_ingreso() {

    $('#tbl_plan_conservacion_ingreso').DataTable({
        autoWidth: false,
        ordering: false,
        destroy: true,
        fixedColumns: true,
        dom: '<"datatable-header"fB><"datatable-scroll"t><"datatable-footer"ip>',
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

    var table = $('#tbl_plan_conservacion_ingreso').DataTable();

    table.buttons().remove();

    $("div.datatable-header").append('<div class="text-center"><h4>Reporte Mantenciones</h4></div>');
}

function fnc_Exportar_plan() {
    let maintenanceTypeSelected = $('#MaintenanceTypeSelected').select2().val();
    let startDate = $('#startDate').val();
    let endDate = $('#endDate').val();

    let Root = { "inputString": maintenanceTypeSelected, "inputString2": startDate, "inputString3": endDate };

    let swalInit = swal.mixin({
        buttonsStyling: false,
        confirmButtonClass: 'btn btn-primary',
        cancelButtonClass: 'btn btn-light'
    });

    if (maintenanceTypeSelected == '0' || !fnc_IsNullOrEmpty(startDate) || !fnc_IsNullOrEmpty(endDate)) {
        swalInit.fire({
            title: 'Exportar Mantenciones.',
            html: 'ERROR : Todos los parametros son requeridos.',
            type: 'error',
            showCancelButton: false,
            confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
            confirmButtonClass: 'btn btn-primary',
            buttonsStyling: false
        });

        return false;
    }

    $("#cargando").fadeIn();

    //Call Ajax Method
    $.ajax({
        url: $("#hddn_get_reporte_plan").val(),
        type: "POST",
        data: JSON.stringify(Root),
        dataType: "json",
        contentType: "application/json"
    }).done(function (response) {

        if (response.status) {

            const fileContents = response.formModalExportar.fileContents;
            const contentType = response.formModalExportar.contentType;
            const fileDownloadName = response.formModalExportar.fileDownloadName;

            if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                //console.log('IE');
                var byteCharacters = atob(fileContents);
                var byteNumbers = new Array(byteCharacters.length);
                for (var i = 0; i < byteCharacters.length; i++) {
                    byteNumbers[i] = byteCharacters.charCodeAt(i);
                }
                var byteArray = new Uint8Array(byteNumbers);
                var blob = new Blob([byteArray], { type: "application/vnd.ms-excel" });
                window.navigator.msSaveOrOpenBlob(blob, fileDownloadName);
                return;
            }

            const link = document.createElement("a");
            document.body.appendChild(link);
            link.setAttribute("type", "hidden");
            link.href = "data:application/vnd.ms-excel;base64," + fileContents;
            link.download = fileDownloadName;
            link.click();
            document.body.removeChild(link);

        } else {
            console.log("fnc_Exportar_plan / Request failed:" + response.responseText);

            let swalInit = swal.mixin({
                buttonsStyling: false,
                confirmButtonClass: 'btn btn-primary',
                cancelButtonClass: 'btn btn-light'
            });

            swalInit.fire({
                title: 'Exportar Mantenciones.',
                html: 'ERROR :' + response.responseText,
                type: 'error',
                showCancelButton: false,
                confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
                confirmButtonClass: 'btn btn-primary',
                buttonsStyling: false
            });
        }
    }).fail(function (jqXHR, textStatus) {
        console.log("fnc_Exportar_plan / Request failed:" + textStatus);
    }).always(function () {
        $("#cargando").fadeOut();
    });
    // Fin Call Ajax Method
    return false;
    // fin Metodo
}

function fnc_IsNullOrEmpty(str) {
    if (typeof str == 'undefined' || !str || str.length === 0 || str === "" || !/[^\s]/.test(str) || /^\s*$/.test(str) || str.replace(/\s/g, "") === "")
        return false;
    else
        return true;
}