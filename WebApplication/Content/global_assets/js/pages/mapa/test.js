var Test = function () {

    var _componentSelect2 = function () {
        if (!$().select2) {
            console.warn('Warning - select2.min.js is not loaded.');
            return;
        }

        $('.select2').select2();

        // - TIPO ELEMENTO
        $('#id_tipo_elemento').on('select2:select', function (e) {
 
            $('#div_marker-info').html('');

            $('#id_elemento').html('');
            $('#id_elemento').append('<option data-seguro="false" value="0">Seleccione ..</option>');

            if (this.value != '0') {
                fnc_get_elementos(this.value)
            }
        });

        // - ELEMENTO
        $('#id_elemento').on('select2:select', function (e) {

            $('#div_marker-info').html('');

            if (this.value != '0') {

                let TypeElement = $('#id_tipo_elemento').val();
                let element = this.value;

                fnc_marker_info(TypeElement, element)
            }
        });

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
            _componentSelect2();
            _componentDataTable();
        }
    };
}();

document.addEventListener('DOMContentLoaded', function () {
    Test.init();


});

function fn_marker_details(idTipoMarker, idMarker) {

    $('#cargando').fadeIn();
 
    $('#modal_detail_warning_span').text('');
    $('#modal_detail_warning').addClass('d-none');
    $('#field_img').addClass('d-none');
    $('#col_left').html('');
    $('#col_right').html('');
    $('#text_area_obs').html('');

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
        } else {
            $('#modal_detail_warning_span').html(response.exception);
            $('#modal_detail_warning').removeClass('d-none');
        }
    }).fail(function (jqXHR, textStatus) {
        console.log("fn_marker_details / Request failed: " + textStatus);
        $('#modal_detail_warning_span').html(textStatus);
        $('#modal_detail_warning').removeClass('d-none');
    }).always(function () {
        $('#text_area_obs').prop('readonly', true);
        $('#staticBackdrop').modal('show');
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
function fnc_get_elementos(idTipoMarker) {
    $("form").first().trigger("submit");
}

function fnc_marker_info(idTipoMarker, idMarker) {

    $('#cargando').fadeIn();
    $('#div_marker-info').html('');

    //Call Ajax Method
    $.ajax({
        url: $("#hddn_getMarkersById").val(),
        type: "POST",
        data: JSON.stringify({ idTipoMarker: idTipoMarker, idMarker: idMarker }),
        dataType: "json",
        contentType: "application/json"
    }).done(function (response) {

        if (response.status) {

            let markers = response.List;
            let str = '';

            if (markers != []) {
                $.each(markers, function (index, obj) {

                    str += '<div class="card border-0 pb-0 mb-1">';
                    str += '<div class="card-header bg-transparent header-elements-inline p-1">';
                    str += '<span class="text-uppercase font-size-sm font-weight-semibold"><i class="icon-pushpin mr-2"></i>' + obj.com_categoria + '</span>';
                    str += '</div>';

                    str += '<table class="table table-borderless table-xs border-top-0 my-2">';
                    str += '<tbody>';

                    str += '<tr>';
                    str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">ID:</td>';
                    str += '   <td class="text-right py-0 px-1">' + obj.com_id_inv + '</td>';
                    str += '</tr>';

                    str += '<tr>';
                    str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">DM:</td>';
                    str += '   <td class="text-right py-0 px-1">' + obj.com_dm + '</td>';
                    str += '</tr>';

                    str += '<tr>';
                    str += '   <td class="text-uppercase font-weight-semibold py-0 px-1 text-nowrap">Nom Ele:</td>';
                    str += '   <td class="py-0 px-1 text-right text-nowrap">' + obj.com_elemento_nombre + '</td>';
                    str += '</tr>';

                    str += '<tr>';
                    str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">Ruta:</td>';
                    str += '   <td class="text-right py-0 px-1">' + obj.com_ruta + '</td>';
                    str += '</tr>';
                    str += '<tr>';
                    str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">Tramo:</td>';
                    str += '   <td class="text-right py-0 px-1">' + obj.com_tramo_desc + '</td>';
                    str += '</tr>';
                    str += '<tr>';
                    str += '<tr>';
                    str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">ID Mop:</td>';
                    str += '   <td class="text-right py-0 px-1">' + obj.com_id_mop + '</td>';
                    str += '</tr>';

                    if (obj.com_map_ubicacion_flag) {
                        str += '<tr>';
                        str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">Ubicación:</td>';
                        str += '   <td class="text-right py-0 px-1">' + obj.com_map_ubicacion_inicio + '</td>';
                        str += '</tr>';
                    } else {
                        str += '<tr>';
                        str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">Inicio:</td>';
                        str += '   <td class="text-right py-0 px-1">' + obj.com_map_ubicacion_inicio + '</td>';
                        str += '</tr>';
                        str += '<tr>';
                        str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">Fin:</td>';
                        str += '   <td class="text-right py-0 px-1">' + obj.com_map_ubicacion_fin + '</td>';
                        str += '</tr>';

                    }
                    str += '</tbody>';
                    str += '</table>';

                    str += '<div class="card-body p-0 text-center">';
                    str += '   <button type="button" class="btn btn-link p-0" onclick="fn_marker_details(\'' + obj.com_tipo + '\',\'' + obj.com_id + '\')"><i class="icon-eye2 mr-2"></i>Ver detalle</button>';
                    str += '</div>';

                    /*console.log(obj.com_img_vig);*/

                    //if (obj.com_img_vig) {
                    //    str += '<div class="card-body p-1 text-center">';
                    //    str += '   <img width="200" src="' + obj.com_map_img + '" style="border: 1px solid rgba(0,0,0,.125);">';
                    //    str += '</div>';

                    //}

                    str += '</div>';

                });

                $('#div_marker-info').html(str);
            }
        }
        else {

        }
    }).fail(function (jqXHR, textStatus) {
        console.log("fnc_marker_info / Request failed: " + textStatus);
    }).always(function () {
        $('#cargando').fadeOut();
    });
    // Fin Call Ajax Method
 
}