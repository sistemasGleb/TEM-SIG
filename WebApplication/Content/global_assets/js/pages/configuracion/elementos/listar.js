var Listar = function () {

    var _componentDatatable = function () {
        if (!$().DataTable) {
            console.warn('Warning - datatables.min.js is not loaded.');
            return;
        }

        $.extend($.fn.dataTable.defaults, {
            destroy: true,
            autoWidth: false,
            responsive: true,
            columnDefs: [{
                orderable: false,
            }],
            dom: '<"datatable-header"fBl><"datatable-scroll"t><"datatable-footer"ip>',
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Todos"]],
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
            },

        });

        ///* Control position*/
        //$('.datatable-responsive-control-right').DataTable({
        //    lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Todos"]],
        //    order: [[0, 'asc']],
        //    responsive: {
        //        details: {
        //            type: 'column',
        //            target: -1
        //        }
        //    },
        //    columnDefs: [
        //        {
        //            className: 'control',
        //            orderable: false,
        //            targets: -1
        //        },
        //        {
        //            width: "100px",
        //            targets: [10]
        //        },
        //        {
        //            orderable: false,
        //            targets: [10]
        //        }
        //    ]
        //});

        $('.datatable-responsive-control-right').DataTable().buttons().remove();

        $("div.datatable-header").append('<div class="text-center"><h4>Administración de Elementos</h4></div>');
    }

    return {
        init: function () {

            _componentDatatable();

        }
    }
}();

document.addEventListener('DOMContentLoaded', function () {
    Listar.init();
});

function fnc_get_marker_history(idTipoMarker, idMarker) {


    //Call Ajax Method
    $.ajax({
        url: $("#hddn_getMarkerHistory").val(),
        type: "POST",
        data: JSON.stringify({ idTipoMarker: idTipoMarker, idMarker: idMarker }),
        dataType: "json",
        contentType: "application/json"
    }).done(function (response) {

        if (response.status) {

            var table_mant = $('#table_maitenance_element_detail').DataTable({
                destroy:true,
                paging: false,
                retrieve: false,
                ordering: false,
                data: response.list,
                dom: '<"datatable-header"fBl><"datatable-scroll"t><"datatable-footer"ip>',
                columns: [
                    {
                        data: "tra_fec_ing", className: 'text-lowercase text-capitalize text-center text-nowrap',
                        "render": function (value) {
                            if (value === null) return "";
                            return moment(value).format('DD/MM/YYYY');
                        }
                    },
                    { data: "tra_tip_mant_nom", className: 'text-lowercase text-capitalize text-left text-nowrap' },
                    { data: "tra_tip_act_nom", className: 'text-lowercase text-capitalize text-left text-nowrap' },
                    { data: "tra_tip_ele_nom", className: 'text-lowercase text-capitalize text-left text-nowrap' },
                    { data: "tra_ele_nom", className: 'text-lowercase text-capitalize text-left text-nowrap' },
                    { data: "tra_res_nom", className: 'text-lowercase text-capitalize text-left text-nowrap' },
                    { data: "tra_can", className: 'text-lowercase text-capitalize text-right text-nowrap' },
                    { data: "tra_uni_nom", className: 'text-lowercase text-capitalize text-left text-nowrap' },
                    { data: "tra_obs", className: 'text-lowercase text-capitalize text-left text-nowrap' },
                ],
                order: [[0, 'desc']]
            });

            $('#table_maitenance_element_detail').DataTable().buttons().remove();

        } else {
            console.log("fnc_get_marker_history / Request failed: " + response.exception);
        }
    }).fail(function (jqXHR, textStatus) {
        console.log("fnc_get_marker_history / Request failed: " + textStatus);
    }).always(function () {

        $('#tag_modal_title').html(idMarker);
        $('#staticBackdropHistorial').modal('show');
        $('#cargando').fadeOut();
    });
    // Fin Call Ajax Method
}