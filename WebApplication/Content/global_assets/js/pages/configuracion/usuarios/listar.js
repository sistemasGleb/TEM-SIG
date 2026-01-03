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
                    text: 'Agregar<i class="icon-googleplus5 ml-2"></i>',
                    className: 'btn bg-teal-400',
                    action: function () {
                        window.location.href = $("#usuario_agregar").val();
                    }
                }
            ]
        });

        $("div.datatable-header").append('<div class="text-center"><h4>Administración De Usuarios</h4></div>');
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

function fncEliminarUsuario(usuario,login) {
    bootbox.confirm({
        title: 'Eliminar Usuario',
        message: '¿Está seguro de que desea eliminar el usuario ' + login.toUpperCase() + '?',
        html:'',
        buttons: {
            confirm: {
                label: 'Sí, Eliminar usuario',
                className: 'btn-danger'
            },
            cancel: {
                label: 'No, Mantener usuario',
                className: 'btn-light'
            }
        },
        callback: function (result) {
            if (result)
                window.location.href = $("#usuario_eliminar").val() + "/" + usuario + "?login=" + login;
        }
    });
}