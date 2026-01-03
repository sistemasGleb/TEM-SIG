var Listar = function () {
    var _componentDataTable = function () {
        if (!$().DataTable) {
            console.warn('Warning - datatables.min.js is not loaded.');
            return;
        }

        $.extend($.fn.dataTable.defaults, {
            autoWidth: false,
            columnDefs: [{
                orderable: false,
            }],
            dom: '<"datatable-header"fBl><"datatable-scroll"t><"datatable-footer"ip>',
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

        $('.table').DataTable({
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Todos"]],
            order: [[0, 'asc']],
            buttons: [
                {
                    text: 'Agregar Cargo<i class="icon-googleplus5 ml-2"></i>',
                    className: 'btn bg-teal-400',
                    action: function () {
                        window.location.href = $("#cargo_agregar").val();
                    }
                }
            ]
        });

        $("div.datatable-header").append('<div class="text-center"><h4>Administración De Cargos</h4></div>');
    }

    return {
        init: function () {
            _componentDataTable();
        }
    }
}();

document.addEventListener('DOMContentLoaded', function () {
    Listar.init();
});

function fncEliminarCargo(id, cargo) {

    var isValid = false;
    //Call Ajax Method
    $.ajax({
        url: $("#hddn_cargo_validar").val(),
        type: "POST",
        data: JSON.stringify({ cargo: cargo }),
        dataType: "json",
        contentType: "application/json"
    }).done(function (response) {

        if (response) {
            bootbox.confirm({
                title: 'Eliminar Cargo',
                message: '¿Está seguro de que desea eliminar el Cargo ' + cargo.toUpperCase() + '?',
                html: '',
                buttons: {
                    confirm: {
                        label: 'Sí, Eliminar Cargo',
                        className: 'btn-danger'
                    },
                    cancel: {
                        label: 'No, Mantener Cargo',
                        className: 'btn-light'
                    }
                },
                callback: function (result) {
                    if (result)
                        window.location.href = $("#cargo_eliminar").val() + "/" + id + "?cargo=" + cargo;
                }
            });
        }
        else {
 

            let swalInit = swal.mixin({
                buttonsStyling: false,
                confirmButtonClass: 'btn btn-primary',
                cancelButtonClass: 'btn btn-light'
            });

            swalInit.fire({
                title: 'Eliminar cargo',
                html: 'ERROR : No se puede eiminar el cargo seleccionado, está asignado a uno o mas usuarios.' ,
                type: 'error',
                showCancelButton: false,
                confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
                confirmButtonClass: 'btn btn-primary',
                buttonsStyling: false
            });


        }

    }).fail(function (jqXHR, textStatus) {
        console.log("fncValidaEliminar / Request failed : " + textStatus);
    }).always(function () {
   /*     $('#cargando').fadeOut();*/
    });
    // /.Call Ajax Method

}