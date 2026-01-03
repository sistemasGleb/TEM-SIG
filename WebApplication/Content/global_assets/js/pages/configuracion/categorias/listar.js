var Categorias = function () {

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

        $('.table').DataTable({
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Todos"]],
            order: [[0, 'asc']],
            buttons: [
                {
                    text: 'Volver<i class="icon-backward2"></i>',
                    className: 'btn btn-light mr-3',
                    action: function () {
                        window.location.href = $("#hidden_Listar_Docs").val();
                    }
                },
                {
                    text: 'Agregar Biblioteca<i class="icon-googleplus5 ml-2"></i>',
                    className: 'btn bg-teal-400 agregar-categoria',
                    action: function () {
                        window.location.href = $("#categoria_agregar").val();
                    }
                }
            ]
        });

        $(".agregar-categoria").css("background-color", "#5D3260");

        $("div.datatable-header").append('<div class="text-center"><h4>Administración de Bibliotecas</h4></div>');
    }

    return {
        init: function () {
            _componentDataTable();
        }
    }
}();

document.addEventListener('DOMContentLoaded', function () {
    Categorias.init();
});