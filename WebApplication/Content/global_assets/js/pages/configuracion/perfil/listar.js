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
                    text: 'Agregar Perfil<i class="icon-googleplus5 ml-2"></i>',
                    className: 'btn bg-teal-400',
                    action: function () {
                        window.location.href = $("#perfil_agregar").val();
                    }
                }
            ]
        });

        $("div.datatable-header").append('<div class="text-center"><h4>Administración De Perfiles</h4></div>');
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

function fncEliminarPerfil(id, perfil) {
 
    //Call Ajax Method
    $.ajax({
        url: $("#hddn_perfil_validar").val(),
        type: "POST",
        data: JSON.stringify({ id: id }),
        dataType: "json",
        contentType: "application/json"
    }).done(function (response) {

        if (response) {
            bootbox.confirm({
                title: 'Eliminar Perfil',
                message: '¿Está seguro de que desea eliminar el Perfil ' + perfil.toUpperCase() + '?',
                html: '',
                buttons: {
                    confirm: {
                        label: 'Sí, Eliminar Perfil',
                        className: 'btn-danger'
                    },
                    cancel: {
                        label: 'No, Mantener Perfil',
                        className: 'btn-light'
                    }
                },
                callback: function (result) {
                    if (result)
                        window.location.href = $("#perfil_eliminar").val() + "/" + id + "?perfil=" + perfil;
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
                title: 'Eliminar Perfil',
                html: 'ERROR : No se puede eiminar el Perfil seleccionado, está asignado a uno o mas Cargos.',
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


//function fncEliminarPerfil(id, perfil) {
//    bootbox.confirm({
//        title: 'Eliminar Perfil',
//        message: '¿Está seguro de que desea eliminar el Perfil ' + perfil.toUpperCase() + '?',
//        html: '',
//        buttons: {
//            confirm: {
//                label: 'Sí, Eliminar Perfil',
//                className: 'btn-danger'
//            },
//            cancel: {
//                label: 'No, Mantener Perfil',
//                className: 'btn-light'
//            }
//        },
//        callback: function (result) {
//            if (result)
//                window.location.href = $("#perfil_eliminar").val() + "/" + id + "?perfil=" + perfil;
//        }
//    });
//}