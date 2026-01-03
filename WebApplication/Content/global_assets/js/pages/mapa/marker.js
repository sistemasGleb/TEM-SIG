var Detail = function () {

    var _componentDataTable = function () {
        if (!$().DataTable) {
            console.warn('Warning - datatables.min.js is not loaded.');
            return;
        }

        $.extend($.fn.dataTable.defaults, {
            autoWidth: false,
            ordering: false,
            fixedColumns: true,
            dom: '<"datatable-header"><"datatable-scroll"t><"datatable-footer">',
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

    return {
        init: function () {
            _componentDataTable();
        }
    };
}();

document.addEventListener('DOMContentLoaded', function () {
    Detail.init();
});


function fn_marker_detail(idTipoMarker, idMarker) {

    let swalInit = swal.mixin({
        buttonsStyling: false,
        confirmButtonClass: 'btn btn-primary',
        cancelButtonClass: 'btn btn-light'
    });
 
    $('#modal_detail_warning_span').text('');
    $('#modal_detail_warning').addClass('d-none');
    $('#field_img').addClass('d-none');
    $('#col_left').html('');
    $('#col_right').html('');
    $('#text_area_obs').html('');

    $('#cargando').fadeIn();

    //Call Ajax Method
    $.ajax({
        url: $("#hddn_getMarkersDetail").val(),
        type: "POST",
        data: JSON.stringify({ idTipoMarker: idTipoMarker, idMarker: idMarker }),
        dataType: "json",
        contentType: "application/json"
    }).done(function (response) {

        if (response.status) {
            $('#title_element').html('');
            $('#title_element').html(response.titulo);
            $('#col_left').html(response.MarkerLeft);
            $('#col_right').html(response.MarkerRight);
            $('#text_area_obs').text(response.obs);

            load_table(response.matenciones);

            if (response.img_flag) {
                $('#img_elemento').html('');
                $('#img_elemento').html(response.img);

                $('#field_img').removeClass('d-none');
            }

            $('#text_area_obs').prop('readonly', true);
            $('#staticBackdrop').modal('show');

        } else {
            console.log("fn_marker_detail / response.status: false \n" + response.exception);
            //error
            swalInit.fire({
                title: 'Atencion!',
                html: 'ERROR :' + response.exception,
                type: 'error',
                showCancelButton: false,
                confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
                confirmButtonClass: 'btn btn-primary',
                buttonsStyling: false
            });
        }
    }).fail(function (jqXHR, textStatus) {
        console.log("fn_marker_detail / Request failed: " + textStatus);

        //error
        swalInit.fire({
            title: 'Atencion!',
            html: 'ERROR :' + 'Errores en la carga del detalle del marcador.',
            type: 'error',
            showCancelButton: false,
            confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
            confirmButtonClass: 'btn btn-primary',
            buttonsStyling: false 
        });

    }).always(function () {
        //$('#text_area_obs').prop('readonly', true);
        //$('#staticBackdrop').modal('show');
        $('#cargando').fadeOut();
    });
    // Fin Call Ajax Method
}
function load_table(list) {

    var table_mant = $('#table_maitenance_element').DataTable({
        destroy: true,
        paging: false,
        retrieve: false,
        ordering: false,
        data: list,
        dom: '<"datatable-header"Bl><"datatable-scroll"t><"datatable-footer"ip>',
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

    $('#table_maitenance_element').DataTable().buttons().remove();

};